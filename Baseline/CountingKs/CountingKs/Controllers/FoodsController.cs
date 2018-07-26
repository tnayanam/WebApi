using CountingKs.Data;
using CountingKs.Data.Entities;
using CountingKs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Mvc;

namespace CountingKs.Controllers
{
    public class FoodsController : BaseApiController
    {
        public FoodsController(ICountingKsRepository repo):base(repo)
        {
        }
        // we are using parameters to specify behaviour of below methods
        const int PAGE_SIZE = 50;
        public object Get(bool includeMeasures = true, int page = 0)
        {
            IQueryable<Food> query;
            if(includeMeasures)
                query = TheRepository.GetAllFoodsWithMeasures();
            else
                query = TheRepository.GetAllFoods();
            // here we mapped multiple food object which was comiong from backend to food model 
            var baseQuery = query
                .OrderBy(f => f.Description);
            var totalCount = baseQuery.Count();
            var totalPages = Math.Ceiling((double)totalCount / PAGE_SIZE);
            // whenever you want to create a link manually
            var helper = new System.Web.Http.Routing.UrlHelper(Request);
            var prevUrl = page > 0? helper.Link("Food", new { page = page - 1 }):"";
            var nextUrl = page <totalPages -1 ? helper.Link("Food", new { page = page + 1 }): "";

            var results = baseQuery
                .Skip(PAGE_SIZE*page)
                .Take(PAGE_SIZE)
               .ToList()
               .Select(f => TheModelFactory.Create(f));
            return new
            { TotalPages = totalPages,
                TotalCount = totalCount,
                PrevPageUrl = prevUrl,
                NextPageUrl = nextUrl,
                Results = results
                
            };
        }
        public FoodModel Get(int foodid)
        {
            return TheModelFactory.Create(TheRepository.GetFood(foodid)); // we reused the same mapping for one food object which was coming from backend
        }
    }
}

// Steps for Dependency Injection
/*
 * 1. remove the new from any of the controller action
 * 2. in constructor add the parameter of a Interface
 * 3. We will use Ninject here
 * 4. so do below
 * 5. PM> Install-Package Ninject.MVC3 -Version 3.0.0.6
 * 6. Install-Package WebApiContrib.IoC.Ninject -Version 0.9.3.0
 * 7. Now make changes in NinjectWebCommon.cs
 */
