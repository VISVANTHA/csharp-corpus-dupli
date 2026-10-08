using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using NUnit.Framework;
using OrderKit;

namespace OrderKit.Tests
{
    [TestFixture]
    public sealed class TelemetryExportTests
    {
        private static readonly List<SpanRecord> Captured = new List<SpanRecord>();
        private static ActivityListener _listener;
        private static readonly ActivitySource Source = new ActivitySource("OrderKit.Tests");

        private sealed class SpanRecord
        {
            public string Name { get; set; }
            public long DurationTicks { get; set; }
            public string TraceId { get; set; }
        }

        [OneTimeSetUp]
        public void StartListener()
        {
            _listener = new ActivityListener
            {
                ShouldListenTo = _ => true,
                Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
                ActivityStopped = activity =>
                {
                    if (activity == null)
                    {
                        return;
                    }

                    Captured.Add(new SpanRecord
                    {
                        Name = activity.OperationName ?? activity.DisplayName ?? "unknown",
                        DurationTicks = activity.Duration.Ticks,
                        TraceId = activity.TraceId.ToString(),
                    });
                },
            };
            ActivitySource.AddActivityListener(_listener);
        }

        [OneTimeTearDown]
        public void ExportSpans()
        {
            var reportDir = Environment.GetEnvironmentVariable("OTEL_REPORT_DIR");
            if (string.IsNullOrWhiteSpace(reportDir))
            {
                return;
            }

            Directory.CreateDirectory(reportDir);
            var payload = new
            {
                tool = "opentelemetry",
                spans = Captured,
            };
            var path = Path.Combine(reportDir, "otel-spans.json");
            File.WriteAllText(path, JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true }));
            _listener?.Dispose();
            Source?.Dispose();
        }

        [Test]
        public void Pricing_path_records_runtime_spans()
        {
            using (var activity = Source.StartActivity("OrderKit.PricingService.PriceCents"))
            {
                var svc = new PricingService();
                var order = new Order("T-0", "standard");
                order.AddLine(new OrderLine("SKU-T", 1, 100L, "general"));
                Assert.That(svc.PriceCents(order, "domestic", false, 0), Is.GreaterThan(0));
            }

            using (Source.StartActivity("OrderKit.RiskScorer.Score"))
            {
                var scorer = new RiskScorer();
                var order = new Order("T-1", null);
                Assert.That(scorer.Score(order, "domestic", null), Is.GreaterThanOrEqualTo(0));
            }
        }
    }
}
