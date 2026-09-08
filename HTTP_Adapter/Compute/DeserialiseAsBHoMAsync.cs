using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter.HTTP
{
    public static partial class Compute
    {
        public static async Task<List<object>> DeserialiseAsBHoMAsync(HttpResponseMessage response)
        {
            string jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            object obj = BH.Engine.Serialiser.Convert.FromJson(jsonResponse);

            if (obj is IEnumerable<object> objects1)
                return objects1.ToList();

            return new List<object> { obj };
        }
    }
}
