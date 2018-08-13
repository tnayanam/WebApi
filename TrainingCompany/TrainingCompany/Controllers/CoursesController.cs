using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace TrainingCompany.Controllers
{
    public class CoursesController : ApiController
    {
        public IEnumerable<course> Get()
        {
            return courses;
        }

        static List<course> courses = InitCourses();
        private static List<course> InitCourses()
        {
            var ret = new List<course>();
            ret.Add(new course{id = 0, title = "Web Application1"});
            ret.Add(new course{id = 1, title = "Web Application2"});
            return ret;
        }


    }

    public class course
    {
        public int id;
        public string title;
    }
}