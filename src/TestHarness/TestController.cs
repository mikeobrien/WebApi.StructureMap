using System;
using System.Web.Http;

namespace TestHarness
{
    public interface ISingletonDependency : IDisposable
    {
        int Value { get; }
        bool WasDisposedLastTime { get; }
    }

    public interface ITransientDependency : IDisposable
    {
        int Value { get; }
        bool WasDisposedLastTime { get; }
    }

    public class SingletonDependency : ISingletonDependency
    {
        private static bool _wasDisposedLastTime = false;
        private static int _counter;

        public int Value { get; set; } = _counter++;
        public bool WasDisposedLastTime => _wasDisposedLastTime;

        public void Dispose()
        {
            _wasDisposedLastTime = true;
        }
    }

    public class TransientDependency : ITransientDependency
    {
        private static bool _wasDisposedLastTime = false;
        private static int _counter;

        public int Value { get; set; } = _counter++;
        public bool WasDisposedLastTime => _wasDisposedLastTime;

        public void Dispose()
        {
            _wasDisposedLastTime = true;
        }
    }

    public class TestController : ApiController
    {
        private readonly ITransientDependency _transientDependency;
        private readonly ISingletonDependency _singletonDependency;

        public TestController(ITransientDependency transientDependency, 
            ISingletonDependency singletonDependency)
        {
            _transientDependency = transientDependency;
            _singletonDependency = singletonDependency;
        }

        public class Model
        {
            public int SingletonValue { get; set; }
            public bool SingletonWasDisposedLastTime { get; set; }
            public int TransientValue { get; set; }
            public bool TransientWasDisposedLastTime { get; set; }
        }

        [Route("test")]
        public IHttpActionResult Get()
        {
            return Ok(new Model
            {
                TransientValue = _transientDependency.Value,
                TransientWasDisposedLastTime = _transientDependency.WasDisposedLastTime,
                SingletonValue = _singletonDependency.Value,
                SingletonWasDisposedLastTime = _singletonDependency.WasDisposedLastTime
            });
        }
    }
}