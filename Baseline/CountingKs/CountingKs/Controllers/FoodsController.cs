using CountingKs.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace CountingKs.Controllers
{
    public class FoodsController : ApiController
    {
        private ICountingKsRepository _repo;
        public FoodsController(ICountingKsRepository repo)
        {
            _repo = repo;
        }
        public IEnumerable<object> Get()
        {
            var results = _repo.GetAllFoodsWithMeasures()
                .OrderBy(f => f.Description)
                .Take(25)
               .ToList()
               .Select(f => new
               {
                   Description = f.Description,
                   Measures = f.Measures.Select(m =>
                   new
                   {
                       Description = m.Description,
                       Calories = m.Calories
                   })
               });
            return results;
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
 * 
 * 
 */
