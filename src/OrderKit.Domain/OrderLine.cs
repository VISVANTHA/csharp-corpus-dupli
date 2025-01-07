using System;

namespace OrderKit
{
    /// <summary>A single priced line on an order. Immutable by construction.</summary>
    public sealed class OrderLine
    {
        private readonly string _sku;
        private readonly int _quantity;
        private readonly long _unitCents;
        private readonly string _category;

        public OrderLine(string sku, int quantity, long unitCents, string category)
        {
            if (sku == null) throw new ArgumentNullException("sku");
            if (quantity < 0) throw new ArgumentOutOfRangeException("quantity");
            if (unitCents < 0) throw new ArgumentOutOfRangeException("unitCents");
            _sku = sku;
            _quantity = quantity;
            _unitCents = unitCents;
            _category = category == null ? "general" : category;
        }

        public string Sku { get { return _sku; } }
        public int Quantity { get { return _quantity; } }
        public long UnitCents { get { return _unitCents; } }
        public string Category { get { return _category; } }

        public long ExtendedCents()
        {
            return _unitCents * _quantity;
        }

        public override string ToString()
        {
            return string.Format("{0} x{1} @ {2}", _sku, _quantity, _unitCents);
        }
    }
}
