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
        public void OneTimeSetUp()
        {
            _adapter = new HTTPAdapter("http://localhost:5068");
        }

        /***************************************************/

        [SetUp]
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