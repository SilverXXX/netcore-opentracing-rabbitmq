using OpenTracing;
using OpenTracing.Propagation;
using OpenTracing.Tag;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.OpenTracing.RabbitMQ
{
    static class TracingUtils
    {
        public static ISpanContext Extract(IBasicProperties props, ITracer tracer)
        {
            var spanContext = tracer
                .Extract(BuiltinFormats.TextMap, new HeadersMapExtractAdapter(props.Headers));
            if (spanContext != null)
            {
                return spanContext;
            }

            var span = tracer.ActiveSpan;
            if (span != null)
            {
                return span.Context;
            }
            return null;
        }

        public static void BuildAndFinishChildSpan(IBasicProperties props, ITracer tracer)
        {
            var child = BuildChildSpan(props, tracer);
            if (child != null)
            {
                child.Dispose();
            }
        }

        public static IScope BuildChildSpan(IBasicProperties props, ITracer tracer)
        {
            ISpanContext context = TracingUtils.Extract(props, tracer);
            if (context != null)
            {
                var spanBuilder = tracer.BuildSpan("receive")
                    .IgnoreActiveSpan()
                    .AddReference(References.FollowsFrom, context)
                    .WithTag(Tags.SpanKind.Key, Tags.SpanKindConsumer);

                var scope = spanBuilder.StartActive(true);
                SpanDecorator.OnResponse(scope.Span);
                return scope;
            }

            return null;
        }
    }
}
