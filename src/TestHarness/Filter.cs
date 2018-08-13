using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using WebApi.StructureMap;

namespace TestHarness
{
    public class TestFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            actionContext.GetService<DummyService>().DoSomething();
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            actionExecutedContext.GetService<HeaderService>().SetHeaders();
        }
    }

    public class ExceptionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var service = actionExecutedContext.GetService<ResponseInspectorService>();
            actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.OK);
            actionExecutedContext.Response.Headers.Add("ResponseStatus", service.InspectResponse());
        }
    }

    public class DummyService
    {
        public DummyService(
            HttpActionExecutedContext context,
            ISingletonDependency singletonDependency,
            ITransientDependency transientDependency) { }

        public void DoSomething() { }
    }

    public class HeaderService
    {
        private readonly HttpResponseMessage _response;
        private readonly ISingletonDependency _singletonDependency;
        private readonly ITransientDependency _transientDependency;

        public HeaderService(
            HttpResponseMessage response,
            ISingletonDependency singletonDependency,
            ITransientDependency transientDependency)
        {
            _response = response;
            _singletonDependency = singletonDependency;
            _transientDependency = transientDependency;
        }

        public void SetHeaders()
        {
            _response.Headers.Add("SingletonInstance",
                _singletonDependency.GetHashCode().ToString());
            _response.Headers.Add("TransientInstance",
                _transientDependency.GetHashCode().ToString());
        }
    }

    public class ResponseInspectorService
    {
        private readonly HttpResponseMessage _response;

        public ResponseInspectorService(
            HttpResponseMessage response)
        {
            _response = response;
        }

        public string InspectResponse()
        {
            return _response == null ? "NoResponse" : "ResponsePresent";
        }
    }
}