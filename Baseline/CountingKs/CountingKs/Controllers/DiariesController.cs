using CountingKs.Data;
using CountingKs.Models;
using CountingKs.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;

namespace CountingKs.Controllers
{
    public class DiariesController : BaseApiController
    {
        private ICountingKsIdentityService _identityService;

        public DiariesController(ICountingKsRepository repo, ICountingKsIdentityService identityService):base(repo)
        {
            // we could have done below stuff in base controller but the food and mesaure does not need an user so better to do it at this level.
            _identityService = identityService;
        }
        public IEnumerable<DiaryModel> Get()
        {
            var userName = _identityService.CurrentUser;
            var results = TheRepository.GetDiaries(userName)
                .OrderByDescending(d => d.CurrentDate)
                .Take(10)
                .ToList()
                .Select(d => TheModelFactory.Create(d));
            return results;
        }


        // datetime is the primarty keyfor siary that means for one day one user can have only one diary
        // the response for below api could have been a simple DiaryModel but we will not be able to send the status report of various operations.
        // adding httpresonsemessage helps us in that and now the status that we are senging will be get appended to the header where as the reponse will remain
        // as if we had the return type as DirayModel
        public HttpResponseMessage Get(DateTime diaryId)
        {
            var userName = _identityService.CurrentUser;
            var result = TheRepository.GetDiary(userName, diaryId);
            if(result == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            return Request.CreateResponse(HttpStatusCode.OK, TheModelFactory.Create(result));
        }
        public HttpResponseMessage Post([FromBody] DiaryModel model)
        {
            try
            {
                if (TheRepository.GetDiaries(_identityService.CurrentUser).Count(d => d.CurrentDate == model.CurrentDate.Date) > 0)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.Conflict, "A diary already exists for that date");
                }

                var entity = TheModelFactory.Parse(model);
                entity.UserName = _identityService.CurrentUser;

                if (TheRepository.Insert(entity) && TheRepository.SaveAll())
                {
                    return Request.CreateResponse(HttpStatusCode.Created, TheModelFactory.Create(entity));
                }
            }
            catch
            {
                // TODO Add Logging
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        public HttpResponseMessage Delete(DateTime id)
        {
            try
            {
                var entity = TheRepository.GetDiary(_identityService.CurrentUser, id);
                if (entity == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                if (TheRepository.DeleteDiary(entity.Id) && TheRepository.SaveAll())
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch
            {
                // TODO Add Logging
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
    }
}
