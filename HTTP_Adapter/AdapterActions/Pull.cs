/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using BH.Engine.Adapters.HTTP;
using BH.oM.Adapter;
using BH.oM.Base;
using BH.oM.Data.Requests;
using BH.oM.Adapters.HTTP;

namespace BH.Adapter.HTTP
{
    public partial class HTTPAdapter
    {
        /***************************************************/
        /**** Interface Methods                         ****/
        /***************************************************/

        public override IEnumerable<object> Pull(IRequest request,
            PullType pullType = PullType.AdapterDefault,
            ActionConfig actionConfig = null)
        {
            return Pull(request as dynamic, actionConfig);
        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public IEnumerable<object> Pull(GetRequest request, ActionConfig actionConfig)
        {
            string response = Compute.MakeRequest(request);

            if (response == null)
                return new List<BHoMObject>();

            // check if the response is a valid json
            if (response.StartsWith("{") || response.StartsWith("["))
                return new List<object>() { Engine.Serialiser.Convert.FromJson(response) };

            else
                return new List<object>() { response };
        }

        /***************************************************/

        public IEnumerable<object> Pull(BatchRequest requests, HTTPConfig config)
        {
            if (config == null)
                config = new HTTPConfig();

            string[] response = new string[requests.Requests.Count];
            List<BHoMObject> result = new List<BHoMObject>();
            using (HttpClient client = new HttpClient() { Timeout = config.Timeout })
            {
                List <GetRequest> getRequests = requests.Requests.OfType<GetRequest>().ToList();
                response = Task.WhenAll(getRequests.Select(x => Compute.MakeRequestAsync(x, client))).GetAwaiter().GetResult();
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
        /**** Fallback Case                             ****/
        /***************************************************/

        public IEnumerable<object> Pull(object request, ActionConfig actionConfig)
        {
            Engine.Reflection.Compute.RecordError($"Unknown request type {request.GetType()}.\n" +
                "If you are making a GET request, please use the BH.oM.Adapters.HTTP.GetRequest object to specify the request.");
            return null;
        }

        /***************************************************/
    }
}

