using System;
using System.Collections.Generic;

namespace OrderKit
{
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

    /// <summary>A RECORD: value equality, a synthesised ToString, and a clone-with
    /// operator, all generated. Needs IsExternalInit, which this target framework
    /// ships in-box and the one below it did not.</summary>
    public record PriceQuote(string Key, long Cents)
    {
        /// <summary>An INIT-ONLY setter: assignable in an initialiser, immutable
        /// afterwards.</summary>
        public string Region { get; init; } = "domestic";

        public PriceQuote Rebased(long cents) => this with { Cents = cents };
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

        /// <summary>Target-typed `new`: the type is on the left, so it is not
        /// repeated on the right.</summary>
        public static PriceQuote QuoteFor(IPriceable item)
        {
            PriceQuote quote = new(item.PricingKey(), item.BaseCents());
            return quote with { Region = "eu" };
        }
    }
}
