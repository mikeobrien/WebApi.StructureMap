using System.Net.Http;
using System.Web.Http;

namespace TestHarness
{
    public class TestController : ApiController
    {
        private readonly ITransientDependency _transientDependency;
        private readonly ISingletonDependency _singletonDependency;
        private readonly HttpRequestMessage _request;

        public TestController(
            ITransientDependency transientDependency, 
            ISingletonDependency singletonDependency,
            HttpRequestMessage request)
        {
            _transientDependency = transientDependency;
            _singletonDependency = singletonDependency;
            _request = request;
        }

        public class Model
        {
            public int SingletonInstance { get; set; }
            public bool SingletonWasDisposedLastTime { get; set; }
            public int TransientInstance { get; set; }
            public bool TransientWasDisposedLastTime { get; set; }
            public string RequestUrl { get; set; }
        }

        [Route("test")]
        public IHttpActionResult Get()
        {
            return Ok(new Model
            {
                TransientInstance = _transientDependency.GetHashCode(),
                TransientWasDisposedLastTime = _transientDependency.WasDisposedLastTime,
                SingletonInstance = _singletonDependency.GetHashCode(),
                SingletonWasDisposedLastTime = _singletonDependency.WasDisposedLastTime,
                RequestUrl = _request.RequestUri.ToString()
            });
        }
    }
}