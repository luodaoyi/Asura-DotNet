using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Asura.Comm
{
    public static class HttpHelper
    {
        public static async Task<string> GetSync(string url)
        {
            using (var client = new HttpClient())
            {
                var resp= await client.GetAsync(url);
                return resp.StatusCode != HttpStatusCode.OK ? string.Empty : Encoding.UTF8.GetString(await resp.Content.ReadAsByteArrayAsync());
            }
        }
    }
}