using BH.Adapter.HTTP;
using BH.Engine.Serialiser;
using BH.oM.Adapters.HTTP;
using BH.oM.Base;
using BH.oM.Base.Debugging;
using BH.oM.Geometry;

namespace HTTP_Adapter_Tests
{
    public class PushTests
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
        public void TestDefaultPush()
        {
            Point point = new Point() { X = 4 };

            List<object> objs = _adapter.Push(new List<object>() { point }, actionConfig: new HttpPushConfig() { RequestURL = "TestAPI/default"});

            Assert.That(objs, Is.Not.Null);
            Assert.That(objs, Is.Not.Empty);
            Assert.That(objs, Has.Count.EqualTo(1));
            Assert.That(objs[0], Is.Not.Null);
            Assert.That(objs[0], Is.TypeOf(typeof(Point)));
            Assert.That(objs[0], Has.Property("X").EqualTo(4));
            Assert.That(objs[0], Has.Property("Y").EqualTo(0));
            Assert.That(objs[0], Has.Property("Z").EqualTo(0));
        }

        [Test]
        public void TestPushWithParameters()
        {
            CustomObject obj = new CustomObject() { CustomData = new Dictionary<string, object>() { { "existingProp", 1 } } };
            HttpPushConfig config = new HttpPushConfig() { RequestURL = "TestAPI/parameters", Parameters = new Dictionary<string, object>() { { "parameter1", "p1" }, { "parameter2", 2 } } };

            List<object> objs = _adapter.Push(new List<object>() { obj }, actionConfig: config);

            Assert.That(objs, Is.Not.Null);
            Assert.That(objs, Is.Not.Empty);
            Assert.That(objs, Has.Count.EqualTo(1));
            Assert.That(objs[0], Is.Not.Null);
            Assert.That(objs[0], Is.TypeOf<CustomObject>());

            CustomObject co = (CustomObject)objs[0];

            Assert.That(co.CustomData, Has.ItemAt("existingProp").EqualTo(1));
            Assert.That(co.CustomData, Has.ItemAt("parameter1").EqualTo("p1"));
            Assert.That(co.CustomData, Has.ItemAt("parameter2").EqualTo(2));
        }

        [Test]
        public void TestPushJsonProducesJson()
        {
            Point point = new Point() { X = 4 };
            HttpPushConfig config = new HttpPushConfig() { RequestURL = "TestAPI/producesAndAcceptsJson", PushedContentType = "application/json" };

            List<object> objs = _adapter.Push(new List<object>() { point }, actionConfig: config);

            List<Event> events = BH.Engine.Base.Query.CurrentEvents();

            Assert.That(objs, Is.Not.Null);
            Assert.That(objs, Is.Not.Empty);
            Assert.That(objs, Has.Count.EqualTo(1));
            Assert.That(objs[0], Is.Not.Null);
            Assert.That(objs[0], Is.TypeOf<string>());

            object rtn = BH.Engine.Serialiser.Convert.FromJson((string)objs[0]);

            Assert.That(rtn, Is.TypeOf<Point>());
            Assert.That(rtn, Has.Property("X").EqualTo(4));
            Assert.That(rtn, Has.Property("Y").EqualTo(4));
        }
    }
}