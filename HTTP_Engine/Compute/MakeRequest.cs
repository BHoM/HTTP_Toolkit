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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using BH.oM.Adapters.HTTP;
using System.Linq;
using System.Threading.Tasks;

namespace BH.Engine.Adapters.HTTP
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public  Method                            ****/
        /***************************************************/

        [Description("Makes an HTTP GET request and returns the response content as a string.")]
        [Input("request", "The GET request object containing the base URL, headers and parameters.")]
        [Output("response", "The response content as a string.")]
        public static string MakeRequest(GetRequest request)
        {
            return GetRequest(request.BaseUrl, request.Headers, request.Parameters);
        }

        /***************************************************/

        [Description("Makes an HTTP GET request and returns the response content as a byte array.")]
        [Input("request", "The GET request object containing the base URL, headers and parameters.")]
        [Output("response", "The response content as a byte array.")]
        public static byte[] MakeRequestBinary(GetRequest request)
        {
            return GetRequestBinary(request.BaseUrl, request.Headers, request.Parameters);
        }

        /***************************************************/

    }
}







