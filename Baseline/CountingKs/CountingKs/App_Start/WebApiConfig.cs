using CacheCow.Server;
using CacheCow.Server.EntityTagStore.SqlServer;
using CountingKs.Convertor;
using CountingKs.Filters;
using CountingKs.Services;
using Newtonsoft.Json.Serialization;
using System;
using System.Configuration;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Dispatcher;

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
           defaults: new { controller = "foods", foodid = RouteParameter.Optional }
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
        config.Routes.MapHttpRoute(
       name: "Diaries",
       routeTemplate: "api/user/diaries/{diaryid}",
       defaults: new { controller = "diaries", diaryid = RouteParameter.Optional }
         );
        config.Routes.MapHttpRoute(
        name: "DiaryEntries",
        routeTemplate: "api/user/diaries/{diaryid}/entries/{id}",
        defaults: new { controller = "diaryentries", id = RouteParameter.Optional }
          );
    config.Routes.MapHttpRoute(
     name: "DiarySummary",
     routeTemplate: "api/user/diaries/{diaryid}/summary",
     defaults: new { controller = "diarysummary" }
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
        jsonFormatter.SerializerSettings.Converters.Add(new LinkModelConvertor());
#if !DEBUG
        // force https
        config.Filters.Add(new RequireHttpsAttribute());
#endif
        // replaces the default controller selector by ours.
        config.Services.Replace(typeof(IHttpControllerSelector),
            new CountingKsControllerSelector(config));
        // cache
        
        var connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        var etagStore = new SqlServerEntityTagStore(connString);
        var cacheHandler = new CachingHandler(etagStore);
        config.MessageHandlers.Add(cacheHandler);

    }
}
// E-Tag
// 1. get the package PM> install-package cachecow.server -version 0.4.6
// theen make the code chnges in this file
// now GO TO fiddler and make the requests
// you will seee an etag returned but it will be with "w/ thius w means it is weak e tag and it is very temporary
// image 1: first request
// image 2: another request if made will return only 304 becuase data is supposed to be already with the client
// so when we are making get or delete we need to add that etag in if-none-match header
// Now we needed to store etag somehwere so first install
// PM> install-package CacheCow.Server.EntityTagStore.SqlServer -version 0.4.1
// then make the code changs and then copy the script which will cvreate the
// database to store the etag execute the script in SQL Server
// located at location:" C:\Users\Dev-10\Desktop\Demo\Webapi\WebApi\Baseline\CountingKs\packages\CacheCow.Server.EntityTagStore.SqlServer.0.4.1\scripts
// and now when you make the request the etag will not jave "w/" because now its persistece