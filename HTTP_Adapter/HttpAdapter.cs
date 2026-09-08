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

using BH.oM.Adapters.HTTP;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace BH.Adapter.HTTP
{
    public partial class HTTPAdapter : BHoMAdapter
    {
        /***************************************************/
        /**** Public fields                             ****/
        /***************************************************/

        public static string AdapterID = "HTTPAdapter";

        public HttpClient HttpClient => m_httpClient; //allows custom requests in code if necessary (e.g. need to POST with some custom non-bhom content)

        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public HTTPAdapter()
        {
            // Use TLS 1.2 and TLS 1.3 (if available). Deprecated protocols not supported any longer: SSL3, TLS 1.0, TLS 1.1.
            // TLS 1.2 is widely supported and secure. TLS 1.3 may not be available in all .NET Framework versions.
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 |  (SecurityProtocolType)3072; // 3072 = Tls13
            m_httpClient = new HttpClient();
        }

        /***************************************************/
        public HTTPAdapter(string baseAddress)
        {
            // Use TLS 1.2 and TLS 1.3 (if available). Deprecated protocols not supported any longer: SSL3, TLS 1.0, TLS 1.1.
            // TLS 1.2 is widely supported and secure. TLS 1.3 may not be available in all .NET Framework versions.
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | (SecurityProtocolType)3072; // 3072 = Tls13
            m_httpClient = new HttpClient() { BaseAddress = new Uri(baseAddress) };
        }

        /***************************************************/

        public HTTPAdapter(HttpClient httpClient)
        {
            // Use TLS 1.2 and TLS 1.3 (if available). Deprecated protocols not supported any longer: SSL3, TLS 1.0, TLS 1.1.
            // TLS 1.2 is widely supported and secure. TLS 1.3 may not be available in all .NET Framework versions.
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | (SecurityProtocolType)3072; // 3072 = Tls13
            m_httpClient = httpClient;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private Uri ConstructUri(string absoluteOrRelativeUrl, Dictionary<string, object> parameters)
        {
            Uri baseUri;

            if (m_httpClient.BaseAddress != null)
                baseUri = new Uri(m_httpClient.BaseAddress, absoluteOrRelativeUrl);
            else
                baseUri = new Uri(absoluteOrRelativeUrl);

            string queryParameters = Engine.Adapters.HTTP.Convert.ToUrlString(parameters);

            UriBuilder builder = new UriBuilder(baseUri) { Query = queryParameters };

            return builder.Uri;
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private readonly HttpClient m_httpClient;
    }
}







