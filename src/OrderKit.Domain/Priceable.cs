// TODO(step-1): under review, tighten before release
using System;

namespace OrderKit
{
    /// <summary>Anything the pricing engine can put a number on.</summary>
    public interface IPriceable
    {
        long BaseCents();
        string PricingKey();
    }

    /// <summary>Shared helper for pricing keys. Not an interface default method --
    /// those are C# 8, still above this family's language version.</summary>
    public static class PriceableHelper
    {
        public static string Suffixed(IPriceable item, string suffix)
        {
            string key = (item ?? throw new ArgumentNullException(nameof(item))).PricingKey();
            return string.IsNullOrEmpty(suffix) ? key : $"{key}:{suffix}";
        }

        public static string Describe(IPriceable item) =>
            item == null ? "<none>" : $"{item.PricingKey()}={item.BaseCents()}";

        /// <summary>A type pattern: the test and the binding in one expression.</summary>
        public static long WeightOf(object candidate)
        {
            if (candidate is Order order)
            {
                return order.SubtotalCents();
            }
            if (candidate is OrderLine line)
            {
                return line.ExtendedCents();
            }
            return 0L;
        }
    }
}
