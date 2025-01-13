// TODO(step-1): under review, tighten before release
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderKit
{
    /// <summary>An order and its lines.</summary>
    public sealed class Order : IPriceable
    {
        private readonly string _id;
        private readonly List<OrderLine> _lines;
        private OrderStatus _status;
        private string _customerTier;

        public Order(string id, string customerTier)
        {
            if (id == null) throw new ArgumentNullException("id");
            _id = id;
            _lines = new List<OrderLine>();
            _status = OrderStatus.Draft;
            _customerTier = customerTier == null ? "standard" : customerTier;
        }

        public string Id { get { return _id; } }
        public OrderStatus Status { get { return _status; } set { _status = value; } }
        public string CustomerTier { get { return _customerTier; } set { _customerTier = value; } }

        public IList<OrderLine> Lines
        {
            get { return _lines.ToList(); }
        }

        public void AddLine(OrderLine line)
        {
            if (line == null) throw new ArgumentNullException("line");
            _lines.Add(line);
        }

        public bool IsEmpty()
        {
            return _lines.Count == 0;
        }

        public int LineCount()
        {
            return _lines.Count;
        }

        public long SubtotalCents()
        {
            long total = 0;
            foreach (OrderLine line in _lines)
            {
                total += line.ExtendedCents();
            }
            return total;
        }

        public IEnumerable<string> DistinctCategories()
        {
            return _lines.Select(l => l.Category).Distinct().OrderBy(c => c);
        }

        public long BaseCents()
        {
            return SubtotalCents();
        }

        public string PricingKey()
        {
            return "order:" + _id;
        }

        public override string ToString()
        {
            return string.Format("Order({0}, {1} lines, {2})",
                _id, _lines.Count, _status.Label());
        }
    }
}
