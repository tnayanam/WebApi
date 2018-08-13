using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net;


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

        public course Get(int id)
        {
            var ret = courses.Where(c => c.id == id).FirstOrDefault();
            return ret;
        }

        public void Put(int id, [FromBody]course co)
        {
            var ret = courses.Where(c => c.id == id).FirstOrDefault();
            ret.title = co.title;
        }

        public void Post([FromBody] course c)
        {
            c.id = courses.Count;
            courses.Add(c);
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