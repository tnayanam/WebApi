using CountingKs.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CountingKs.Controllers
{
    public class StatsController : BaseApiController
    {
        public StatsController(ICountingKsRepository repo) :base(repo)
        {
        }

        public HttpResponseMessage Get()
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
