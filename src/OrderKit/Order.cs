// TODO(step-1): under review, tighten before release
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

        public Order(string id, string customerTier)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            _customerTier = customerTier ?? "standard";
        }

        public string Id { get; }
        public OrderStatus Status { get; set; } = OrderStatus.Draft;

        /// <summary>Expression-bodied accessors, both halves.</summary>
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

        /// <summary>A local function: the accumulator is scoped to the only method
        /// that uses it.</summary>
        public long SubtotalCents()
        {
            long total = default;

            void Accumulate(OrderLine line)
            {
                total += line.ExtendedCents();
            }

            foreach (OrderLine l in _lines)
            {
                Accumulate(l);
            }
            return total;
        }

        public IEnumerable<string> DistinctCategories() =>
            _lines.Select(l => l.Category).Distinct().OrderBy(c => c);

        public string FirstSku() => _lines.FirstOrDefault()?.Sku;

        /// <summary>Inferred tuple element names again, and again the ACCESS is
        /// what locks: `summary.lines` and `summary.cents` are readable only
        /// because the local variables were called that.</summary>
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
