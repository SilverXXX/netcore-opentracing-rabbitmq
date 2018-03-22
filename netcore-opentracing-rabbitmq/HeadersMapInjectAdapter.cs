using OpenTracing.Propagation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NetCore.OpenTracing.RabbitMQ
{
    class HeadersMapInjectAdapter : ITextMap
    {
        private readonly IDictionary<string, object> dict;
        public HeadersMapInjectAdapter(IDictionary<string, object> headers)
        {
            dict = headers;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            throw new InvalidOperationException("HeadersMapInjectAdapter should only be used with Tracer.inject()");
        }

        public void Set(string key, string value)
        {
            dict[key] = Encoding.UTF8.GetBytes(value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new InvalidOperationException("HeadersMapInjectAdapter should only be used with Tracer.inject()");
        }
    }
}
