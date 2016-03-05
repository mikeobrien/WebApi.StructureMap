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
        public void should_deserialize_and_serialize_with_bender()
        {
            var client = new WebClient();
            client.Headers.Add("content-type", "application/json");
            client.Headers.Add("accept", "application/json");
            try
            {
                var result = Deserialize.Json<TestController.Model>(
                    client.DownloadString($"{_website.Url}test"));
                result.SingletonValue.ShouldEqual(0);
                result.SingletonWasDisposedLastTime.ShouldBeFalse();
                result.TransientValue.ShouldEqual(0);
                result.TransientWasDisposedLastTime.ShouldBeFalse();

                result = Deserialize.Json<TestController.Model>(
                    client.DownloadString($"{_website.Url}test"));
                result.SingletonValue.ShouldEqual(0);
                result.SingletonWasDisposedLastTime.ShouldBeFalse();
                result.TransientValue.ShouldEqual(1);
                result.TransientWasDisposedLastTime.ShouldBeTrue();
            }
            catch (WebException exception)
            {
                Console.WriteLine(new StreamReader(exception.Response.GetResponseStream()).ReadToEnd());
                throw;
            }
        }
    }
}