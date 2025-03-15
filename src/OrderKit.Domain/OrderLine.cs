using System;

namespace OrderKit
{
    /// <summary>A single priced line on an order. Immutable by construction.</summary>
    public sealed class OrderLine
    {
        /// <summary>Throw expressions: the guard is part of the assignment rather
        /// than a statement before it. A deconstructing expression-bodied
        /// constructor would have been the showier choice and was rejected --
        /// tuple syntax needs System.ValueTuple, which is in-box only from net47
        /// and is a NuGet package here, and NuGet cannot be restored at this host.
        /// A lock that depends on an unrestorable package is a lock that cannot be
        /// verified.</summary>
        public OrderLine(string sku, int quantity, long unitCents, string category)
        {
            Sku = sku ?? throw new ArgumentNullException(nameof(sku));
            Quantity = quantity >= 0
                ? quantity
                : throw new ArgumentOutOfRangeException(nameof(quantity));
            UnitCents = unitCents >= 0
                ? unitCents
                : throw new ArgumentOutOfRangeException(nameof(unitCents));
            Category = category ?? "general";
        }

        public string Sku { get; }
        public int Quantity { get; }
        public long UnitCents { get; }
        public string Category { get; }

        public long ExtendedCents() => UnitCents * Quantity;

        public override string ToString() => $"{Sku} x{Quantity} @ {UnitCents}";
    }
}
