﻿using Application;
using Application.BaseManager;
using AutoMapper;

namespace SkyLearn.ContentPreview.Api.Services
{
    public class CoursesContentService : RestService
    {
        private readonly ContentContextPreview _context;
        public CoursesContentService(ContentContextPreview context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
        }
    }
}
