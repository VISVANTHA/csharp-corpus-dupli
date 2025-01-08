// TODO(step-1): under review, tighten before release
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
    /// those are C# 8, far above this family's language version.</summary>
    public static class PriceableHelper
    {
        public static string Suffixed(IPriceable item, string suffix)
        {
            if (item == null) throw new ArgumentNullException("item");
            string key = item.PricingKey();
            if (string.IsNullOrEmpty(suffix)) return key;
            return key + ":" + suffix;
        }
    }
}
