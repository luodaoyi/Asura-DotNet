using System.Collections.Generic;
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
            Disqus.NET.DisqusEndpoints.SetProxy(this.Config.Disqus.ApiDomain);
            this.DisqusApi=new DisqusApi(DisqusAuthMethod.PublicKey,this.Config.Disqus.Publickey);
        }

        [Route("disqus/post-{slug}")]
        [Route("disqus/post-{slug}.{ext}")]
        public async Task<IActionResult> DisqusPosts(string slug, string ext = "html")
        {
            var dcs = new DisqusComments();
            var cursor = HttpContext.Request.Query["cursor"].ToString();

            var request = DisqusThreadListPostsRequest
                .New(DisqusThreadLookupType.Identifier,$"post-{slug}")
                .Cursor(cursor)
                .Forum(Config.Disqus.Shortname)
                .Limit(50);

            CursoredDisqusResponse<IEnumerable<Disqus.NET.Models.DisqusPost>>  response = null; 
            try
            {
                response = await DisqusApi.Threads.ListPostsAsync(request).ConfigureAwait(false);

            }
            catch (DisqusApiException ex)
            {
                dcs.ErrNo = 1;
                dcs.ErrMsg = ex.Error;
                return Json(dcs);
            }
            dcs.ErrNo = (int)response.Code;
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
                    Name = detail.Author.Name,
                    Parent = detail.Parent.Id,
                    Url = detail.Author.ProfileUrl,
                    Avatar = detail.Author.Username,
                    CreatedAtStr = CommHelper.ConvertStr(detail.CreatedAt),
                    Message = detail.Message,
                    IsDeleted = detail.IsDeleted
                };
                dcs.Data.Comments.Add(comm);
            }
            return Json(dcs);
        }
    }
}