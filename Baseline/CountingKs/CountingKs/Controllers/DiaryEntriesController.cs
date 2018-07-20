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

        public object Post(DateTime diaryId, [FromBody]DiaryEntryModel model)
        {
            return 6;
        }
    }
}
