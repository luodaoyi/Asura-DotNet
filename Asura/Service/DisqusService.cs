using System;
using System.Threading.Tasks;
using Asura.Comm;
using Asura.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Asura.Service
{
    public class DisqusService
    {
        private DisqusConfig Config { get; set; }

        public DisqusService(DisqusConfig option)
        {
            this.Config = option;
        }

        public async Task<DisqusPostListResponse> PostListSync(string slug, string cursor)
        {
            if (string.IsNullOrEmpty(Config.Postslist) || string.IsNullOrEmpty(Config.Publickey) || string.IsNullOrEmpty(Config.Shortname))
            return null;

            var url =
                $"{Config.Postslist}?limit=50&api_key={Config.Publickey}&forum={Config.Shortname}&cursor={cursor}&thread:ident=post-{slug}";

            var respStr = await HttpHelper.GetSync(url);
            return string.IsNullOrEmpty(respStr) ? null : JsonConvert.DeserializeObject<DisqusPostListResponse>(respStr);
        }
    }

}