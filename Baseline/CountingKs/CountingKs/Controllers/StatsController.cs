using CountingKs.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CountingKs.Controllers
{
    //[RoutePrefix("api/stats")] // optimization of routes.
    public class StatsController : BaseApiController
    {
        public StatsController(ICountingKsRepository repo) :base(repo)
        {
        }

     //   [Route("")] // this needs to be tehre even if you dont want the non standard routes.  empty one should be there on each routes.
        public HttpResponseMessage Get()
        {
            var results = new
            {
                NumFoods = TheRepository.GetAllFoods().Count(),
                NumUsers = TheRepository.GetApiUsers().Count()
            };

            return Request.CreateResponse(HttpStatusCode.OK, results);
        }

        //   [Route("~/api/stat/{id:integer}")] // now this route will use this entry route where as rest of them will use the standard one defined at controller level.
        public HttpResponseMessage Get(int id)
        {
            var results = new
            {
                NumFoods = TheRepository.GetAllFoods().Count(),
                NumUsers = TheRepository.GetApiUsers().Count()
            };

            return Request.CreateResponse(HttpStatusCode.OK, results);
        }

        //   [Route("~/api/stat/{name:alpha}")]
        public HttpResponseMessage Get(string name)
        {
            var results = new
            {
                NumFoods = TheRepository.GetAllFoods().Count(),
                NumUsers = TheRepository.GetApiUsers().Count()
            };

            return Request.CreateResponse(HttpStatusCode.OK, results);
        }
    }
}
