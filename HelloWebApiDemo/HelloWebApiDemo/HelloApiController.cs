using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace HelloWebApiDemo
{
    public class HelloApiController : ApiController
    {
        public string Get()
        {
            return "Hello";
        }
    }
}

// link : https://app.pluralsight.com/player?course=aspnetwebapi&author=jon-flanders&name=aspnetwebapi-m1-introduction&clip=5&mode=live