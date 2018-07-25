﻿using CountingKs.Data;
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
    public class DiarySummaryController : BaseApiController
    {
        private ICountingKsIdentityService _identityService;

        public DiarySummaryController(ICountingKsRepository repo, ICountingKsIdentityService identityService) : base(repo)
        {
            // we could have done below stuff in base controller but the food and mesaure does not need an user so better to do it at this level.
            _identityService = identityService;
        }

        public object Get(DateTime diaryId)
        {
            try
            {
                var diary = TheRepository.GetDiary(_identityService.CurrentUser ,diaryId);
                if (diary == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                return TheModelFactory.CreateSummary(diary);
            }
            catch (Exception ex)
            {

                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
           
        }
    }
}
