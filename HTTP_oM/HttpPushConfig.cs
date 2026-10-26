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

using BH.oM.Adapter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BH.oM.Adapters.HTTP
{
    public class HttpPushConfig : ActionConfig
    {
        [Description("The uri provided is appended to the base URI set in the adapter. If the base uri is empty, this must be the full URL for the requested resource.")]
        public virtual string RequestURL { get; set; } = "";

        [Description("If only one object is provided to the Push, this forces the adapter to push it as a json array instead of a single object. By default the adapter will push single objects alone instead of as a list.")]
        public virtual bool ForcePostAsList { get; set; } = false;

        [Description("Defines the content type of the POST message. If the endpoint being sent to does not directly accept application/bhom as a content-type, it can be changed here. Most likely changed to 'application/json'.")]
        public virtual string PushedContentType { get; set; } = "application/bhom";

        [Description("If the response from the POST request is expected to be a BHoM object but doesn't have content type of 'application/bhom', this forces the adapter to deserialise the result as BHoM formatted json. Otherwise the adapter returns the response content as a string.")]
        public virtual bool ForceDeserialiseAsBHoM { get; set; } = false;

        [Description("Custom request headers to send with the POST request.")]
        public virtual Dictionary<string, object> Headers { get; set; } = new Dictionary<string, object>();

        [Description("Custom parameters to provide to the POST request.")]
        public virtual Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
    }
}
