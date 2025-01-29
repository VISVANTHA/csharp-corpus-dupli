using System;

namespace OrderKit
{
    /// <summary>A single priced line on an order. Immutable by construction.</summary>
    public sealed class OrderLine
    {
        public OrderLine(string sku, int quantity, long unitCents, string category)
        {
            if (sku == null) throw new ArgumentNullException(nameof(sku));
            if (quantity < 0) throw new ArgumentOutOfRangeException(nameof(quantity));
            if (unitCents < 0) throw new ArgumentOutOfRangeException(nameof(unitCents));
            Sku = sku;
            Quantity = quantity;
            UnitCents = unitCents;
            Category = category ?? "general";
        }

        public string Sku { get; private set; }
        public int Quantity { get; private set; }
        public long UnitCents { get; private set; }
        public string Category { get; private set; }

        public long ExtendedCents() => UnitCents * Quantity;

        public override string ToString() => $"{Sku} x{Quantity} @ {UnitCents}";
    }
}
