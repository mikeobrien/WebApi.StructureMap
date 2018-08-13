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
        [Test]
        public void should_build_object_graph_and_dispose_nested_containers()
        {
            var client = new WebClient();
            client.Headers.Add("content-type", "application/json");
            client.Headers.Add("accept", "application/json");
            try
            {
                var url = IISBootstrap.BuildUrl("test");
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
                    client.DownloadString(url));
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

        [Test]
        public void should_retrieve_depedency_from_http_context_when_no_reponse_present()
        {
            var client = new WebClient();
            client.Headers.Add("content-type", "application/json");
            client.Headers.Add("accept", "application/json");

            var url = IISBootstrap.BuildUrl("test2");
            var result = client.DownloadString(url);

            result.ShouldBeEmpty();
            client.ResponseHeaders["ResponseStatus"].ShouldEqual("NoResponse");
        }
    }
}