using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Asura.TagHelpers
{
    [HtmlTargetElement("staticFile")]
    public class StaticTagHelper : TagHelper
    {
        static StaticTagHelper()
        {
            Cache = new Dictionary<string, string>();
        }

        public string CurrentDirectory => Directory.GetCurrentDirectory();
        public static Dictionary<string, string> Cache { get; set; }

        [HtmlAttributeName("type")]
        public string TagType { get; set; }

        [HtmlAttributeName("path")]
        public string Path { get; set; }


        [HtmlAttributeName("source")]
        public ModelExpression Source { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (Source != null)
            {
                Path = Source.Model.ToString();
            }
            var str = string.Empty;
            if (!string.IsNullOrEmpty(Path))
            {
                if (!Cache.ContainsKey(Path) ||
                    !Cache.TryGetValue(Path, out str))
                {
                    str = await ReadFile(Path);
                    Cache.TryAdd(Path, str);
                }
            }
            else
            {
                str = await ReadFile(Path);
            }
            output.TagMode = TagMode.StartTagAndEndTag;
            if (TagType == "css")
            {
                output.TagName = "style";
                output.Attributes.SetAttribute("type", "text/css");
                output.Content.SetHtmlContent(str);
            }
            if (TagType == "js")
            {
                output.TagName = "script";
                output.Attributes.SetAttribute("type", "text/javascript");
                output.Content.SetHtmlContent(str);
            }
        }

        private async Task<string> ReadFile(string path)
        {
            path = System.IO.Path.Combine(CurrentDirectory, @"wwwroot", path);
            var result = string.Empty;
            if (File.Exists(path))
            {
                result = await File.ReadAllTextAsync(path);
            }
            return result;
        }
    }
}