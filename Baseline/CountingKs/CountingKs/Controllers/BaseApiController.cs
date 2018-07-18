﻿using CountingKs.Data;
using CountingKs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CountingKs.Controllers
{
    public abstract class BaseApiController : ApiController
    {
        private ICountingKsRepository _repo;
        private ModelFactory _modelFactory;

        public BaseApiController(ICountingKsRepository repo)
        {
            _repo = repo;
        }
        protected ICountingKsRepository TheRepository
        {
            get
            {
                return _repo;
            }
        }
        public ModelFactory TheModelFactory
        {
            get
            {
                if (_modelFactory == null)
                {
                    _modelFactory = new ModelFactory(this.Request);
                }
                return _modelFactory;
            }
        }
    }
}
