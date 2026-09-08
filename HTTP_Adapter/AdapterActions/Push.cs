/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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

using BH.Engine.Reflection;
using BH.Engine.Serialiser;
using BH.oM.Adapter;
using BH.oM.Adapters.HTTP;
using BH.oM.Base;
using BH.oM.Data.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BH.Adapter.HTTP
{
    public partial class HTTPAdapter
    {
        /***************************************************/
        /**** Override Methods                          ****/
        /***************************************************/

        public override List<object> Push(IEnumerable<object> objects, string tag = "", PushType pushType = PushType.AdapterDefault, ActionConfig actionConfig = null)
        {
            if (!(actionConfig is HttpPushConfig pushConfig))
            {
                BH.Engine.Base.Compute.RecordWarning("Provided ActionConfig was not an HttpPushConfig, running with default push config arguments:\nRequestURI=\"\"\nForcePostAsList=false\nDeserialiseAsBHoM=true");
                pushConfig = new HttpPushConfig();
            }

            Uri requestUri = ConstructUri(pushConfig.RequestURL, pushConfig.Parameters);

            HttpRequestMessage requestMessage = Create.ConstructHttpRequestMessage(HttpMethod.Post, requestUri, pushConfig.Headers);

            if (objects.Count() == 1 & !pushConfig.ForcePostAsList)
                Compute.AddBHoMContent(ref requestMessage, objects.Single());
            else
                Compute.AddBHoMContent(ref requestMessage, objects);

            using (HttpResponseMessage response = m_httpClient.SendAsync(requestMessage).ConfigureAwait(false).GetAwaiter().GetResult())
            {
                //assert success
                if (!response.IsSuccessStatusCode)
                {
                    Engine.Base.Compute.RecordError($"POST request failed with code {response.StatusCode}: {response.ReasonPhrase}");
                    return new List<object>();
                }

                if (pushConfig.ForceDeserialiseAsBHoM || response.Content.Headers.ContentType.MediaType == "application/bhom")
                    return Compute.DeserialiseAsBHoMAsync(response).ConfigureAwait(false).GetAwaiter().GetResult();

                string responseString = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                return new List<object>() { responseString };
            }
        }
    }
}