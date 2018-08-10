using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace HelloWebApiDemo
{
    public class SecondController: ApiController
    {
        public string Get()
        {
            return "my";
        }
        // if you want the resrult in xmll check the content negotiation.
    }
}