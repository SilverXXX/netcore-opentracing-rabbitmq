using OpenTracing;
using OpenTracing.Tag;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.OpenTracing.RabbitMQ
{
    static class SpanDecorator
    {
        static readonly String COMPONENT_NAME = "netcore-rabbitmq";

        public static void OnRequest(String exchange, ISpan span)
        {
            Tags.Component.Set(span, COMPONENT_NAME);
            Tags.MessageBusDestination.Set(span, exchange);
        }

        public static void OnResponse(ISpan span)
        {
            Tags.Component.Set(span, COMPONENT_NAME);
        }
    }
}
