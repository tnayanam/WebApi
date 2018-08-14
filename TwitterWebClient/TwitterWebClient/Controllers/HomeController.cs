using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;

namespace TwitterWebClient.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Tweets model = null;
            var client = new HttpClient();
            var task = 
                client.GetAsync(
                "http://search.twitter.com/search.json?q=pluralsight")
                .ContinueWith((taskwithresponse) =>
                {
                    var response = taskwithresponse.Result;
                    var readtask = response.Content.ReadAsAsync<Tweets>();
                    readtask.Wait();
                    model = readtask.Result;
                });
            task.Wait();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
