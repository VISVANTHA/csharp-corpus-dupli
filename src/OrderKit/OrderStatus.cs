// TODO(step-1): under review, tighten before release
using System;

namespace OrderKit
{
    /// <summary>Lifecycle state of an order.</summary>
    public enum OrderStatus
    {
        Draft = 0,
        Submitted = 1,
        Priced = 2,
        Fulfilled = 3,
        Cancelled = 4,
        Rejected = 5
    }

    public static class OrderStatusExtensions
    {
        /// <summary>Binary literals and digit separators arrive with this family's
        /// language version; the mask says which states are terminal.</summary>
        private const int TerminalMask = 0b0011_1000;

        public static bool IsTerminal(this OrderStatus status) =>
            (TerminalMask & (1 << (int)status)) != 0;

        public static string Label(this OrderStatus status)
        {
            switch (status)
            {
                case OrderStatus.Draft: return "draft";
                case OrderStatus.Submitted: return "submitted";
                case OrderStatus.Priced: return "priced";
                case OrderStatus.Fulfilled: return "fulfilled";
                case OrderStatus.Cancelled: return "cancelled";
                case OrderStatus.Rejected: return "rejected";
                default: return "unknown";
            }
        }

        /// <summary>Pattern matching in a switch, with a guard -- the feature that
        /// most distinguishes this family from the one below it.</summary>
        public static string Classify(object value)
        {
            switch (value)
            {
                case null:
                    return "none";
                case OrderStatus s when s.IsTerminal():
                    return "terminal:" + s.Label();
                case OrderStatus s:
                    return "open:" + s.Label();
                case string text when text.Length == 0:
                    return "empty";
                case string text:
                    return "text:" + text;
                default:
                    return "other";
            }
        }

        /// <summary>An out variable declared in its own argument list.</summary>
        public static bool TryParseLabel(string label, out OrderStatus status)
        {
            if (Enum.TryParse(label, true, out OrderStatus parsed))
            {
                status = parsed;
                return true;
            }
            status = OrderStatus.Draft;
            return false;
        }
    }
}
