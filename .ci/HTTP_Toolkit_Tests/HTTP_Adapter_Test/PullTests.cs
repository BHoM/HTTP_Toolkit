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

using BH.Adapter.HTTP;
using BH.oM.Adapters.HTTP;
using BH.oM.Base;
using BH.oM.Base.Debugging;
using BH.oM.Geometry;

namespace HTTP_Adapter_Tests
{
    public class PullTests
    {
        private HTTPAdapter _adapter = default!;

        /***************************************************/
        /**** Setup                                     ****/
        /***************************************************/

        [OneTimeSetUp]
        [Description("One time set up.")]
        public void OneTimeSetUp()
        {
            _adapter = new HTTPAdapter("http://localhost:5068");
        }

        /***************************************************/

        [SetUp]
        [Description("These tests need the TestAPI project running to work properly. If the api is not running, then all tests will be marked as inconclusive.")]
        public void SetUp()
        {
            try
            {
                using HttpResponseMessage message = _adapter.HttpClient.GetAsync("TestAPI/health").Result;

                //the test API (a simple API that returns and accepts BHoM Point objects) isn't running, so report as inconclusive.
                if (message.StatusCode != System.Net.HttpStatusCode.OK)
                    Assert.Inconclusive("Test reported as inconclusive as the test API is not responding, maybe it isn't running?"); 
            }
            catch (Exception ex)
            {
                Assert.Inconclusive("Test reported as inconclusive as the test API is not responding, maybe it isn't running?");
            }
        }

        /***************************************************/
        /**** Tests                                     ****/
        /***************************************************/

        [Test]
        [Description("Tests the default behaviour of GETting with the adapter. The endpoint should return a point.")]
        public void TestDefaultPull()
        {
            GetRequest request = new GetRequest() { BaseUrl = "TestAPI/default" };

            List<object> objs = _adapter.Pull(request).ToList();

            Assert.That(objs, Is.Not.Null);
            Assert.That(objs, Is.Not.Empty);
            Assert.That(objs, Has.Count.EqualTo(1));
            Assert.That(objs[0], Is.Not.Null);
            Assert.That(objs[0], Is.TypeOf<Point>());
            Assert.That(objs[0], Has.Property("X").EqualTo(1));
            Assert.That(objs[0], Has.Property("Y").EqualTo(2));
            Assert.That(objs[0], Has.Property("Z").EqualTo(3));
        }

        /***************************************************/

        [Test]
        [Description("Tests pulling with query parameters, the TestAPI puts the parameters in a custom object to allow verification.")]
        public void TestPullWithParameters()
        {
            GetRequest request = new GetRequest() { BaseUrl = "TestAPI/parameters", Parameters = new Dictionary<string, object>() { { "parameter1", "p1" }, { "parameter2", 2 } } };

            List<object> objs = _adapter.Pull(request).ToList();

            Assert.That(objs, Is.Not.Null);
            Assert.That(objs, Is.Not.Empty);
            Assert.That(objs, Has.Count.EqualTo(1));
            Assert.That(objs[0], Is.Not.Null);
            Assert.That(objs[0], Is.TypeOf<CustomObject>());

            CustomObject co = (CustomObject)objs[0];

            Assert.That(co.CustomData, Has.ItemAt("parameter1").EqualTo("p1"));
            Assert.That(co.CustomData, Has.ItemAt("parameter2").EqualTo(2));
        }

        /***************************************************/

        [Test]
        [Description("Tests getting as a string while setting force deserialisation to false, where the content type of the return is not set to 'application/bhom'.")]
        public void TestPullProducesJson()
        {
            GetRequest request = new GetRequest() { BaseUrl = "TestAPI/producesJson"};
            HttpPullConfig config = new HttpPullConfig() { ForceDeserialiseAsBHoM = false };

            List<object> objs = _adapter.Pull(request, config).ToList();

            Assert.That(objs, Is.Not.Null);
            Assert.That(objs, Is.Not.Empty);
            Assert.That(objs, Has.Count.EqualTo(1));
            Assert.That(objs[0], Is.Not.Null);
            Assert.That(objs[0], Is.TypeOf<string>());

            object rtn = BH.Engine.Serialiser.Convert.FromJson((string)objs[0]);

            Assert.That(rtn, Is.TypeOf<Point>());
            Assert.That(rtn, Has.Property("X").EqualTo(1));
        }
    }
}