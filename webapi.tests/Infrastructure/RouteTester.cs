using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;

namespace webapi.tests.Infrastructure
{
    public class RouteTester
    {
        HttpConfiguration config;
        HttpRequestMessage request;
        IHttpRouteData routeData;
        IHttpControllerSelector controllerSelector;
        HttpControllerContext controllerContext;

        public RouteTester(HttpConfiguration conf, HttpRequestMessage req)
        {
            config = conf;
            request = req;
            routeData = config.Routes.GetRouteData(request);
            request.Properties[HttpPropertyKeys.HttpRouteDataKey] = routeData;

            controllerSelector = new DefaultHttpControllerSelector(config);
            controllerContext = new HttpControllerContext(config, routeData, request);
        }

        public string GetActionName()
        {
            if (controllerContext.ControllerDescriptor == null)
                GetControllerType();

            var actionSelector = new ApiControllerActionSelector();
            var descriptor = actionSelector.SelectAction(controllerContext);

            return descriptor.ActionName;
        }

        public Type GetControllerType()
        {
            var descriptor = controllerSelector.SelectController(request);
            controllerContext.ControllerDescriptor = descriptor;
            return descriptor.ControllerType;
        }
    }
}
