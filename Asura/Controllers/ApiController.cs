using System.Collections.Generic;
using System.Threading.Tasks;
using Asura.Comm;
using Asura.Database;
using Asura.Models;
using Asura.Service;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Asura.Controllers
{
    public class ApiController : Controller
    {
        private SiteConfig Config;
        private AsuraContext db;
        private DisqusService DisqusService;

        public ApiController(IOptions<SiteConfig> option, AsuraContext context)
        {
            this.Config = option.Value;
            this.db = context;
            this.DisqusService = new DisqusService(Config.Disqus);
        }

        [Route("disqus/post-{slug}")]
        [Route("disqus/post-{slug}.{ext}")]
        public async Task<IActionResult> Disqus(string slug, string ext = "html")
        {
            var dcs = new DisqusComments();
            var cursor = HttpContext.Request.Query["cursor"].ToString();
            var postsList = await DisqusService.PostListSync(slug, cursor);

            if (postsList == null)
            {
                dcs.ErrNo = 1;
                dcs.ErrMsg = "系统错误";
            }
            else
            {
                dcs.ErrNo = postsList.Code;
                if (postsList.Cursor != null)
                {
                    dcs.Data.Next = postsList.Cursor.Next;
                }
                dcs.Data.Total = postsList.Response.Count;
                dcs.Data.Comments = new List<DisqusCommentsDetail>();
                foreach (var detail in postsList.Response)
                {
                    if (dcs.Data != null && string.IsNullOrEmpty(dcs.Data.Thread))
                        dcs.Data.Thread = detail.Thread;
                    var comm = new DisqusCommentsDetail()
                    {
                        Id = detail.Id,
                        Name = detail.Author.Name,
                        Parent = detail.Parent,
                        Url = detail.Author.ProfileUrl,
                        Avatar = detail.Author.Avatar.Cache,
                        CreatedAtStr = CommHelper.ConvertStr(detail.CreatedAt),
                        Message = detail.Message,
                        IsDeleted = detail.IsDeleted
                    };
                    dcs.Data.Comments.Add(comm);
                }
            }
            return Json(dcs);
        }
    }
}