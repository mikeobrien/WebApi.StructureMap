using System;

namespace TestHarness
{
    public interface IDependency : IDisposable
    {
        bool WasDisposedLastTime { get; }
    }

    public interface ISingletonDependency : IDependency { }
    public interface ITransientDependency : IDependency { }

    public class SingletonDependency : ISingletonDependency
    {
        private static bool _wasDisposedLastTime;

        public bool WasDisposedLastTime => _wasDisposedLastTime;

        public void Dispose()
        {
            _wasDisposedLastTime = true;
        }
    }

    public class TransientDependency : ITransientDependency
    {
        private static bool _wasDisposedLastTime;

        public bool WasDisposedLastTime => _wasDisposedLastTime;

        public void Dispose()
        {
            _wasDisposedLastTime = true;
        }
    }
}