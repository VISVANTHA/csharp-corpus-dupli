using System;
using System.Collections.Generic;

namespace OrderKit
{
    public interface IPriceable
    {
        long BaseCents();
        string PricingKey();
    }

    /// <summary>A readonly struct: passed by reference without defensive copies.</summary>
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

        /// <summary>A type pattern. NOT a version lock -- verified.</summary>
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

        /// <summary>An `Enum` generic constraint -- the compiler can express
        /// "any enum" only from this family's language version.</summary>
        public static string NameOfStatus<T>(T value) where T : Enum => value.ToString();

        /// <summary>Tuple equality: `==` over tuples compares element-wise, and is
        /// rejected one language version below.</summary>
        public static bool SameShape(OrderLine a, OrderLine b)
        {
            if (a == null || b == null) return a == b;
            return (a.Sku, a.Quantity) == (b.Sku, b.Quantity);
        }
    }
}
