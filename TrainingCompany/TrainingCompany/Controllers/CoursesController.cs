using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net;
using System.Net.Http;


namespace TrainingCompany.Controllers
{
    public class CoursesController : ApiController
    {
         [HttpGet]
        public IEnumerable<course> AllCourses()
        {
            return courses;
        }

        static List<course> courses = InitCourses();
        private static List<course> InitCourses()
        {
            var ret = new List<course>();
            ret.Add(new course { id = 0, title = "Web Application1" });
            ret.Add(new course { id = 1, title = "Web Application2" });
            return ret;
        }

        public HttpResponseMessage Get(int id)
        {
            var ret = courses.Where(c => c.id == id).FirstOrDefault();
            HttpResponseMessage msg = null;
            if(ret == null)
            {
                msg = Request.CreateResponse(HttpStatusCode.NotFound, "Course could not be found");
            }
            else
            {
                msg = Request.CreateResponse<course>(HttpStatusCode.OK, ret); // more Rest type
            }
            return msg;
        }

        public void Put(int id, [FromBody]course co)
        {
            var ret = courses.Where(c => c.id == id).FirstOrDefault();
            ret.title = co.title;
        }

        public HttpResponseMessage Post([FromBody] course c)
        {
            c.id = courses.Count;
            courses.Add(c);
            var msg = Request.CreateResponse(HttpStatusCode.Created);
            msg.Headers.Location = new Uri(Request.RequestUri + c.id.ToString()); // this is REST convention!
            return msg;
        }

        public void Delete(int id)
        {
            var ret = courses.Where(c => c.id == id).FirstOrDefault();
            courses.Remove(ret);
        }
    }

    public class course
    {
        public int id;
        public string title;
    }
}