using OpenTracing;
using OpenTracing.Tag;
using OpenTracing.Propagation;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;

namespace NetCore.OpenTracing.RabbitMQ
{
    public class TracingModel : IModel
    {
        private readonly IModel model;
        private readonly ITracer tracer;
        public TracingModel(IModel model, ITracer tracer)
        {
            this.model = model;
            this.tracer = tracer;
        }

        public int ChannelNumber => model.ChannelNumber;

        public ShutdownEventArgs CloseReason => model.CloseReason;

        public IBasicConsumer DefaultConsumer { get => model.DefaultConsumer; set => model.DefaultConsumer = value; }

        public bool IsClosed => model.IsClosed;

        public bool IsOpen => model.IsOpen;

        public ulong NextPublishSeqNo => model.NextPublishSeqNo;

        public TimeSpan ContinuationTimeout { get => model.ContinuationTimeout; set => model.ContinuationTimeout = value; }

        public event EventHandler<BasicAckEventArgs> BasicAcks { add => model.BasicAcks += value; remove => model.BasicAcks -= value; }
        public event EventHandler<BasicNackEventArgs> BasicNacks { add => model.BasicNacks += value; remove => model.BasicNacks -= value; }
        public event EventHandler<EventArgs> BasicRecoverOk { add => model.BasicRecoverOk += value; remove => model.BasicRecoverOk -= value; }
        public event EventHandler<BasicReturnEventArgs> BasicReturn { add => model.BasicReturn += value; remove => model.BasicReturn -= value; }
        public event EventHandler<CallbackExceptionEventArgs> CallbackException { add => model.CallbackException += value; remove => model.CallbackException -= value; }
        public event EventHandler<FlowControlEventArgs> FlowControl { add => model.FlowControl += value; remove => model.FlowControl -= value; }
        public event EventHandler<ShutdownEventArgs> ModelShutdown { add => model.ModelShutdown += value; remove => model.ModelShutdown -= value; }

        public void Abort()
        {
            model.Abort();
        }

        public void Abort(ushort replyCode, string replyText)
        {
            model.Abort(replyCode, replyText);
        }

        public void BasicAck(ulong deliveryTag, bool multiple)
        {
            model.BasicAck(deliveryTag, multiple);
        }

        public void BasicCancel(string consumerTag)
        {
            model.BasicCancel(consumerTag);
        }

        public string BasicConsume(string queue, bool autoAck, string consumerTag, bool noLocal, bool exclusive, IDictionary<string, object> arguments, IBasicConsumer consumer)
        {
            return model.BasicConsume(queue, autoAck, consumerTag, noLocal, exclusive, arguments, consumer);
        }

        public BasicGetResult BasicGet(string queue, bool autoAck)
        {
            var result = model.BasicGet(queue, autoAck);
            TracingUtils.BuildAndFinishChildSpan(result.BasicProperties, tracer);
            return result;
        }

        public void BasicNack(ulong deliveryTag, bool multiple, bool requeue)
        {
            throw new NotImplementedException();
        }

        public void BasicPublish(string exchange, string routingKey, bool mandatory, IBasicProperties basicProperties, byte[] body)
        {
            using (IScope scope = buildSpan(exchange, basicProperties)) {
                var properties = inject(basicProperties, scope.Span);
                model.BasicPublish(exchange, routingKey, mandatory, properties, body);
            }
        }

        public void BasicQos(uint prefetchSize, ushort prefetchCount, bool global)
        {
            model.BasicQos(prefetchSize, prefetchCount, global);
        }

        public void BasicRecover(bool requeue)
        {
            model.BasicRecover(requeue);
        }

        public void BasicRecoverAsync(bool requeue)
        {
            model.BasicRecoverAsync(requeue);
        }

        public void BasicReject(ulong deliveryTag, bool requeue)
        {
            model.BasicReject(deliveryTag, requeue);
        }

        public void Close()
        {
            model.Close();
        }

        public void Close(ushort replyCode, string replyText)
        {
            model.Close(replyCode, replyText);
        }

        public void ConfirmSelect()
        {
            model.ConfirmSelect();
        }

        public uint ConsumerCount(string queue)
        {
            return model.ConsumerCount(queue);
        }

        public IBasicProperties CreateBasicProperties()
        {
            return model.CreateBasicProperties();
        }

        public void ExchangeBind(string destination, string source, string routingKey, IDictionary<string, object> arguments)
        {
            model.ExchangeBind(destination, source, routingKey, arguments);
        }

        public void ExchangeBindNoWait(string destination, string source, string routingKey, IDictionary<string, object> arguments)
        {
            model.ExchangeBindNoWait(destination, source, routingKey, arguments);
        }

        public void ExchangeDeclare(string exchange, string type, bool durable, bool autoDelete, IDictionary<string, object> arguments)
        {
            model.ExchangeDeclare(exchange, type, durable, autoDelete, arguments);
        }

        public void ExchangeDeclareNoWait(string exchange, string type, bool durable, bool autoDelete, IDictionary<string, object> arguments)
        {
            model.ExchangeDeclareNoWait(exchange, type, durable, autoDelete, arguments);
        }

        public void ExchangeDeclarePassive(string exchange)
        {
            model.ExchangeDeclarePassive(exchange);
        }

        public void ExchangeDelete(string exchange, bool ifUnused)
        {
            model.ExchangeDelete(exchange, ifUnused);
        }

        public void ExchangeDeleteNoWait(string exchange, bool ifUnused)
        {
            model.ExchangeDeleteNoWait(exchange, ifUnused);
        }

        public void ExchangeUnbind(string destination, string source, string routingKey, IDictionary<string, object> arguments)
        {
            model.ExchangeUnbind(destination, source, routingKey, arguments);
        }

        public void ExchangeUnbindNoWait(string destination, string source, string routingKey, IDictionary<string, object> arguments)
        {
            model.ExchangeUnbindNoWait(destination, source, routingKey, arguments);
        }

        public uint MessageCount(string queue)
        {
            return model.MessageCount(queue);
        }

        public void QueueBind(string queue, string exchange, string routingKey, IDictionary<string, object> arguments)
        {
            model.QueueBind(queue, exchange, routingKey, arguments);
        }

        public void QueueBindNoWait(string queue, string exchange, string routingKey, IDictionary<string, object> arguments)
        {
            model.QueueBindNoWait(queue, exchange, routingKey, arguments);
        }

        public QueueDeclareOk QueueDeclare(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object> arguments)
        {
            return model.QueueDeclare(queue, durable, exclusive, autoDelete, arguments);
        }

        public void QueueDeclareNoWait(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object> arguments)
        {
            model.QueueDeclareNoWait(queue, durable, exclusive, autoDelete, arguments);
        }

        public QueueDeclareOk QueueDeclarePassive(string queue)
        {
            return model.QueueDeclarePassive(queue);
        }

        public uint QueueDelete(string queue, bool ifUnused, bool ifEmpty)
        {
            return model.QueueDelete(queue, ifUnused, ifEmpty);
        }

        public void QueueDeleteNoWait(string queue, bool ifUnused, bool ifEmpty)
        {
            model.QueueDeleteNoWait(queue, ifUnused, ifEmpty);
        }

        public uint QueuePurge(string queue)
        {
            return model.QueuePurge(queue);
        }

        public void QueueUnbind(string queue, string exchange, string routingKey, IDictionary<string, object> arguments)
        {
            model.QueueUnbind(queue, exchange, routingKey, arguments);
        }

        public void TxCommit()
        {
            model.TxCommit();
        }

        public void TxRollback()
        {
            model.TxRollback();
        }

        public void TxSelect()
        {
            model.TxSelect();
        }

        public bool WaitForConfirms()
        {
            return model.WaitForConfirms();
        }

        public bool WaitForConfirms(TimeSpan timeout)
        {
            return model.WaitForConfirms(timeout);
        }

        public bool WaitForConfirms(TimeSpan timeout, out bool timedOut)
        {
            return model.WaitForConfirms(timeout, out timedOut);
        }

        public void WaitForConfirmsOrDie()
        {
            model.WaitForConfirmsOrDie();
        }

        public void WaitForConfirmsOrDie(TimeSpan timeout)
        {
            model.WaitForConfirmsOrDie(timeout);
        }

        #region IDisposable Support
        private bool disposedValue = false; // Per rilevare chiamate ridondanti

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: eliminare lo stato gestito (oggetti gestiti).
                    model.Dispose();
                }

                // TODO: liberare risorse non gestite (oggetti non gestiti) ed eseguire sotto l'override di un finalizzatore.
                // TODO: impostare campi di grandi dimensioni su Null.

                disposedValue = true;
            }
        }

        // TODO: eseguire l'override di un finalizzatore solo se Dispose(bool disposing) include il codice per liberare risorse non gestite.
        // ~TracingModel() {
        //   // Non modificare questo codice. Inserire il codice di pulizia in Dispose(bool disposing) sopra.
        //   Dispose(false);
        // }

        // Questo codice viene aggiunto per implementare in modo corretto il criterio Disposable.
        public void Dispose()
        {
            // Non modificare questo codice. Inserire il codice di pulizia in Dispose(bool disposing) sopra.
            Dispose(true);
            // TODO: rimuovere il commento dalla riga seguente se è stato eseguito l'override del finalizzatore.
            // GC.SuppressFinalize(this);
        }
        #endregion

        private IScope buildSpan(String exchange, IBasicProperties props)
        {
            var spanBuilder = tracer.BuildSpan("send")
                .IgnoreActiveSpan()
                .WithTag(Tags.SpanKind.Key, Tags.SpanKindProducer);

            ISpanContext spanContext = null;

            if (props != null && props.Headers != null)
            {
                // just in case if span context was injected manually to props in basicPublish
                spanContext = tracer.Extract(BuiltinFormats.TextMap,
                     new HeadersMapExtractAdapter(props.Headers));
            }

            if (spanContext == null)
            {
                var parentSpan = tracer.ActiveSpan;
                if (parentSpan != null)
                {
                    spanContext = parentSpan.Context;
                }
            }

            if (spanContext != null)
            {
                spanBuilder.AsChildOf(spanContext);
            }

            IScope scope = spanBuilder.StartActive(true);
            SpanDecorator.OnRequest(exchange, scope.Span);

            return scope;
        }

        private IBasicProperties inject(IBasicProperties properties, ISpan span)
        {

            // Headers of AMQP.BasicProperties is unmodifiableMap therefore we build new AMQP.BasicProperties
            // with injected span context into headers
            IDictionary<string, object> headers = new Dictionary<string, object>();

            tracer.Inject(span.Context, BuiltinFormats.TextMap, new HeadersMapInjectAdapter(headers));
            
            if (properties == null)
            {
                properties = model.CreateBasicProperties();
            }

            foreach (var kvp in headers)
            {
                if (properties.Headers == null)
                    properties.Headers = new Dictionary<string, object>();

                properties.Headers.Add(kvp.Key, kvp.Value);
            }

            return properties;
        }
    }
}
