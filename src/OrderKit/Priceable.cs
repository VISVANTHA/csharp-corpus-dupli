// TODO(step-1): under review, tighten before release
using System;
using System.Collections.Generic;

namespace OrderKit
{
    /// <summary>Anything the pricing engine can put a number on.
    /// The helper is now a DEFAULT INTERFACE METHOD -- every earlier family in this
    /// corpus carried a comment saying that was still above its language version.
    /// This is the family where it stops being true, and it needs runtime support,
    /// not just a compiler.</summary>
    public interface IPriceable
    {
        long BaseCents();
        string PricingKey();

        string Suffixed(string suffix) =>
            string.IsNullOrEmpty(suffix) ? PricingKey() : $"{PricingKey()}:{suffix}";
    }

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
        public static string Suffixed(IPriceable item, string suffix) =>
            (item ?? throw new ArgumentNullException(nameof(item))).Suffixed(suffix);

        public static string Describe(IPriceable item) =>
            item == null ? "<none>" : $"{item.PricingKey()}={item.BaseCents()}";

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

        public static long TotalOf(in Cents a, in Cents b) => a.Value + b.Value;

        public static string NameOfStatus<T>(T value) where T : Enum => value.ToString();

        public static bool SameShape(OrderLine a, OrderLine b)
        {
            if (a == null || b == null) return a == b;
            return (a.Sku, a.Quantity) == (b.Sku, b.Quantity);
        }
    }
}
