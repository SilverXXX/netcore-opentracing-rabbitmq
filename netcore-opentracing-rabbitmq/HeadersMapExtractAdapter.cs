using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using OpenTracing.Propagation;

namespace NetCore.OpenTracing.RabbitMQ
{
    class HeadersMapExtractAdapter : ITextMap
    {
        private readonly IDictionary<string, string> dict = new Dictionary<string, string>();
        public HeadersMapExtractAdapter(IDictionary<string, object> headers)
        {
            foreach (var kvp in headers)
            {
                dict.Add(kvp.Key, Encoding.UTF8.GetString((byte[])kvp.Value));
            }
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return dict.GetEnumerator();
        }

        public void Set(string key, string value)
        {
            throw new InvalidOperationException("HeadersMapExtractAdapter should only be used with Tracer.extract()");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dict.GetEnumerator();
        }
    }
}
