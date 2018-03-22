using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netcore_opentracing_rabbitmq.Tests
{
    [TestFixture]
    public class TestClass
    {
        [Test]
        public void TestMethod()
        {
            var cf = new ConnectionFactory() { Uri = new Uri("amqp://guest:guest@localhost:5672") };            

            var serviceName = "initExampleService";
            //var reporter = new LoggingReporter(logger);
            var transport = new JaegerHttpTransport();
            var sampler = new ConstSampler(true);
            var tracer = new Tracer.Builder(serviceName)
                //.WithLoggerFactory(loggerFactory)
                .WithTransport(transport)
                //.WithReporter(reporter)
                .WithSampler(sampler)
                .Build();

            conn = cf.CreateConnection("testtracing");
            model = new TracingModel(conn.CreateModel(), tracer);
            // TODO: Add your test code here

            model.BasicPublish("testexchange", "", false, null, Encoding.UTF8.GetBytes("testmessage"));
            Task.Delay(200).Wait();
            var consumer = new EventingBasicConsumer(model);
            consumer.Received += (ch, ea) =>
            {
                var body = ea.Body;
                // ... process the message
                model.BasicAck(ea.DeliveryTag, false);
            };


            var res = model.BasicConsume("testqueueu", false, consumer);
            Assert.Pass("Your first passing test");
        }
    }
}
