using CountingKs.Data;
using CountingKs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CountingKs.Controllers
{
    public class MeasuresController : BaseApiController
    {

        public MeasuresController(ICountingKsRepository repo):base(repo)
        {
        }

        // this parameter foodid should match whats in the webapi config route.
        public IEnumerable<MeasureModel> Get(int foodid)
        {
            var results = TheRepository.GetMeasuresForFood(foodid)
                .ToList()
                .Select(m => TheModelFactory.Create(m));

            return results;
        }

        public MeasureModel Get(int foodid, int id)
        {
            var results = TheRepository.GetMeasure(id);
            if (results.Food.Id == foodid)
                return TheModelFactory.Create(results);
            else return null;
        }
    }
}
// ok so we wanted to send the url also as a part of the api calls.
// so firstly lets add the property for the URL in both Measure and FoodModel.
// now (HttpRequestMessage request) this is what that contains the URI request so we need to pass it during modelfactory creation
// so we added a constructor in the modelfacotry which will add this url request 
// but the problem is if you see previous checkins we have initialized the Modelfactory in the constructor of the controller which gets called pretty early at that time
// the request is not ready compltetly, so it will not have the URL
// so we need a way to defer the modelfactory creation. only create the model factory when actually its needed and not in the constructor
// so lets create a base controller BaseApiController and move the logic for repo first. Now the base controller will inherit from apicontroller
// and the other controller will NOT inherit directly from APIcontroller but from baseapicontroller
// now wisely we removed the creation of modelfactory from constructor of controller to the basecontroller but as a property so that it gets executed whenever we call it.
// hence its creation is kind of deferred now.
// now when when the actual api function is getting called inside that we will try to instanstite the modelfactory and the request object hence we deferred this creation of request.
// which is what we needed.