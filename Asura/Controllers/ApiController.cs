using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Asura.Comm;
using Asura.Database;
using Asura.Models;
using Asura.Service;
using Disqus.NET;
using Disqus.NET.Requests;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Asura.Controllers
{
    public class ApiController : Controller
    {
        private SiteConfig Config;
        private AsuraContext db;
        private DisqusApi DisqusApi;

        public ApiController(IOptions<SiteConfig> option, AsuraContext context)
        {
            this.Config = option.Value;
            this.db = context;
            //将服务器上的disqus.com host指向 23.235.33.134 就不用使用代理了
            Disqus.NET.DisqusEndpoints.SetProxy(this.Config.Disqus.ApiDomain);
            this.DisqusApi = new DisqusApi(DisqusAuthMethod.PublicKey, this.Config.Disqus.Publickey);
        }

        [Route("disqus/post-{slug}")]
        [Route("disqus/post-{slug}.{ext}")]
        public async Task<IActionResult> DisqusPosts(string slug, string ext = "html")
        {
            var dcs = new DisqusComments();
            var cursor = HttpContext.Request.Query["cursor"].ToString();

            var request = DisqusThreadListPostsRequest
                .New(DisqusThreadLookupType.Identifier, $"post-{slug}")
                .Cursor(cursor)
                .Forum(Config.Disqus.Shortname)
                .Limit(50);

            CursoredDisqusResponse<IEnumerable<Disqus.NET.Models.DisqusPost>> response = null;
            var reTry = false;
            do
            {
                try
                {
                    response = await DisqusApi.Threads.ListPostsAsync(request);
                    if (response != null) reTry = false;
                }
                catch (DisqusApiException ex)
                {
                    dcs.ErrNo = (int)ex.Code;
                    dcs.ErrMsg = ex.Error;

                    if (ex.Code == DisqusApiResponseCode.MissingOrInvalidArgument)
                    {
                        var article = await db.Articles.Where(w => (w.Slug == slug) && !w.IsDraft)
                            .SingleOrDefaultAsync();
                        if (article == null) return Json(dcs);
                        var createReques = DisqusThreadCreateRequest
                            .New(Config.Disqus.Shortname, article.Title)
                            .Identifier($"post-{slug}");

                        var rep = await DisqusApi.Threads.CreateAsync(DisqusAccessToken.Create(Config.Disqus.Accesstoken), createReques);
                        reTry = true;
                    }

                    return Json(dcs);
                }
            } while (reTry);

            if (null == response)
            {
                dcs.ErrNo = 1;
                dcs.ErrMsg = "调用disqus接口失败!";
                return Json(dcs);
            }

            dcs.ErrNo = (int)response.Code;
            dcs.Data = new DisqusCommentsData();
            if (response.Cursor != null)
            {
                dcs.Data.Next = response.Cursor.Next;
            }
            dcs.Data.Total = response.Response.Count();
            dcs.Data.Comments = new List<DisqusCommentsDetail>();
            foreach (var detail in response.Response)
            {
                if (dcs.Data != null && string.IsNullOrEmpty(dcs.Data.Thread))
                    dcs.Data.Thread = detail.Thread.Id;
                var comm = new DisqusCommentsDetail()
                {
                    Id = detail.Id,
                    CreatedAtStr = CommHelper.ConvertStr(detail.CreatedAt),
                    Message = detail.Message,
                    IsDeleted = detail.IsDeleted
                };
                if (detail.Author != null)
                {
                    comm.Name = detail.Author.Name;
                    comm.Url = detail.Author.ProfileUrl;
                    comm.Avatar = detail.Author.Name;
                }
                if (detail.Parent != null)
                {
                    comm.Parent = detail.Parent.Id;
                }
                dcs.Data.Comments.Add(comm);
            }
            return Json(dcs);
        }


        [Route("disqus/form/post-{param}/")]
        public async Task<IActionResult> DisqusForm(string param)
        {
            if (string.IsNullOrEmpty(param)) return Content("出错啦！！");
            string[] paramsList = param.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (paramsList.Count() < 2 || string.IsNullOrEmpty(paramsList[1]) || string.IsNullOrEmpty(paramsList[0]))
            {
                return Content("出错啦！！");
            }

            var arcticle = await db.Articles.Where(w => !w.IsDraft && w.Slug == paramsList[0]).SingleOrDefaultAsync();

            if(null == arcticle)  return Content("找不到这篇文章啊！！");

            DisqusPagrViewModel viewModel = new DisqusPagrViewModel()
            {
                Title = $"发表评论 | {Config.Blogger.Btitle}",
                ATitle = arcticle.Title,
                Thread = paramsList[1],
                Slug = arcticle.Slug
            };
            return View(viewModel);
        }

    }
}