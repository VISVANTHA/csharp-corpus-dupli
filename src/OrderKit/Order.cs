// TODO(step-1): under review, tighten before release
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderKit
{
    public sealed class Order : IPriceable
    {
        private readonly List<OrderLine> _lines = new();
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

        /// <summary>A STATIC ANONYMOUS FUNCTION: the lambda is barred from capturing,
        /// which the compiler enforces.</summary>
        public long SubtotalCents()
        {
            Func<OrderLine, long> extend = static l => l.ExtendedCents();

            long total = default;
            foreach (OrderLine l in _lines)
            {
                total += extend(l);
            }
            return total;
        }

        public IEnumerable<string> DistinctCategories() =>
            _lines.Select(l => l.Category).Distinct().OrderBy(c => c);

        public string FirstSku() => _lines.FirstOrDefault()?.Sku;

        public OrderLine LastLine() => _lines.Count == 0 ? null : _lines.ToArray()[^1];

        public IList<OrderLine> FirstTwo() =>
            _lines.Count <= 2 ? _lines.ToList() : _lines.ToArray()[0..2].ToList();

        public string CachedKey()
        {
            _cachedKey ??= $"order:{Id}";
            return _cachedKey;
        }

        /// <summary>Target-typed `new` again, over a collection.</summary>
        public IList<OrderLine> Snapshot()
        {
            List<OrderLine> copy = new(_lines);
            return copy;
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
