using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace BH.Adapter.HTTP
{
    public static partial class Compute
    {
        public static void AddBHoMContent(ref HttpRequestMessage message, object value)
        {
            string json = BH.Engine.Serialiser.Convert.ToJson(value);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/bhom");
            message.Content = content;
        }
    }
}
