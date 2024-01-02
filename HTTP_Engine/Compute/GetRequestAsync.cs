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

using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using BH.oM.Base;

namespace BH.Engine.Adapters.HTTP
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Private  Methods                          ****/
        /***************************************************/

        private static async Task<string> GetRequestAsync(string uri, HttpClient client = null)
        {
            if (client == null)
                client = new HttpClient();

            using (HttpResponseMessage response = await client.GetAsync(uri, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
            {
                string result = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    Engine.Base.Compute.ClearCurrentEvents();
                    Engine.Base.Compute.RecordError($"GET request failed with code {response.StatusCode}: {response.ReasonPhrase}");
                    return null;
                }
                else
                {
                    return result;
                }
            }
        }

        /***************************************************/

        private static async Task<string> GetRequestAsync(string url, Dictionary<string, object> headers = null, Dictionary<string, object> parameters = null, HttpClient client = null)
        {
            string uri = Convert.ToUrlString(url, parameters);
            return await GetRequestAsync(uri, client);
        }

        /***************************************************/

        private static async Task<string> GetRequestAsync(string url, CustomObject headers = null, CustomObject parameters = null, HttpClient client = null)
        {
            return await GetRequestAsync(url, headers.CustomData, parameters.CustomData, client);
        }

        /***************************************************/
    }
}





