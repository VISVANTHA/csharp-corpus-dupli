// TODO(step-1): under review, tighten before release
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

    /// <summary>Shared helper for pricing keys. Not an interface default method --
    /// those are C# 8, still above this family's language version.</summary>
    public static class PriceableHelper
    {
        public static string Suffixed(IPriceable item, string suffix)
        {
            string key = (item ?? throw new ArgumentNullException(nameof(item))).PricingKey();
            return string.IsNullOrEmpty(suffix) ? key : $"{key}:{suffix}";
        }

        public static string Describe(IPriceable item) =>
            item == null ? "<none>" : $"{item.PricingKey()}={item.BaseCents()}";

        /// <summary>A type pattern: the test and the binding in one expression.</summary>
        public static long WeightOf(object candidate)
        {
            if (candidate is Order order)
            {
                return order.SubtotalCents();
            }
            if (candidate is OrderLine line)
            {
                return line.ExtendedCents();
            }
            return default;
        }

        /// <summary>Pattern matching against an open type parameter. NOT a lock:
        /// verified by probe that both this and the value-type form below compile
        /// at the language version beneath this one. Kept because it is reasonable
        /// code, but it is not counted as version differentiation, and an earlier
        /// draft wrongly claimed it was.</summary>
        public static T FirstOfType<T>(IEnumerable<object> candidates) where T : class
        {
            foreach (object candidate in candidates ?? new object[0])
            {
                if (candidate is T typed)
                {
                    return typed;
                }
            }
            return default;
        }

        /// <summary>The same shape over a value type. Also not a lock.</summary>
        public static bool TryFirst<T>(IEnumerable<object> candidates, out T found)
        {
            foreach (object candidate in candidates ?? new object[0])
            {
                if (candidate is T typed)
                {
                    found = typed;
                    return true;
                }
            }
            found = default;
            return false;
        }
    }
}
