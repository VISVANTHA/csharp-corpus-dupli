using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderKit
{
    /// <summary>An order and its lines.</summary>
    public sealed class Order : IPriceable
    {
        private readonly List<OrderLine> _lines = new List<OrderLine>();
        private string _customerTier;
        private string _cachedKey;

        public Order(string id, string customerTier)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            _customerTier = customerTier ?? "standard";
        }

        public string Id { get; }
        public OrderStatus Status { get; set; } = OrderStatus.Draft;

        public string CustomerTier
        {
            get => _customerTier;
            set => _customerTier = value ?? "standard";
        }

        public IList<OrderLine> Lines => _lines.ToList();

        public void AddLine(OrderLine line) =>
            _lines.Add(line ?? throw new ArgumentNullException(nameof(line)));

        public bool IsEmpty() => _lines.Count == 0;

        public int LineCount() => _lines.Count;

        /// <summary>A STATIC local function: it cannot capture, which the compiler
        /// enforces. `static` on a local function is this family's language version.</summary>
        public long SubtotalCents()
        {
            static long Extend(OrderLine line) => line.ExtendedCents();

            long total = default;
            foreach (OrderLine l in _lines)
            {
                total += Extend(l);
            }
            return total;
        }

        public IEnumerable<string> DistinctCategories() =>
            _lines.Select(l => l.Category).Distinct().OrderBy(c => c);

        public string FirstSku() => _lines.FirstOrDefault()?.Sku;

        /// <summary>Index and range operators over the line list.</summary>
        public OrderLine LastLine() => _lines.Count == 0 ? null : _lines.ToArray()[^1];

        public IList<OrderLine> FirstTwo() =>
            _lines.Count <= 2 ? _lines.ToList() : _lines.ToArray()[0..2].ToList();

        /// <summary>Null-coalescing assignment: assign only if currently null.</summary>
        public string CachedKey()
        {
            _cachedKey ??= $"order:{Id}";
            return _cachedKey;
        }

        public string Describe()
        {
            int lines = LineCount();
            long cents = SubtotalCents();
            var summary = (lines, cents);
            return $"{Id}: {summary.lines} lines, {summary.cents}c, {Status.Label()}";
        }

        public long BaseCents() => SubtotalCents();

        public string PricingKey() => $"order:{Id}";

        public override string ToString() =>
            $"Order({Id}, {_lines.Count} lines, {Status.Label()})";
    }
}
