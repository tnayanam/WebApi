using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace CountingKs
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // suppose you want the controller to hit is foods but the url you want is api/nutrition/foods then do below
            // changing the name of optional parameter {id} - {foodid} needs changes in the controller methods too
            // http://localhost:8901/api/nutrition/foods?includeMeasures=false here this works fine even though it does not match the actual {foodid} as paramter. 
            // why it works is because as soon as you out "?" in the url it knows it has to go to a method as if the URL was just http://localhost:8901/api/nutrition/foods
            // and then whatever is passed after the ? is mapped to the methods thats getting called.
            config.Routes.MapHttpRoute(
               name: "Food",
               routeTemplate: "api/nutrition/foods/{foodid}",
               defaults: new {controller = "foods", foodid = RouteParameter.Optional }
               //constraints: new { foodsid = "/d+"} // this will make sure only the integer requests will make it to the controller
           );
            // here we have made the foodsID to be non optional but id is optional, so if user wants all the measures for a particular food the can skip the id but he obviously needs
            // to provide the foodid
            config.Routes.MapHttpRoute(
             name: "Measures",
             routeTemplate: "api/nutrition/foods/{foodid}/measures/{id}",
             defaults: new { controller = "measures", id = RouteParameter.Optional }
         //constraints: new { foodsid = "/d+"} // this will make sure only the integer requests will make it to the controller
         );
            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
            // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
            // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
            //config.EnableQuerySupport();
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.MediaTypeMappings
.Add(new System.Net.Http.Formatting.RequestHeaderMapping("Accept",
                   "text/html",
                   StringComparison.InvariantCultureIgnoreCase,
                   true,
                   "application/json"));
            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().FirstOrDefault();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}