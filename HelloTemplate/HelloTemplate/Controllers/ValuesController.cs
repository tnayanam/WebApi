using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HelloTemplate.Controllers
{
    public class ValuesController : ApiController
    {
        static List<string> data = initList();

        private static List<string> initList()
        {
            var temp = new List<string>();
            temp.Add("one");
            temp.Add("two");
            temp.Add("three");
            temp.Add("four");
            return temp;
        }

        // GET api/values
        public IEnumerable<string> Get()
        {
            return data;
        }

        // GET api/values/5
        public HttpResponseMessage Get(int id)
        {
            if (data.Count > id)
            {
                return Request.CreateResponse(HttpStatusCode.OK, data[id]);
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Item not found.");
        }

        // POST api/values
        public void Post([FromBody]string value) // fiddler test for post
        {
            data.Add(value);
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
            data[id] = value;
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
            data.RemoveAt(id);
        }
    }
}
