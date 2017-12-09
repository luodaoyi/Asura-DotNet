using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Asura.Database;
using Microsoft.AspNetCore.Mvc;
using Asura.Models;
using Asura.Service;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Asura.Controllers
{
    public class HomeController : Controller
    {
        public SiteConfig Config;
        public AsuraContext db;

        public HomeController(IOptions<SiteConfig> option, AsuraContext context)
        {
            Config = option.Value;
            db = context;
        }

        [Route("")]
        [Route("index")]
        [Route("{page}.{ext}")]
        [Route("page/{page}")]
        [Route("page/{page}.{ext}")]
        public async Task<IActionResult> Index(int page, string ext)
        {
            if (!string.IsNullOrEmpty(ext))
            {
                if (ext.ToLower() != "html")
                    return NotFound();
            }
            

            var pageSize = 10;
            ViewData["Blogger"] = Config.Blogger;
            ViewData["Qiniu"] = Config.QiNiu.Domain;
            ViewData["Title"] = $"{Config.Blogger.Btitle} | {Config.Blogger.SubTitle}";
            ViewData["Description"] = $"博客首页，{Config.Blogger.SubTitle}";

            var viewModel = new HomeViewModel();
            var startRow = ((page >= 0 ? page : 1) - 1) * pageSize;
            var query = await db.Articles.Where(w => w.IsDraft == false).OrderByDescending(p => p.SortFlag)
                .Skip(startRow)
                .Take(pageSize).ToListAsync();

            viewModel.Articles = query;
            viewModel.Prev = page > 1 ? page - 1 : 0;
            viewModel.Next = (await db.Articles.Where(w => w.IsDraft == false).CountAsync()) > page * pageSize + 1
                ? page + 1
                : 0;

            return View(viewModel);
        }

        [Route("p/{slug}")]
        [Route("p/{slug}.{ext}")]
        public async Task<IActionResult> Article(string slug, string ext)
        {
            ViewData["Type"] = ext;
            return View((object) slug);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}