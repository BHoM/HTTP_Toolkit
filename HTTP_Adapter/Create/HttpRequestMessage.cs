using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace BH.Adapter.HTTP
{
    public static partial class Create
    {
        public static HttpRequestMessage ConstructHttpRequestMessage(HttpMethod method, Uri uri, Dictionary<string, object> headers = null)
        {
            var message = new HttpRequestMessage(method, uri);

            if (headers != null)
                foreach (var kvp in headers)
                    message.Headers.Add(kvp.Key, kvp.Value.ToString());

            return message;
        }
    }
}
