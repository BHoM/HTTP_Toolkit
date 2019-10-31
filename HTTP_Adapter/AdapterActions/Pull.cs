/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BH.Engine.HTTP;
using BH.oM.Base;
using BH.oM.Data.Requests;
using BH.oM.HTTP;

namespace BH.Adapter.HTTP
{
    public partial class HTTPAdapter
    {
        /***************************************************/
        /**** Interface Methods                         ****/
        /***************************************************/

        public override IEnumerable<object> Pull(IRequest request, Dictionary<string, object> config = null)
        {
            return Pull(request as dynamic, config);
        }

        /***************************************************/
        /**** Fallback Case                             ****/
        /***************************************************/

        public IEnumerable<object> Pull(object request, Dictionary<string, object> config = null)
        {
            Engine.Reflection.Compute.RecordError($"Unknown request type {request.GetType()}.\n" +
                "If you are making a GET request, please use the BH.oM.HTTP.GetRequest object to specify the request.");
            return null;
        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public IEnumerable<object> Pull(GetRequest request, Dictionary<string, object> config = null)
        {
            string response = Engine.HTTP.Compute.MakeRequest(request);

            if (response == null)
                return new List<BHoMObject>();

            BHoMObject obj = Engine.Serialiser.Convert.FromJson(response) as BHoMObject;
            if (obj == null)
            {
                // in case the response is not a valid json, wrap it around a CustomObject
                return new List<BHoMObject>
                {
                    new CustomObject()
                    {
                        CustomData = new Dictionary<string, object>() { { "Response", response } }
                    }
                };
            }

            return new List<BHoMObject> { obj }; // This is at least a CustomObject
        }

        /***************************************************/

        public IEnumerable<object> Pull(BatchRequest requests, Dictionary<string, object> config = null)
        {
            HTTPAdapterConfig adapterConfig = config == null ? new HTTPAdapterConfig() : (HTTPAdapterConfig)config;

            string[] response = new string[requests.Requests.Count];
            List<BHoMObject> result = new List<BHoMObject>();
            using (HttpClient client = new HttpClient() { Timeout = adapterConfig.Timeout })
            {
                List<string> urls = requests.Requests.OfType<GetRequest>().Select(req => req.ToUrlString()).ToList();
                response = Task.WhenAll(urls.Select(x => Compute.GetRequestAsync(x, client))).GetAwaiter().GetResult();
                client.CancelPendingRequests();
                client.Dispose();
            }

            Parallel.ForEach(response, res =>
            {
                if (res == null)
                    return;
                BHoMObject obj = Engine.Serialiser.Convert.FromJson(res) as BHoMObject;
                if (obj == null)
                {
                    Engine.Reflection.Compute.RecordNote($"{res.GetType()} failed to deserialise to a BHoMObject and is set to null." +
                        $"Perform a request with Compute.GetRequest(string url) if you want the raw output");
                    return; // return is equivalent to `continue` in a Parallel.ForEach
                }
                result.Add(obj);
            });
            return result;
        }

        /***************************************************/
    }
}
