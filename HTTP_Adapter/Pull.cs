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

using System.Collections.Generic;
using BH.oM.Base;
using BH.oM.DataManipulation.Queries;
using BH.oM.HTTP;

namespace BH.Adapter.HTTP
{
    public partial class HTTPAdapter
    {
        /***************************************************/
        /**** Interface Methods                         ****/
        /***************************************************/

        public override IEnumerable<object> Pull(IQuery query, Dictionary<string, object> config = null)
        {
            return Pull(query as dynamic, config);
        }

        /***************************************************/
        /**** Fallback Case                             ****/
        /***************************************************/

        public IEnumerable<object> Pull(object query, Dictionary<string, object> config = null)
        {
            Engine.Reflection.Compute.RecordError($"Unknown query type {query.GetType()}.\n" +
                "If you are making a GET request, please use the BH.oM.HTTP.GetQuery object to specify the query.");
            return null;
        }

        /***************************************************/

        public IEnumerable<object> Pull(GetQuery query, Dictionary<string, object> config = null)
        {
            string response = Engine.HTTP.Compute.GetRequest(query);

            BHoMObject obj = Engine.Serialiser.Convert.FromJson(response) as BHoMObject;
            if (obj == null)
            {
                // in case the response is not a valid json, wrap it around a CustomObject
                return new List<BHoMObject>
                {
                    new CustomObject()
                    {
                        CustomData = new Dictionary<string, object>() { { "Response", response } }
                    }
                };
            }

            return new List<BHoMObject> { obj }; // This is at least a CustomObject
        }

        /***************************************************/
    }
}