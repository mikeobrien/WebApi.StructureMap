using System;
using System.IO;
using System.Net;
using Bender;
using NUnit.Framework;
using Should;
using TestHarness;

namespace Tests
{
    [TestFixture]
    public class AcceptanceTests
    {
        private Website _website;

        [TestFixtureSetUp]
        public void Setup()
        {
            _website = Website.Create(@"..\..\..\TestHarness");
            _website.Start();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            _website.Stop();
        }

        [Test]
        public void should_build_object_graph_and_dispose_nested_containers()
        {
            var client = new WebClient();
            client.Headers.Add("content-type", "application/json");
            client.Headers.Add("accept", "application/json");
            try
            {
                var url = $"{_website.Url}test";
                var result = Deserialize.Json<TestController.Model>(
                    client.DownloadString(url));

                result.RequestUrl.ShouldEqual(url);

                var singletonInstance = result.SingletonInstance;
                result.SingletonWasDisposedLastTime.ShouldBeFalse();
                var transientInstance = result.TransientInstance;
                result.TransientWasDisposedLastTime.ShouldBeFalse();

                var filterSingletonInstance = int.Parse(client.ResponseHeaders["SingletonInstance"]);
                var filterTransientInstance = int.Parse(client.ResponseHeaders["TransientInstance"]);

                filterSingletonInstance.ShouldEqual(singletonInstance);
                filterTransientInstance.ShouldEqual(transientInstance);

                result = Deserialize.Json<TestController.Model>(
                    client.DownloadString($"{_website.Url}test"));
                result.SingletonInstance.ShouldEqual(singletonInstance);
                result.SingletonWasDisposedLastTime.ShouldBeFalse();
                result.TransientInstance.ShouldNotEqual(transientInstance);
                result.TransientWasDisposedLastTime.ShouldBeTrue();

                filterSingletonInstance = int.Parse(client.ResponseHeaders["SingletonInstance"]);
                filterTransientInstance = int.Parse(client.ResponseHeaders["TransientInstance"]);

                filterSingletonInstance.ShouldEqual(singletonInstance);
                filterTransientInstance.ShouldEqual(result.TransientInstance);
            }
            catch (WebException exception)
            {
                Console.WriteLine(new StreamReader(exception.Response.GetResponseStream()).ReadToEnd());
                throw;
            }
        }
    }
}