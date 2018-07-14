using CountingKs.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
// so for creating this api controller, I selected from controllers dropdown
// empty api controller, I did not select the empty asp.net mvc controller.
namespace CountingKs.Controllers
{
    public class FoodsController : ApiController
    {
        public List<Data.Entities.Food> Get()
        {
            var repo = new CountingKsRepository(new CountingKsContext());
            var results = repo.GetAllFoods()
                .OrderBy(f => f.Description)
                .Take(25)
                .ToList();
            return results;
        }
    }
}
