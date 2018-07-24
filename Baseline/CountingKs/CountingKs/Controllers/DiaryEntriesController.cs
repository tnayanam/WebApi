using CountingKs.Data;
using CountingKs.Models;
using CountingKs.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CountingKs.Controllers
{
    public class DiaryEntriesController : BaseApiController
    {
        private ICountingKsIdentityService _identityService;

        public DiaryEntriesController(ICountingKsRepository repo, ICountingKsIdentityService identityService) : base(repo)
        {
            // we could have done below stuff in base controller but the food and mesaure does not need an user so better to do it at this level.
            _identityService = identityService;
        }

        public IEnumerable<DiaryEntryModel> Get(DateTime diaryId)
        {

            var results = TheRepository.GetDiaryEntries(_identityService.CurrentUser, diaryId.Date)
                .ToList()
                .Select(e => TheModelFactory.Create(e));

            return results;
        }
        public HttpResponseMessage Get(DateTime diaryId, int id)
        {
            // get diary entry for that user. for that day for that entry (id)
            var result = TheRepository.GetDiaryEntry(_identityService.CurrentUser, diaryId.Date, id);

            if (result == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, TheModelFactory.Create(result));
        }

        public HttpResponseMessage Post(DateTime diaryId, [FromBody]DiaryEntryModel model)
        {
            try
            {
                var entity = TheModelFactory.Parse(model);
                if (entity == null)
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could   not  parse the diary entry");
                var diary = TheRepository.GetDiary(_identityService.CurrentUser, diaryId);
                if (diary == null)
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could   not  parse the diary entry");
                if (diary.Entries.Any(e => e.Measure.Id == entity.Measure.Id))
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Duplicate measure not allowed");
                diary.Entries.Add(entity);
                if (!TheRepository.SaveAll())
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not save to the DB");
                }
                return Request.CreateResponse(HttpStatusCode.Created, TheModelFactory.Create(entity));
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
        public HttpResponseMessage Delete(DateTime diaryId, int id)
        {
            try
            {
                if (TheRepository.GetDiaryEntries(_identityService.CurrentUser, diaryId).Any(e => e.Id == id) == false)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                if (TheRepository.DeleteDiaryEntry(id) && TheRepository.SaveAll())
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {

                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
        // patch allows a partial update where as put allows full object to be passed in. here we are updating partially so use patch
        [HttpPut]
        [HttpPatch]
        public HttpResponseMessage Patch(DateTime diaryId, int id, [FromBody] DiaryEntryModel model)
        {
            try
            {
                var entity = TheRepository.GetDiaryEntry(_identityService.CurrentUser, diaryId, id);
                if (entity == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                var parsedValue = TheModelFactory.Parse(model);
                if (parsedValue == null) return Request.CreateResponse(HttpStatusCode.BadRequest);
                if(entity.Quantity != parsedValue.Quantity)
                {
                    entity.Quantity = model.Quantity;
                    if (TheRepository.SaveAll())
                    {
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                }
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {

                return Request.CreateErrorResponse(HttpStatusCode.BadRequest,ex);
            }
        }
    }
}
