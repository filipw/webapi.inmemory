using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Routing;
using Newtonsoft.Json;
using webapi.inmemory.Controllers;
using webapi.inmemory.Handlers;
using webapi.inmemory.Models;
using Xunit;

namespace webapi.tests
{
    public class WebApiIntegrationTests : IDisposable
    {
        private HttpServer _server;
        private string _url = "http://www.strathweb.com/";

        public WebApiIntegrationTests()
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(name: "Default", routeTemplate: "api/{controller}/{action}/{id}", defaults: new { id = RouteParameter.Optional });
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            config.MessageHandlers.Add(new WebApiKeyHandler());

            _server = new HttpServer(config);
        }

        [Fact]
        public void GetAllUrls()
        {
            var client = new HttpClient(_server);
            var request = createRequest("api/url/get?apikey=test", "application/json", HttpMethod.Get);
            var expectedJson = "[{\"UrlId\":1,\"Address\":\"http://www.strathweb.com/2012/03/build-facebook-style-infinite-scroll-with-knockout-js-and-last-fm-api/\",\"Title\":\"Build Facebook style infinite scroll with knockout.js and Last.fm API\",\"Description\":\"Since knockout.js is one of the most amazing and innovative pieces of front-end code I have seen in recent years, I hope this is going to help you a bit in your everday battles. In conjuction with Last.FM API, we are going to create an infinitely scrollable history of your music records – just like the infinite scroll used on Facebook or on Twitter.\",\"CreatedAt\":\"2012-03-20T00:00:00\",\"CreatedBy\":\"Filip\"},{\"UrlId\":2,\"Address\":\"http://www.strathweb.com/2012/04/your-own-sports-news-site-with-espn-api-and-knockout-js/\",\"Title\":\"Your own sports news site with ESPN API and Knockout.js\",\"Description\":\"You will be able to browse the latest news from ESPN from all sports categories, as well as filter them by tags. The UI will be powered by KnockoutJS and Twitter bootstrap, and yes, will be a single page. We have already done two projects together using knockout.js – last.fm API infinite scroll and ASP.NET WebAPI file upload. Hopefully we will continue our knockout.js adventures in an exciting, and interesting for you, way.\",\"CreatedAt\":\"2012-04-08T00:00:00\",\"CreatedBy\":\"Filip\"},{\"UrlId\":3,\"Address\":\"http://www.strathweb.com/2012/04/rss-atom-mediatypeformatter-for-asp-net-webapi/\",\"Title\":\"RSS & Atom MediaTypeFormatter for ASP.NET WebAPI\",\"Description\":\"Today we are going to build a custom formatter for ASP.NET WebAPI, deriving from MediaTypeFormatter class. It will return our model (or collection of models) in RSS or Atom format.\",\"CreatedAt\":\"2012-04-22T00:00:00\",\"CreatedBy\":\"Filip\"}]";

            using (HttpResponseMessage response = client.SendAsync(request).Result)
            {
                Assert.NotNull(response.Content);
                Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);
                Assert.Equal(3, response.Content.ReadAsAsync<IQueryable<Url>>().Result.Count());
                Assert.Equal(60, response.Headers.CacheControl.MaxAge.Value.TotalSeconds);
                Assert.Equal(true, response.Headers.CacheControl.MustRevalidate);
                Assert.Equal(expectedJson, response.Content.ReadAsStringAsync().Result);
            }

            request.Dispose();
        }

        [Fact]
        public void PostSingleUrl()
        {
            var client = new HttpClient(_server);
            var newUrl = new Url() { Title = "Test post", Address = "http://www.strathweb.com", Description = "This is test post", CreatedAt = DateTime.Now, CreatedBy = "Filip", UrlId = 4 };
            var request = createRequest("api/url/add?apikey=test", "application/json", HttpMethod.Post, newUrl, new JsonMediaTypeFormatter());
            var expectedJson = JsonConvert.SerializeObject(newUrl);
            //"{\"UrlId\":4,\"Address\":\"http://www.strathweb.com\",\"Title\":\"Test post\",\"Description\":\"This is test post\",\"CreatedAt\":\"2012-06-10T23:23:22.8292873+02:00\",\"CreatedBy\":\"Filip\"}"

            using (HttpResponseMessage response = client.SendAsync(request, new CancellationTokenSource().Token).Result)
            {
                Assert.NotNull(response.Content);
                Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);
                Assert.Equal(newUrl, response.Content.ReadAsAsync<Url>().Result);
                Assert.Equal(expectedJson, response.Content.ReadAsStringAsync().Result);
            }

            request.Dispose();
        }

        [Fact]
        public void GetAllUrlsNoApiKey()
        {
            var client = new HttpClient(_server);
            var request = createRequest("api/url/get", "application/json", HttpMethod.Get);

            using (HttpResponseMessage response = client.SendAsync(request).Result)
            {
                Assert.NotNull(response.Content);
                Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
                Assert.Equal("You can't use the API without the key.", response.Content.ReadAsStringAsync().Result);
            }

            request.Dispose();
        }

        [Fact]
        public void PostRemoveSingleUrl()
        {
            var client = new HttpClient(_server);
            var request = createRequest("api/url/delete/1?apikey=test", "application/json", HttpMethod.Delete);
            var expectedJson = "{\"UrlId\":1,\"Address\":\"http://www.strathweb.com/2012/03/build-facebook-style-infinite-scroll-with-knockout-js-and-last-fm-api/\",\"Title\":\"Build Facebook style infinite scroll with knockout.js and Last.fm API\",\"Description\":\"Since knockout.js is one of the most amazing and innovative pieces of front-end code I have seen in recent years, I hope this is going to help you a bit in your everday battles. In conjuction with Last.FM API, we are going to create an infinitely scrollable history of your music records – just like the infinite scroll used on Facebook or on Twitter.\",\"CreatedAt\":\"2012-03-20T00:00:00\",\"CreatedBy\":\"Filip\"}"; 

            using (HttpResponseMessage response = client.SendAsync(request, new CancellationTokenSource().Token).Result)
            {
                Assert.NotNull(response.Content);
                Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);
                Assert.Equal(1, response.Content.ReadAsAsync<Url>().Result.UrlId);
                Assert.Equal(expectedJson, response.Content.ReadAsStringAsync().Result);
            }

            request.Dispose();
        }

        private HttpRequestMessage createRequest(string url, string mthv, HttpMethod method)
        {
            var request = new HttpRequestMessage();

            request.RequestUri = new Uri(_url + url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mthv));
            request.Method = method;

            return request;
        }

        private HttpRequestMessage createRequest<T>(string url, string mthv, HttpMethod method, T content, MediaTypeFormatter formatter) where T : class
        {
            HttpRequestMessage request = createRequest(url, mthv, method);
            request.Content = new ObjectContent<T>(content, formatter);

            return request;
        }

        public void Dispose()
        {
            if (_server != null)
            {
                _server.Dispose();
            }
        }
    }
}
