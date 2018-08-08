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

     //   [Route("")]
        public HttpResponseMessage Get()
        {
            var results = new
            {
                NumFoods = TheRepository.GetAllFoods().Count(),
                NumUsers = TheRepository.GetApiUsers().Count()
            };

            return Request.CreateResponse(HttpStatusCode.OK, results);
        }

        //   [Route("~/api/stat/{id}")] // now this route will use this entry route where as rest of them will use the standard one defined at controller level.
        public HttpResponseMessage Get(int id)
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
