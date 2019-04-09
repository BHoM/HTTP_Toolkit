﻿/*
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

using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Net.Http;
using BH.oM.HTTP;

namespace BH.Engine.HTTP
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public  Method                            ****/
        /***************************************************/

        public static string GetRequest(string baseUrl, Dictionary<string, object> headers = null, Dictionary<string, object> parameters = null)
        {
            using (HttpClient client = new HttpClient() { BaseAddress = new Uri(baseUrl) })
            {
                if (headers != null)
                {
                    foreach (KeyValuePair<string, object> pair in headers)
                        client.DefaultRequestHeaders.Add(pair.Key, pair.Value.ToString());
                }

                UriBuilder builder = new UriBuilder(baseUrl);
                if (parameters != null)
                {
                    builder.Query = Convert.ToUrlString(parameters);
                    client.BaseAddress = builder.Uri;
                }

                HttpResponseMessage response = client.GetAsync(baseUrl).Result;
                Engine.Reflection.Compute.RecordNote($"Performing GET request on the following uri: {response.RequestMessage}");
                if (!response.IsSuccessStatusCode)
                {
                    Engine.Reflection.Compute.ClearCurrentEvents();
                    Engine.Reflection.Compute.RecordError($"GET request failed with code {response.StatusCode}: {response.ReasonPhrase}");
                    return null;
                }

                return response.Content.ReadAsStringAsync().Result;
            }
        }

        /***************************************************/

        public static string GetRequest(string baseUrl, CustomObject headers = null, CustomObject parameters = null)
        {
            return GetRequest(baseUrl, headers.CustomData, parameters.CustomData);
        }

        /***************************************************/

        public static string GetRequest(GetQuery query)
        {
            return GetRequest(query.BaseUrl, query.Headers, query.Parameters);
        }

        /***************************************************/
    }
}