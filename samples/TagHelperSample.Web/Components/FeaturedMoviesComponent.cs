// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Caching;
using Microsoft.Framework.Caching.Memory;
using TagHelperSample.Web.Models;
using TagHelperSample.Web.Services;

namespace TagHelperSample.Web.Components
{
    [ViewComponent(Name = "FeaturedMovies")]
    public class FeaturedMoviesComponent : ViewComponent
    {
        private readonly IMemoryCache _cache;
        private readonly MoviesService _moviesService;

        public FeaturedMoviesComponent(MoviesService moviesService, IMemoryCache cache)
        {
            _moviesService = moviesService;
            _cache = cache;
        }

        public IViewComponentResult Invoke()
        {
            var cacheKey = "featured_movies";
            IEnumerable<FeaturedMovies> movies;
            if (!_cache.TryGetValue(cacheKey, out movies))
            {
                IExpirationTrigger trigger;
                movies = _moviesService.GetFeaturedMovies(out trigger);
                _cache.Set(cacheKey, movies, new MemoryCacheEntryOptions().AddExpirationTrigger(trigger));
            }

            return View(movies);
        }

        public IViewComponentResult Invoke(string movieName)
        {
            string quote;
            if (!_cache.TryGetValue(movieName, out quote))
            {
                IExpirationTrigger trigger;
                quote = _moviesService.GetCriticsQuote(out trigger);
                _cache.Set(movieName, quote, new MemoryCacheEntryOptions().AddExpirationTrigger(trigger));
            }

            return Content(quote);
        }
    }
}