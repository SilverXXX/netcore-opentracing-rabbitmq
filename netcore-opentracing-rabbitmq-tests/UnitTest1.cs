using LetsTrace;
using LetsTrace.Reporters;
using LetsTrace.Jaeger.Transport;
using LetsTrace.Zipkin.Transport.ZipkinJSON;
using LetsTrace.Samplers;
using RabbitMQ.Client;
using System;
using Xunit;
using NetCore.OpenTracing.RabbitMQ;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace netcore_opentracing_rabbitmq_tests
{
    public class UnitTest1
    {
        [Fact]
        public void TestPublish()
        {
            var cf = new ConnectionFactory() { Uri = new Uri("amqp://guest:guest@localhost:5672") };

            var serviceName = "initExampleService";
            var transport = new JaegerHttpTransport("192.168.99.100");
            //var zt = new ZipkinJSONTransport(new Uri("http://192.168.99.100:9411"), 2);

            var reporter = new RemoteReporter.Builder(transport).Build();
            //var zreporter = new RemoteReporter.Builder(zt).Build();

            var sampler = new ConstSampler(true);

            var serviceProvider = new ServiceCollection()
                      .AddLogging() //<-- You were missing this
                      .BuildServiceProvider();
            //get logger
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            loggerFactory.AddConsole(LogLevel.Trace);
            loggerFactory.AddDebug(LogLevel.Trace);

            var log = loggerFactory.CreateLogger("test");
            log.LogInformation("prova log {0}", "prova");

            var logReporter = new LoggingReporter(loggerFactory);

            var tracer = new Tracer.Builder(serviceName)
                .WithLoggerFactory(loggerFactory)
                //.WithTransport(transport)
                .WithReporter(logReporter)
                .WithSampler(sampler)
                .Build();

            var conn = cf.CreateConnection("testtracing");
            var model = new TracingModel(conn.CreateModel(), tracer);
            // TODO: Add your test code here

            model.BasicPublish("testexchange", "", false, null, Encoding.UTF8.GetBytes("testmessage"));
            /*var consumer = new EventingBasicConsumer(model);
            consumer.Received += (ch, ea) =>
            {
                var body = ea.Body;
                // ... process the message
                model.BasicAck(ea.DeliveryTag, false);
            };

            Task.Delay(200).Wait();

            var res = model.BasicConsume("testqueueu", false, consumer);*/
            Task.Delay(200).Wait();
            var res = model.BasicGet("testqueueu", true);
            Assert.True(true, "Your first passing test");
        }


        [Fact]
        public void TestConsumer()
        {
            var cf = new ConnectionFactory() { Uri = new Uri("amqp://guest:guest@localhost:5672") };

            var serviceName = "initExampleService";
            var transport = new JaegerHttpTransport("192.168.99.100");
            //var zt = new ZipkinJSONTransport(new Uri("http://192.168.99.100:9411"), 2);

            var reporter = new RemoteReporter.Builder(transport).Build();
            //var zreporter = new RemoteReporter.Builder(zt).Build();

            var sampler = new ConstSampler(true);

            var serviceProvider = new ServiceCollection()
                      .AddLogging() //<-- You were missing this
                      .BuildServiceProvider();
            //get logger
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            loggerFactory.AddConsole(LogLevel.Trace);
            loggerFactory.AddDebug(LogLevel.Trace);

            var log = loggerFactory.CreateLogger("test");
            log.LogInformation("prova log {0}", "prova");

            var logReporter = new LoggingReporter(loggerFactory);

            var tracer = new Tracer.Builder(serviceName)
                .WithLoggerFactory(loggerFactory)
                //.WithTransport(transport)
                .WithReporter(logReporter)
                .WithSampler(sampler)
                .Build();

            var conn = cf.CreateConnection("testtracing");
            var model = new TracingModel(conn.CreateModel(), tracer);
            // TODO: Add your test code here

            model.BasicPublish("testexchange", "", false, null, Encoding.UTF8.GetBytes("testmessage"));
            var consumer = new EventingBasicConsumer(model);
            consumer.Received += (ch, ea) =>
            {
                var body = ea.Body;
                // ... process the message
                model.BasicAck(ea.DeliveryTag, false);
            };

            var trConsumer = new TracingConsumer(consumer, tracer);

            Task.Delay(200).Wait();

            var res = model.BasicConsume("testqueueu", false, trConsumer);

            
            Assert.True(true, "Your first passing test");
        }
    }
}
