using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            
            ViewData["Title"] = $"{Config.Blogger.Btitle} | {Config.Blogger.SubTitle}";
            ViewData["Description"] = $"博客首页，{Config.Blogger.SubTitle}";

            ViewData["Qiniu"] = Config.QiNiu.Domain;
            ViewData["Avatar"] = Config.Blogger.Avatar;
            ViewData["Domain"] = Config.Blogger.Domain;
            ViewData["BTitle"] = Config.Blogger.Btitle;
            ViewData["BlogName"] = Config.Blogger.BlogName;
            ViewData["SubTitle"] = Config.Blogger.SubTitle;
            ViewData["Beian"] = Config.Blogger.Beian;
        }

        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [Route("page/{page}")]
        [Route("page/{page}/")]
        public async Task<IActionResult> Index(int page)
        {
            var pageSize = 10;
            
//            ViewData["Path"] = Request.GetUri().AbsolutePath ?? string.Empty;
            ViewData["CurrentPage"] = "blog-home";
                
            var viewModel = new HomeViewModel();
            var startRow = ((page > 0 ? page : 0) - 1) * pageSize;
            var query = await db.Articles.Where(w => w.IsDraft == false).OrderByDescending(p => p.SortFlag).Skip(startRow)
                .Take(pageSize).ToListAsync();

            viewModel.Articles = query;
            viewModel.Prev = page > 0 ? page : 0;
            viewModel.Next = (await db.Articles.Where(w => w.IsDraft == false).CountAsync()) > page + 1 ? page + 1 :0 ;
            
            return View(viewModel);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}