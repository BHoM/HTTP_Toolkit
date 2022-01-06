/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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
using BH.oM.Adapters.HTTP;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BH.Engine.Adapters.HTTP
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public  Method                            ****/
        /***************************************************/

        public static string ToUrlString(this GetRequest request)
        {           
            if (request.Parameters != null)
            {
                UriBuilder builder = new UriBuilder(request.BaseUrl);
                builder.Query = Convert.ToUrlString(request.Parameters);
                return builder.Uri.AbsoluteUri;
            }
            else
            {
                return request.BaseUrl;
            }
        }

        /***************************************************/

        public static string ToUrlString(string baseUrl, Dictionary<string, object> parameters = null)
        {
            if (parameters != null)
            {
                UriBuilder builder = new UriBuilder(baseUrl)
                {
                    Query = parameters == null ? "" : Convert.ToUrlString(parameters)
                };
                return builder.Uri.AbsoluteUri;
            }
            else
            {
                return baseUrl;
            }          
        }

        /***************************************************/

        public static string ToUrlString(Dictionary<string, object> data)
        {
            List<string> url = new List<string>();
            foreach (KeyValuePair<string, object> pair in data)
            {
                if (typeof(IEnumerable).IsAssignableFrom(pair.Value.GetType()) && !(pair.Value is string))
                {
                    foreach(object val in (IEnumerable)pair.Value)
                    {
                        url.Add($"{pair.Key}[]={val.ToString()}");
                    }
                }
                else {url.Add($"{pair.Key}={pair.Value.ToString()}");}
            }
            return string.Join("&", url);
        }

        /***************************************************/

        public static string ToUrlString(CustomObject obj)
        {
            return ToUrlString(obj.CustomData);
        }

        /***************************************************/
    }
}



