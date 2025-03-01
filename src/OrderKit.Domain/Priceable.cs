using System;

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
            if (item == null) throw new ArgumentNullException(nameof(item));
            string key = item.PricingKey();
            return string.IsNullOrEmpty(suffix) ? key : $"{key}:{suffix}";
        }

        public static string Describe(IPriceable item) =>
            item == null ? "<none>" : $"{item.PricingKey()}={item.BaseCents()}";
    }
}
