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
        public static bool IsTerminal(this OrderStatus status)
        {
            return status == OrderStatus.Fulfilled
                || status == OrderStatus.Cancelled
                || status == OrderStatus.Rejected;
        }

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
    }
}
