using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Asura.Database;
using Microsoft.AspNetCore.Mvc;
using Asura.Models;
using Asura.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Asura.Controllers
{
    public class HomeController : Controller
    {
        private SiteConfig Config;
        private AsuraContext db;

        public HomeController(IOptions<SiteConfig> option, AsuraContext context)
        {
            Config = option.Value;
            db = context;
        }

        /// <summary>
        /// 首页 、 列表页
        /// </summary>
        /// <param name="page"></param>
        /// <param name="ext"></param>
        /// <returns></returns>
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

            ViewData["Blogger"] = Config.Blogger;
            ViewData["Qiniu"] = Config.QiNiu.Domain;
            ViewData["Title"] = $"{Config.Blogger.Btitle} | {Config.Blogger.SubTitle}";
            ViewData["Description"] = $"博客首页，{Config.Blogger.SubTitle}";

            var pageSize = 10;
            var viewModel = new HomeViewModel();
            var startRow = (page - 1) * pageSize;
            var query = await db.Articles.Where(w => w.IsDraft == false).OrderByDescending(p => p.CreateTime)
                .Skip(startRow)
                .Take(pageSize).ToListAsync();

            viewModel.Articles = query;
            viewModel.Prev = page - 1;
            viewModel.Next = (await db.Articles.Where(w => w.IsDraft == false).CountAsync()) > page * pageSize + 1
                ? page + 1
                : 0;


            return View(viewModel);
        }

        /// <summary>
        /// 文章页
        /// </summary>
        /// <param name="slug"></param>
        /// <param name="ext"></param>
        /// <returns></returns>
        [Route("p/{slug}")]
        [Route("p/{slug}.{ext}")]
        public async Task<IActionResult> Article(string slug, string ext = "html")
        {
            if (!string.IsNullOrEmpty(ext))
            {
                ext = ext.ToLower();
            }

            var article = new Article();


            if (ext == "html" || ext == "md")
            {
                article = await db.Articles
                    .Include(p => p.SerieArticle).ThenInclude(se => se.Serie)
                    .Include(p => p.TagArticles).ThenInclude(se => se.Tag)
                    .Where(w => (w.IsDraft == false && w.Slug == slug))
                    .SingleOrDefaultAsync();
            }

            if (article == null) return NotFound();

            if (ext == "md")
            {
                var footer =
                    $"\n 原文链接：[https://{Config.Blogger.Domain}/p/{slug}.html](https://{Config.Blogger.Domain}/p/{slug}.html)，[前往原文评论 »](https://{Config.Blogger.Domain}/p/{slug}.html#comments)";
                return Content(article.Content + footer);
            }
            // 请求html页面
            if (ext == "html")
            {
                ViewData["Title"] = $"{article.Title} | {Config.Blogger.Btitle}";
                ViewData["Blogger"] = Config.Blogger;
                ViewData["Qiniu"] = Config.QiNiu.Domain;
                ViewData["Description"] = $"{article.Desc}，{Config.Blogger.SubTitle}";
                var viewModel = new ArticleViewModel();

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
                viewModel.Article = article;
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

                return View(viewModel);
            }

            return NotFound();
        }

        /// <summary>
        /// 专题页
        /// </summary>
        /// <param name="ext">url 扩展类型 </param>
        /// <returns></returns>
        [Route("series")]
        [Route("series.{ext}")]
        public async Task<IActionResult> Series(string ext = "html")
        {
            if (!string.IsNullOrEmpty(ext))
            {
                if (ext.ToLower() != "html")
                    return NotFound();
            }
            ViewData["Blogger"] = Config.Blogger;
            ViewData["Qiniu"] = Config.QiNiu.Domain;
            ViewData["Title"] = $"专题 | {Config.Blogger.Btitle}";
            ViewData["Description"] = $"专题列表，，{Config.Blogger.SubTitle}";
            ViewData["SeriesSubTitle"] = Config.Blogger.SeriesSubTitle;
            var viewModels = new List<SerieViewModel>();
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
            return View(viewModels);
        }

        /// <summary>
        /// 归档页
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        [Route("archives")]
        [Route("archives.{ext}")]
        public async Task<IActionResult> Archives(string ext = "html")
        {
            if (!string.IsNullOrEmpty(ext))
            {
                if (ext.ToLower() != "html")
                    return NotFound();
            }
            ViewData["Blogger"] = Config.Blogger;
            ViewData["Qiniu"] = Config.QiNiu.Domain;
            ViewData["Title"] = $"归档 | {Config.Blogger.Btitle}";
            ViewData["Description"] = $"博客归档，{Config.Blogger.SubTitle}";
            ViewData["ArchiveSubTitle"] = Config.Blogger.ArchiveSubTitle;

            var viewModel = await db.Articles
                .Where(a => a.IsDraft == false)
                .GroupBy(c => new
                {
                    Year = c.CreateTime.Year,
                    Month = c.CreateTime.Month
                })
                .Select(c => new ArchivesViewModel
                {
                    Mouth = c.Key.Month,
                    Year = c.Key.Year,
                    Articles = c.Select(avm=>new ArticleSlugViewModel
                    {
                        Slug = avm.Slug,
                        Title = avm.Title,
                        CreateTime = avm.CreateTime
                    }).ToList()
                })
                .OrderByDescending(a=>a.Year)
                .ThenByDescending(a=>a.Mouth)
                .ToListAsync();
            return View(viewModel);
        }
        
        
        /// <summary>
        /// 404页面
        /// </summary>
        /// <returns></returns>
        [Route("error/404")]
        public IActionResult Error404()
        {
            ViewData["Blogger"] = Config.Blogger;
            ViewData["Qiniu"] = Config.QiNiu.Domain;
            ViewData["Description"] = $"迷路了。。，{Config.Blogger.SubTitle}";
            ViewData["ArchiveSubTitle"] = Config.Blogger.ArchiveSubTitle;
            
            return View();
        }
        
        /// <summary>
        /// 其他错误页面
        /// </summary>
        /// <param name="code">错误code</param>
        /// <returns></returns>
        [Route("error/{code:int}")]
        public IActionResult Error(int code)
        {
            ViewData["Blogger"] = Config.Blogger;
            ViewData["Qiniu"] = Config.QiNiu.Domain;
            ViewData["Title"] = $"{code} | {Config.Blogger.Btitle}";
            ViewData["Description"] = $"发生错误了，{Config.Blogger.SubTitle}";
            ViewData["ArchiveSubTitle"] = Config.Blogger.ArchiveSubTitle;
            
            // handle different codes or just return the default error view
            return View();
        }
        
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}