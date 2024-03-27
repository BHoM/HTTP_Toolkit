/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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

using System;
using BH.oM.Base;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;

namespace BH.Engine.HTTP
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public  Method                            ****/
        /***************************************************/

        public static string PostRequest(string uri)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Dictionary<string, string> h3 = new Dictionary<string, string>();
            h3.Add("tag", "v0.0.1");
            h3.Add("message", "Initial test");
            h3.Add("object", "277e0873199301acfe7c306e5efd8041f8974dad");
            h3.Add("type", "commit");
            h3.Add("tagger", "\"name\": \"Fraser Greenroyd\", \"email\":\"fraser.greenroyd@burohappold.com\", \"date\":\"2019-09-26T10:39:40\"");

            var h1 = new FormUrlEncodedContent(h3);
            try
            {
                using (HttpResponseMessage response = new HttpClient().PostAsync(uri, h1).Result)
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    if (!response.IsSuccessStatusCode)
                    {
                        Engine.Reflection.Compute.ClearCurrentEvents();
                        Engine.Reflection.Compute.RecordError($"POST request failed with code {response.StatusCode}: {response.ReasonPhrase}");
                        return null;
                    }
                    else
                    {
                        return result;
                    }
                }
            }
            catch (Exception e)
            {
                Engine.Reflection.Compute.RecordError(e.ToString());
                return null;
            }
        }

        /***************************************************/

        public static string PostRequest(string url, Dictionary<string, object> headers = null, Dictionary<string, object> parameters = null)
        {
            string uri = Convert.ToUrlString(url, parameters);
            return PostRequest(uri);
        }

        /***************************************************/

        public static string PostRequest(string baseUrl, CustomObject headers = null, CustomObject parameters = null)
        {
            return PostRequest(baseUrl, headers.CustomData, parameters.CustomData);
        }
    }
}


