using System;
using System.Collections.Generic;
using System.Text;
using OpenTracing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NetCore.OpenTracing.RabbitMQ
{
    public class TracingConsumer : IBasicConsumer
    {
        private readonly IBasicConsumer consumer;
        private readonly ITracer tracer;

        public TracingConsumer(IBasicConsumer consumer, ITracer tracer)
        {
            this.consumer = consumer;
            this.tracer = tracer;
        }
        public IModel Model => consumer.Model;

        public event EventHandler<ConsumerEventArgs> ConsumerCancelled { add => consumer.ConsumerCancelled += value; remove => consumer.ConsumerCancelled -= value; }

        public void HandleBasicCancel(string consumerTag)
        {
            consumer.HandleBasicCancel(consumerTag);
        }

        public void HandleBasicCancelOk(string consumerTag)
        {
            consumer.HandleBasicCancelOk(consumerTag);
        }

        public void HandleBasicConsumeOk(string consumerTag)
        {
            consumer.HandleBasicConsumeOk(consumerTag);
        }

        public void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
        {
            using(IScope child = TracingUtils.BuildChildSpan(properties, tracer))
            {
                consumer.HandleBasicDeliver(consumerTag, deliveryTag, redelivered, exchange, routingKey, properties, body);
            }
        }

        public void HandleModelShutdown(object model, ShutdownEventArgs reason)
        {
            consumer.HandleModelShutdown(model, reason);
        }
    }
}
