using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;
using System.Web.Routing;
using webapi.inmemory;
using webapi.inmemory.Controllers;
using webapi.inmemory.Models;
using webapi.tests.Infrastructure;
using Xunit;

namespace webapi.tests
{
    public class RouteTests
    {
        HttpConfiguration _config;

        public RouteTests()
        {
            _config = new HttpConfiguration();
            _config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            _config.Routes.MapHttpRoute(name: "Default", routeTemplate: "api/{controller}/{id}", defaults: new { id = RouteParameter.Optional });
            _config.Routes.MapHttpRoute(name: "DefaultRPC", routeTemplate: "api/v2/{controller}/{action}/{id}", defaults: new { id = RouteParameter.Optional });
        }

        [Fact]
        public void UrlControllerGetIsCorrect()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "http://www.strathweb.com/api/url/");
            
            var routeTester = new RouteTester(_config, request);

            Assert.Equal(typeof(UrlController), routeTester.GetControllerType());
            Assert.Equal(ReflectionHelpers.GetMethodName((UrlController p) => p.Get()), routeTester.GetActionName());
        }

        [Fact]
        public void UrlControllerPostIsCorrect()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://www.strathweb.com/api/url/");

            var routeTester = new RouteTester(_config, request);

            Assert.Equal(typeof(UrlController), routeTester.GetControllerType());
            Assert.Equal(ReflectionHelpers.GetMethodName((UrlController p) => p.Add(new Url())), routeTester.GetActionName());
        }

        [Fact]
        public void V2_RPC_UrlControllerGetIsCorrect()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "http://www.strathweb.com/api/v2/url/Get");

            var routeTester = new RouteTester(_config, request);

            Assert.Equal(typeof(UrlController), routeTester.GetControllerType());
            Assert.Equal(ReflectionHelpers.GetMethodName((UrlController p) => p.Get()), routeTester.GetActionName());
        }

        [Fact]
        public void V2_RPC_UrlControllerPostIsCorrect()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://www.strathweb.com/api/v2/url/Add");

            var routeTester = new RouteTester(_config, request);

            Assert.Equal(typeof(UrlController), routeTester.GetControllerType());
            Assert.Equal(ReflectionHelpers.GetMethodName((UrlController p) => p.Add(new Url())), routeTester.GetActionName());
        }
    }
}
