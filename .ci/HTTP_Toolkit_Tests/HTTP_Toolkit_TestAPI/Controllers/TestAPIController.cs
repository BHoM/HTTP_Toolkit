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
using BH.oM.Geometry;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace HTTP_Toolkit_TestAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestAPIController : ControllerBase
    {
        private readonly ILogger<TestAPIController> _logger;

        public TestAPIController(ILogger<TestAPIController> logger)
        {
            _logger = logger;
        }

        /***************************************************/
        /**** HTTP requests - GET                       ****/
        /***************************************************/

        [HttpGet("health")]
        public ActionResult GetHealth() => Ok();

        /***************************************************/

        [HttpGet("default")]
        [Produces("application/bhom")]
        public ActionResult<Point> Get()
        {
            _logger.LogInformation("GET called");
            return Ok(new Point() { X=1, Y=2, Z=3 });
        }

        /***************************************************/

        [HttpGet("parameters")]
        [Produces("application/bhom")]
        public ActionResult<CustomObject> GetWithParameters(string parameter1, int parameter2)
        {
            _logger.LogInformation("GET with parameter {parameter1}, {parameter2} called.", parameter1, parameter2);
            return Ok(new CustomObject() { CustomData = new Dictionary<string, object>() { { "parameter1", parameter1 }, { "parameter2", parameter2 } } });
        }

        /***************************************************/

        [HttpGet("producesJson")]
        public ActionResult<string> GetJson()
        {
            Point point = new Point() { X=1 };
            string json = BH.Engine.Serialiser.Convert.ToJson(point);
            return Ok(json);
        }

        /***************************************************/
        /**** HTTP requests - POST                      ****/
        /***************************************************/

        [HttpPost("default")]
        [Consumes("application/bhom")]
        [Produces("application/bhom")]
        public ActionResult<Point> Post(Point point)
        {
            _logger.LogInformation("POST called");
            if (point.X == 4)
                _logger.LogInformation("point X component equal to 4");

            return Ok(point);
        }

        /***************************************************/

        [HttpPost("parameters")]
        [Consumes("application/bhom")]
        [Produces("application/bhom")]
        public ActionResult<CustomObject> PostWithParameters([FromBody]CustomObject customObject, [FromQuery]string parameter1, [FromQuery]int parameter2)
        {
            _logger.LogInformation("POST with parameters {parameter1}, {parameter2} called.", parameter1, parameter2);

            customObject.CustomData["parameter1"] = parameter1;
            customObject.CustomData["parameter2"] = parameter2;

            return Ok(customObject);
        }

        /***************************************************/

        [HttpPost("producesAndAcceptsJson")]
        [Consumes("application/json")]
        public ActionResult<string> PostJson([FromBody]JsonElement value)
        {
            object obj = JsonSerializer.Deserialize<Point>(value);

            _logger.LogInformation("Input json {json} deserialised to {objType}", value, obj.GetType().Name);

            if (obj is not Point p)
                return Problem("Provided string was not valid json/deserialised to incorrect object.", statusCode:400);

            p.Y = 4;
            string json = BH.Engine.Serialiser.Convert.ToJson(p);
            return Ok(json);
        }
    }
}
