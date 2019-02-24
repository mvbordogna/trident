using System;
using System.Net.Http;
using System.Security.Principal;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;

namespace TestHelpers.WebApi
{
    public static class ActionContextBuilder
    {
        public static HttpActionContext Build<T>(string methodInfo, IPrincipal userPrincipal)
            where T : IHttpController
        {
            var controllerType = typeof(T);

            var actionInfo = controllerType.GetMethod(methodInfo);
            var controllerDescriptor = new HttpControllerDescriptor(
                new HttpConfiguration(),
                nameof(controllerType),
                controllerType
            );

            var controller = (IHttpController)Activator.CreateInstance(controllerType);

            return new HttpActionContext
            {
                ControllerContext = new HttpControllerContext
                {
                    Request = new HttpRequestMessage
                    {
                        Properties = { { HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration() } }                   
                    },
                    Controller = controller,
                    ControllerDescriptor = controllerDescriptor,
                    RequestContext = new HttpRequestContext
                    {
                        Principal = userPrincipal
                    }
                },
                ActionDescriptor = new ReflectedHttpActionDescriptor(controllerDescriptor, actionInfo)
            };

        }
    }
}
