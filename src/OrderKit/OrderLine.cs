// TODO(step-1): under review, tighten before release
using System;

namespace OrderKit
{
    /// <summary>A single priced line on an order. Immutable by construction.</summary>
    public sealed class OrderLine
    {
        /// <summary>Throw expressions: the guard is part of the assignment rather
        /// than a statement before it.</summary>
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

        /// <summary>Deconstruction, and a `default` literal whose type is inferred
        /// from the target rather than restated.</summary>
        public void Deconstruct(out string sku, out int quantity, out long unitCents)
        {
            sku = Sku;
            quantity = Quantity;
            unitCents = UnitCents;
        }

        /// <summary>Inferred tuple element names. The lock is the ACCESS -- reading
        /// `identity.Sku` when nothing named that element -- not the literal.
        /// Converting a tuple literal positionally into a named return type
        /// compiles one language version below; verified by probe, and an earlier
        /// draft of this file made exactly that mistake and claimed a lock it did
        /// not have.</summary>
        public string IdentityKey()
        {
            var identity = (Sku, Quantity);
            return $"{identity.Sku}#{identity.Quantity}";
        }

        public static OrderLine Empty() => new OrderLine("", default, default, null);

        public override string ToString() => $"{Sku} x{Quantity} @ {UnitCents}";
    }
}
