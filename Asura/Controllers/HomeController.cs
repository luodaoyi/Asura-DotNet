using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Asura.Database;
using Microsoft.AspNetCore.Mvc;
using Asura.Models;
using Asura.Service;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Asura.Controllers
{
    public class HomeController : Controller
    {
        private SiteConfig Config;
        private AsuraContext db;
        private IMemoryCache _cache;
        private MemoryCacheEntryOptions _cacheEntryOptions;

        public HomeController(IOptions<SiteConfig> option, AsuraContext context, IMemoryCache memoryCache)
        {
            Config = option.Value;
            db = context;
            _cache = memoryCache;
            _cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(
                    TimeSpan.FromMinutes(Config.Blogger.CacheExpireSecont)
                );
        }

        [Route("")]
        [Route("index")]
        [Route("page/{page}")]
        [Route("page/{page}.{ext}")]
        public async Task<IActionResult> Index(int page, string ext)
        {
            if (!string.IsNullOrEmpty(ext))
            {
                if (ext.ToLower() != "html")
                    return NotFound();
            }

            page = page <= 0 ? 1 : page;
            string cacheKey = $"page-{page}";

            ViewData["Blogger"] = Config.Blogger;
            ViewData["Qiniu"] = Config.QiNiu.Domain;
            ViewData["Title"] = $"{Config.Blogger.Btitle} | {Config.Blogger.SubTitle}";
            ViewData["Description"] = $"博客首页，{Config.Blogger.SubTitle}";

            var pageSize = 10;
            var viewModel = new HomeViewModel();
            if (!_cache.TryGetValue(cacheKey, out viewModel))
            {
                viewModel = new HomeViewModel();
                var startRow = (page - 1) * pageSize;
                var query = await db.Articles.Where(w => w.IsDraft == false).OrderByDescending(p => p.CreateTime)
                    .Skip(startRow)
                    .Take(pageSize).ToListAsync();

                viewModel.Articles = query;
                viewModel.Prev = page - 1;
                viewModel.Next = (await db.Articles.Where(w => w.IsDraft == false).CountAsync()) > page * pageSize + 1
                    ? page + 1
                    : 0;
                _cache.Set(cacheKey, viewModel,_cacheEntryOptions);
            }


            return View(viewModel);
        }

        [Route("p/{slug}")]
        [Route("p/{slug}.{ext}")]
        public async Task<IActionResult> Article(string slug, string ext = "html")
        {
            if (!string.IsNullOrEmpty(ext))
            {
                ext = ext.ToLower();
            }
            string cacheKey = $"article-{slug}-{ext}-article";
            string cacheKeyViewModel = $"article-{slug}-{ext}-viewModel";
            var article = new Article();
            
            
            if (ext == "html" || ext == "md")
            {
                if (!_cache.TryGetValue(cacheKey, out article))
                {
                    article = await db.Articles
                        .Include(p => p.SerieArticle).ThenInclude(se => se.Serie)
                        .Include(p => p.TagArticles).ThenInclude(se => se.Tag)
                        .Where(w => (w.IsDraft == false && w.Slug == slug))
                        .SingleOrDefaultAsync();
                    _cache.Set(cacheKey, article, _cacheEntryOptions);
                }
               
            }

            if (article == null) return NotFound();

            // 请求html页面
            if (ext == "html")
            {
                ViewData["Title"] = $"{article.Title} | {Config.Blogger.Btitle}";
                ViewData["Blogger"] = Config.Blogger;
                ViewData["Qiniu"] = Config.QiNiu.Domain;
                ViewData["Description"] = $"{article.Desc}，{Config.Blogger.SubTitle}";
                var viewModel = new ArticleViewModel();

                if (!_cache.TryGetValue(cacheKeyViewModel, out viewModel))
                {
                    viewModel = new ArticleViewModel();
                    
                    List<SerieViewModel> series = new List<SerieViewModel>();
                    foreach (var sa in article.SerieArticle)
                    {
                        var svm = new SerieViewModel
                        {
                            Name = sa.Serie.Name,
                            SerieId = sa.SerieId,
                            Articles = await db.SerieArticles
                                .Include(p => p.Article)
                                .Where(pt => pt.SerieId == sa.SerieId)
                                .Select(pt => new ArticleSlugViewModel
                                {
                                    Slug = pt.Article.Slug,
                                    Title = pt.Article.Title,
                                    CreateTime = pt.Article.CreateTime,
                                })
                                .ToListAsync()
                        };

                        series.Add(svm);
                    }
                    viewModel.Series = series;
                    viewModel.Tags = article.TagArticles.Select(s => s.Tag).ToList();

                    viewModel.Prev = await db.Articles
                        .Where(w => (w.IsDraft == false && w.ArticleId == article.ArticleId - 1))
                        .Select(ar => new ArticleSlugViewModel
                        {
                            Slug = ar.Slug,
                            Title = ar.Title,
                            CreateTime = ar.CreateTime,
                        })
                        .SingleOrDefaultAsync();
                    viewModel.Next = await db.Articles
                        .Where(w => (w.IsDraft == false && w.ArticleId == article.ArticleId + 1))
                        .Select(ar => new ArticleSlugViewModel
                        {
                            Slug = ar.Slug,
                            Title = ar.Title
                        })
                        .SingleOrDefaultAsync();
                    _cache.Set(cacheKeyViewModel, viewModel, _cacheEntryOptions);

                }
                
                
                viewModel.Article = article;
               

                return View(viewModel);
            }

            return NotFound();
        }

        [Route("series")]
        [Route("series.{ext}")]
        public async Task<IActionResult> Series(string ext = "html")
        {
            if (!string.IsNullOrEmpty(ext))
            {
                if (ext.ToLower() != "html")
                    return NotFound();
            }
            string cacheKey = $"Series-{ext}";
            
            ViewData["Blogger"] = Config.Blogger;
            ViewData["Qiniu"] = Config.QiNiu.Domain;
            ViewData["Title"] = $"专题 | {Config.Blogger.Btitle}";
            ViewData["Description"] = $"专题列表，，{Config.Blogger.SubTitle}";
            ViewData["SeriesSubTitle"] = Config.Blogger.SeriesSubTitle;
            
            
            List<SerieViewModel> viewModels = new List<SerieViewModel>();

            if (!_cache.TryGetValue(cacheKey,out viewModels))
            {
                viewModels = new List<SerieViewModel>();
                
                var series = await db.Series
                    .Include(i => i.SerieArticle)
                    .ToListAsync();
                foreach (var serie in series)
                {
                    var svm = new SerieViewModel
                    {
                        Desc = serie.Desc,
                        Name = serie.Name,
                        SerieId = serie.SerieId,
                        Articles = await db.SerieArticles
                            .Include(p => p.Article)
                            .Where(pt => pt.SerieId == serie.SerieId)
                            .Select(pt => new ArticleSlugViewModel
                            {
                                Slug = pt.Article.Slug,
                                Title = pt.Article.Title,
                                CreateTime = pt.Article.CreateTime,
                            })
                            .ToListAsync()
                    };
                    viewModels.Add(svm);
                }
                _cache.Set(cacheKey, viewModels, _cacheEntryOptions);
            }
            
            return View(viewModels);
        }


        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}