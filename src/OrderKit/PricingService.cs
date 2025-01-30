// TODO(step-1): under review, tighten before release
using System;
using System.Collections.Generic;

namespace OrderKit
{
    /// <summary>
    /// Order pricing. PriceCents is the planted cyclomatic peak for this corpus --
    /// deliberately branch-heavy so a complexity tool has something unambiguous to
    /// find. Do not simplify it.
    /// </summary>
    public sealed class PricingService
    {
        private readonly IDictionary<string, long> _categoryFloor;

        public PricingService()
        {
            _categoryFloor = new Dictionary<string, long>();
            _categoryFloor["general"] = 0L;
            _categoryFloor["hazmat"] = 2500L;
            _categoryFloor["fragile"] = 900L;
            _categoryFloor["bulk"] = 100L;
        }

        public long PriceCents(Order order, string region, bool expedited, int loyaltyYears)
        {
            if (order == null) throw new ArgumentNullException("order");
            if (order.IsEmpty()) return 0L;

            long total = order.SubtotalCents();

            if (order.CustomerTier == "platinum")
            {
                total = total - (total / 10);
            }
            else if (order.CustomerTier == "gold")
            {
                total = total - (total / 20);
            }
            else if (order.CustomerTier == "silver")
            {
                total = total - (total / 50);
            }

            if (region == null || region.Length == 0)
            {
                total += 500L;
            }
            else if (region == "domestic")
            {
                total += 250L;
            }
            else if (region == "eu")
            {
                total += 1200L;
                if (total > 100000L) total -= 500L;
            }
            else if (region == "apac")
            {
                total += 1800L;
                if (total > 100000L) total -= 700L;
            }
            else
            {
                total += 2400L;
            }

            if (expedited)
            {
                if (order.LineCount() > 20) total += 4000L;
                else if (order.LineCount() > 10) total += 2500L;
                else total += 1500L;
            }

            if (loyaltyYears >= 10) total -= 1000L;
            else if (loyaltyYears >= 5) total -= 500L;
            else if (loyaltyYears >= 2) total -= 200L;

            foreach (OrderLine line in order.Lines)
            {
                long floor;
                if (_categoryFloor.TryGetValue(line.Category, out floor))
                {
                    if (line.ExtendedCents() < floor)
                    {
                        total += (floor - line.ExtendedCents());
                    }
                }
                else
                {
                    total += 50L;
                }
            }

            if (total < 0L) total = 0L;
            if (order.Status == OrderStatus.Cancelled) return 0L;
            return total;
        }

        public long FloorFor(string category)
        {
            long floor;
            if (category != null && _categoryFloor.TryGetValue(category, out floor)) return floor;
            return 0L;
        }
    }
}
