using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Threading;

namespace webapi.inmemory.Filters
{
    public class WebApiOutputCacheAttribute : ActionFilterAttribute
    {
        // client cache length in seconds
        private int _clientTimeSpan;

        public WebApiOutputCacheAttribute(int clientTimeSpan)
        {
            _clientTimeSpan = clientTimeSpan;
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var cachecontrol = new CacheControlHeaderValue();
            cachecontrol.MaxAge = TimeSpan.FromSeconds(_clientTimeSpan);
            cachecontrol.MustRevalidate = true;
            actionExecutedContext.ActionContext.Response.Headers.CacheControl = cachecontrol;
        }
    }

}