using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace CountingKs.Services
{
    public class CountingKsControllerSelector : DefaultHttpControllerSelector
    {
        private HttpConfiguration _config;

        public CountingKsControllerSelector(HttpConfiguration config) : base(config)
        {
            _config = config;
        }
        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            var controllers = GetControllerMapping(); // this only works when we have separate controller name which is Measure and MeasureV2,
            // if we had used same controller name (and then obvisouly we would have to use separate namespace) then we would have to write our own logic to fetch all the controller available.
            // but in our case we can use tha above standard method. // so it returns all the controller dictionary
            var routeData = request.GetRouteData();

            var controllerName = (string)routeData.Values["controller"]; // get the controller name from route

            HttpControllerDescriptor descriptor;
            if (controllers.TryGetValue(controllerName, out descriptor))
            {
                // var version = GetVersionFromQueryString(request);
                //var version = GetVersionFromHeader(request);
                var version = GetVersionFromAcceptHeader(request);
                var newName = string.Concat(controllerName, "V", version);
                HttpControllerDescriptor versionDescriptor;
                if (controllers.TryGetValue(newName, out versionDescriptor))
                {
                    return versionDescriptor; // line 1
                }
                return descriptor; // line 2
            }
            return null;
        }

        private object GetVersionFromAcceptHeader(HttpRequestMessage request)
        {
            var accept = request.Headers.Accept;

            foreach(var mime in accept)
            {
                if(mime.MediaType == "application/json")
                {
                    var value = mime.Parameters.Where(v => v.Name
                    .Equals("version", StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();
                    return value.Value;
                }
            }
            return "1";
        }

        private object GetVersionFromHeader(HttpRequestMessage request)
        {
            const string HEADER_NAME = "X-CountingKs-Version";
            if (request.Headers.Contains(HEADER_NAME))
            {
                var header = request.Headers.GetValues(HEADER_NAME).FirstOrDefault();
                if (header != null)
                    return header;
            }
            return "1";
        }

        private string GetVersionFromQueryString(HttpRequestMessage request)
        {
            var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
            var version = query["v"];
            if (version != null)
            {
                return version; // line 4
            }
            return "1"; //line 3 
        }
    }
}
//http://localhost:8901/api/nutrition/foods/4479/measures/7269?v=2 => this wil hit line 4 and then line1 because we have measurev2
//  http://localhost:8901/api/nutrition/foods/4479/measures/7269?v=1 => this will hit line 4 and then line 2 because we dont have mesasurev1 so default meausrecontroller will be called
// http://localhost:8901/api/nutrition/foods/4479/measures/7269 // => this wilhit line 3 and line 2