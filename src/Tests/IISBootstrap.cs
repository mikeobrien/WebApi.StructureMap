using System.Diagnostics;
using System.Linq;
using IISExpressBootstrapper;
using NUnit.Framework;

namespace Tests
{
    [SetUpFixture]
    public class IISBootstrap
    {
        public const int Port = 4743;

        private IISExpressHost _host;

        [OneTimeSetUp]
        public void StartIIS()
        {
            _host = new IISExpressHost("TestHarness", Port);
        }

        [OneTimeTearDown]
        public void StopIIS()
        {
            _host.Dispose();
        }

        public static string BuildUrl(string relativeUrl)
        {
            // Fiddler can't hook into localhost so when its running 
            // you can use localhost.fiddler
            var host = Process.GetProcessesByName("Fiddler").Any() ?
                    "localhost.fiddler" : "localhost";
            return $"http://{host}:{Port}/{relativeUrl}";
        }
    }
}
