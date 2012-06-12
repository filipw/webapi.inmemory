using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webapi.inmemory.Models
{
    public interface IUrlRepository
    {
        IQueryable<Url> GetAll();
        Url Get(int id);
        Url Add(Url url);
        Url Remove(int id);
    }

    public class UrlRepository : IUrlRepository
    {
        private List<Url> urls = new List<Url>();
        private int nextId = 1;

        public UrlRepository()
        {
            this.Add(new Url()
            {
                UrlId = 1,
                Address = "http://www.strathweb.com/2012/03/build-facebook-style-infinite-scroll-with-knockout-js-and-last-fm-api/",
                Title = "Build Facebook style infinite scroll with knockout.js and Last.fm API",
                CreatedBy = "Filip",
                CreatedAt = new DateTime(2012, 3, 20),
                Description = "Since knockout.js is one of the most amazing and innovative pieces of front-end code I have seen in recent years, I hope this is going to help you a bit in your everday battles. In conjuction with Last.FM API, we are going to create an infinitely scrollable history of your music records – just like the infinite scroll used on Facebook or on Twitter."
            });
            this.Add(new Url()
            {
                UrlId = 2,
                Address = "http://www.strathweb.com/2012/04/your-own-sports-news-site-with-espn-api-and-knockout-js/",
                Title = "Your own sports news site with ESPN API and Knockout.js",
                CreatedBy = "Filip",
                CreatedAt = new DateTime(2012, 4, 8),
                Description = "You will be able to browse the latest news from ESPN from all sports categories, as well as filter them by tags. The UI will be powered by KnockoutJS and Twitter bootstrap, and yes, will be a single page. We have already done two projects together using knockout.js – last.fm API infinite scroll and ASP.NET WebAPI file upload. Hopefully we will continue our knockout.js adventures in an exciting, and interesting for you, way."
            });
            this.Add(new Url()
            {
                UrlId = 3,
                Address = "http://www.strathweb.com/2012/04/rss-atom-mediatypeformatter-for-asp-net-webapi/",
                Title = "RSS & Atom MediaTypeFormatter for ASP.NET WebAPI",
                CreatedBy = "Filip",
                CreatedAt = new DateTime(2012, 4, 22),
                Description = "Today we are going to build a custom formatter for ASP.NET WebAPI, deriving from MediaTypeFormatter class. It will return our model (or collection of models) in RSS or Atom format."
            });
        }

        public IQueryable<Url> GetAll()
        {
            return this.urls.AsQueryable();
        }
        public Url Get(int id)
        {
            return this.urls.Find(i => i.UrlId == id);
        }
        public Url Add(Url url)
        {
            url.UrlId = nextId++;
            this.urls.Add(url);
            return url;
        }
        public Url Remove(int id)
        {
            var toRemove = this.urls.Find(i => i.UrlId.Equals(id));
            this.urls.Remove(toRemove);
            return toRemove;
        }
    }
}