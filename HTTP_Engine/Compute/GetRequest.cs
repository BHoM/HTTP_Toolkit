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

using BH.oM.Base;
using System.Collections.Generic;
using System.Net.Http;

namespace BH.Engine.HTTP
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public  Method                            ****/
        /***************************************************/

        public static string GetRequest(string uri)
        {
            using (HttpResponseMessage response = new HttpClient().GetAsync(uri).Result)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                if (!response.IsSuccessStatusCode)
                {
                    Engine.Reflection.Compute.ClearCurrentEvents();
                    Engine.Reflection.Compute.RecordError($"GET request failed with code {response.StatusCode}: {response.ReasonPhrase}");
                    return null;
                }
                else
                {
                    return result;
                }
            }
        }

        /***************************************************/

        public static string GetRequest(string uri, Dictionary<string, object> headers = null)
        {
            HttpClient client = new HttpClient();

            //Add headers
            foreach (var kvp in headers)
            {
                client.DefaultRequestHeaders.Add(kvp.Key, kvp.Value.ToString());
            }

            //Post login auth request and return token to m_bearerKey
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);


            using (HttpResponseMessage response = client.SendAsync(request).Result)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                if (!response.IsSuccessStatusCode)
                {
                    Engine.Reflection.Compute.ClearCurrentEvents();
                    Engine.Reflection.Compute.RecordError($"GET request failed with code {response.StatusCode}: {response.ReasonPhrase}");
                    return null;
                }
                else
                {
                    return result;
                }
            }
        }

        /***************************************************/

        public static string GetRequest(string url, Dictionary<string, object> headers = null, Dictionary<string, object> parameters = null)
        {
            string uri = Convert.ToUrlString(url, parameters);
            return GetRequest(uri, headers);
        }

        /***************************************************/

        public static string GetRequest(string baseUrl, CustomObject headers = null, CustomObject parameters = null)
        {
            return GetRequest(baseUrl, headers.CustomData, parameters.CustomData);
        }

        /***************************************************/
    }
}

