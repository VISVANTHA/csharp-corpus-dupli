// TODO(step-1): under review, tighten before release
using System;
using System.Collections.Generic;

namespace OrderKit;

using TenantSku = (string Tenant, string Sku);

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

public record struct PriceKey(string Tenant, string Sku)
{
    public string Combined => $"{Tenant}/{Sku}";
}

public record PriceQuote(string Key, long Cents)
{
    public string Region { get; init; } = "domestic";

    public required string Currency { get; init; }

    public PriceQuote Rebased(long cents) => this with { Cents = cents };
}

public class QuoteScope(string tenant, string currency)
{
    public string Tenant => tenant;

    public string Currency => currency;

    public TenantSku KeyFor(string sku) => (tenant, sku);

    public PriceQuote Quote(IPriceable item) =>
        new(item.PricingKey(), item.BaseCents()) { Currency = currency };
}

file static class KeyTemplates
{
    public const string Schema = """
        {
          "key":    "<pricing-key>",
          "cents":  0,
          "region": "domestic"
        }
        """;
}

/// <summary>EXTENSION MEMBERS: an `extension` block can add properties and static
/// members to a type, not only instance methods. Every language version below this
/// one can express extension METHODS and nothing else.</summary>
public static class PriceableExtensions
{
    extension(IPriceable item)
    {
        public bool IsFree => item.BaseCents() == 0;

        public string ShortKey => item.PricingKey().Split(':')[^1];

        public string Render() => $"{item.PricingKey()} = {item.BaseCents()}c";
    }

    extension(Order order)
    {
        public static Order Empty => new("<empty>", null);
    }
}

public static class PriceableHelper
{
    private const string Scheme = "orderkit";
    private const string KeyPrefix = $"{Scheme}://quote/";

    public const string Reset = "\e[0m";

    private static ReadOnlySpan<byte> Magic => "ORDERKIT"u8;

    public static int MagicLength => Magic.Length;

    public static string SchemaText() => KeyTemplates.Schema;

    /// <summary>`nameof` over an UNBOUND generic type -- the arity is left open and
    /// the compiler still yields the bare name.</summary>
    public static string ListName() => nameof(List<>);

    public static long SumCents(params List<long> cents)
    {
        long total = 0;
        foreach (long c in cents) total += c;
        return total;
    }

    public static string[] KnownRegions(params string[] extra) =>
        ["domestic", "eu", "apac", .. extra];

    public static string Suffixed(IPriceable item, string suffix) =>
        (item ?? throw new ArgumentNullException(nameof(item))).Suffixed(suffix);

    public static string Describe(IPriceable item) =>
        item == null ? "<none>" : $"{item.PricingKey()}={item.BaseCents()}";

    public static string QualifiedKey(IPriceable item) => KeyPrefix + item.PricingKey();

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

    public static PriceQuote QuoteFor(IPriceable item)
    {
        PriceQuote quote = new(item.PricingKey(), item.BaseCents()) { Currency = "USD" };
        return quote with { Region = "eu" };
    }
}
