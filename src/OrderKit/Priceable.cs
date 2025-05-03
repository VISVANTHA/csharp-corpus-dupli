using System;
using System.Collections.Generic;

namespace OrderKit
{
    /// <summary>Anything the pricing engine can put a number on.</summary>
    public interface IPriceable
    {
        long BaseCents();
        string PricingKey();
    }

    /// <summary>A readonly struct: the compiler may pass it by reference without
    /// defensive copies, because it promises not to mutate. The `readonly`
    /// modifier on a struct is this family's language version.</summary>
    public readonly struct Cents
    {
        public Cents(long value)
        {
            Value = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(value));
        }

        public long Value { get; }

        public Cents Plus(in Cents other) => new Cents(Value + other.Value);

        public override string ToString() => $"{Value}c";
    }

    public static class PriceableHelper
    {
        public static string Suffixed(IPriceable item, string suffix)
        {
            string key = (item ?? throw new ArgumentNullException(nameof(item))).PricingKey();
            return string.IsNullOrEmpty(suffix) ? key : $"{key}:{suffix}";
        }

        public static string Describe(IPriceable item) =>
            item == null ? "<none>" : $"{item.PricingKey()}={item.BaseCents()}";

        /// <summary>A type pattern. NOT a version lock -- verified by probe that this
        /// compiles below this family's language version. Kept as ordinary code.</summary>
        public static long WeightOf(object candidate)
        {
            if (candidate is Order order) return order.SubtotalCents();
            if (candidate is OrderLine line) return line.ExtendedCents();
            return default;
        }

        public static T FirstOfType<T>(IEnumerable<object> candidates) where T : class
        {
            foreach (object candidate in candidates ?? new object[0])
            {
                if (candidate is T typed) return typed;
            }
            return default;
        }

        /// <summary>`in` on a readonly struct: read-only reference, no copy.</summary>
        public static long TotalOf(in Cents a, in Cents b) => a.Value + b.Value;
    }
}
