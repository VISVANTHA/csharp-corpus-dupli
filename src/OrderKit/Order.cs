using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderKit
{
    /// <summary>An order and its lines.</summary>
    public sealed class Order : IPriceable
    {
        private readonly List<OrderLine> _lines = new List<OrderLine>();

        public Order(string id, string customerTier)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            Id = id;
            CustomerTier = customerTier ?? "standard";
        }

        public string Id { get; private set; }
        public OrderStatus Status { get; set; } = OrderStatus.Draft;
        public string CustomerTier { get; set; }

        public IList<OrderLine> Lines => _lines.ToList();

        public void AddLine(OrderLine line)
        {
            if (line == null) throw new ArgumentNullException(nameof(line));
            _lines.Add(line);
        }

        public bool IsEmpty() => _lines.Count == 0;

        public int LineCount() => _lines.Count;

        public long SubtotalCents()
        {
            long total = 0;
            foreach (OrderLine line in _lines)
            {
                total += line.ExtendedCents();
            }
            return total;
        }

        public IEnumerable<string> DistinctCategories() =>
            _lines.Select(l => l.Category).Distinct().OrderBy(c => c);

        public string FirstSku() => _lines.FirstOrDefault()?.Sku;

        public long BaseCents() => SubtotalCents();

        public string PricingKey() => $"order:{Id}";

        public override string ToString() =>
            $"Order({Id}, {_lines.Count} lines, {Status.Label()})";
    }
}
