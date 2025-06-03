using System;

namespace OrderKit
{
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
        private const int TerminalMask = 0b0011_1000;

        public static bool IsTerminal(this OrderStatus status) =>
            (TerminalMask & (1 << (int)status)) != 0;

        /// <summary>A switch EXPRESSION: arms yield values, no `case`/`break`, and
        /// the compiler checks exhaustiveness. Rejected one language version below,
        /// where only the switch statement existed.</summary>
        public static string Label(this OrderStatus status) => status switch
        {
            OrderStatus.Draft => "draft",
            OrderStatus.Submitted => "submitted",
            OrderStatus.Priced => "priced",
            OrderStatus.Fulfilled => "fulfilled",
            OrderStatus.Cancelled => "cancelled",
            OrderStatus.Rejected => "rejected",
            _ => "unknown"
        };

        /// <summary>Property patterns inside a switch expression.</summary>
        public static string Classify(object value) => value switch
        {
            null => "none",
            OrderStatus s when s.IsTerminal() => "terminal:" + s.Label(),
            OrderStatus s => "open:" + s.Label(),
            string { Length: 0 } => "empty",
            string text => "text:" + text,
            _ => "other"
        };

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
