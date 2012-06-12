using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using webapi.inmemory.Filters;
using webapi.inmemory.Models;

namespace webapi.inmemory.Controllers
{
    public class UrlController : ApiController
    {
        private readonly IUrlRepository urlRepo = new UrlRepository();

        [WebApiOutputCache(60)]
        public IQueryable<Url> Get()
        {
            return urlRepo.GetAll();
        }

        public Url Get(int id)
        {
            return urlRepo.Get(id);
        }

        public Url Post(Url url)
        {
            return urlRepo.Add(url);
        }

        [HttpPost]
        public Url Remove(int id)
        {
            return urlRepo.Remove(id);
        }
    }
}