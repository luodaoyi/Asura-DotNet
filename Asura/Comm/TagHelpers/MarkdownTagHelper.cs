using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Asura.TagHelpers
{
    //public delegate void SelfApplicable<T>(SelfApplicable<T> self, T arg);
    public delegate Task SelfApplicable<T>(SelfApplicable<T> self, T arg);

    public class Headnav
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ParentId { get; set; }
        public int Level { get; set; }
        public List<Headnav> Child { get; set; }
    }
    
    [HtmlTargetElement("markdown")]
    public class MarkdownTagHelper : TagHelper
    {
        private static async Task Render<T>(T model, SelfApplicable<T> f)
        {
            await f(f, model);
        }

        [HtmlAttributeName("text")]
        public string Text { get; set; }


        [HtmlAttributeName("source")]
        private ModelExpression Source { get; set; }

        /// <summary>
        /// 用于导航的列表
        /// </summary>
        private List<Headnav> NavList { get; set; }

        /// <summary>
        /// 生成分级结构
        /// </summary>
        /// <param name="headings"></param>
        private static List<Headnav> GetNavList(IEnumerable<HeadingBlock> headings)
        {
            var index = 1;
            var list = new List<Headnav>();
            //生成分级列表
            foreach (var heading in headings)
            {
                var newHeadNav = new Headnav
                {
                    Name = heading.GetAttributes().Id,
                    Id = index,
                    Level = heading.Level
                };
                index++;
                if (newHeadNav.Level == 1)
                {
                    list.Add(newHeadNav);
                    continue;
                }
                var parentLevel = list.LastOrDefault(w => w.Level == newHeadNav.Level - 1);
                newHeadNav.ParentId = parentLevel?.Id ?? 1;
                list.Add(newHeadNav);
            }
            return list;
        }

        /// <summary>
        /// 获取树状结构
        /// </summary>
        /// <returns></returns>
        public List<Headnav> GetTree()
        {
            var tops = NavList.Where(w=>w.Level == 1).ToList();
            if (!tops.Any()) return null;
            foreach (var top in tops)
            {
                top.Child = GetChild(top.Id);
            }

            return tops.ToList();
        }

        /// <summary>
        /// 获取子列表
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        private List<Headnav> GetChild(int parentId)
        {
            var query = NavList.Where(w=>w.ParentId == parentId).ToList();
            foreach (var ite in query)
            {
                ite.Child = GetChild(ite.Id);
            }
            return query;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (Source != null)
            {
                Text = Source.Model.ToString();
            }
            //组装markdown 解析处理管道
            var pipeline = new MarkdownPipelineBuilder()
                .UseNoFollowLinks() //连接加nofollow
                .UseMediaLinks() //媒体连接
                .UseAutoIdentifiers(AutoIdentifierOptions.GitHub) //使用github式的header解析
                .Build();
            var doc = Markdown.Parse(Text, pipeline);

            //获取需要生成导航的所有head
            var headings = doc.Descendants<HeadingBlock>().ToList();
            //生成分级结构
            this.NavList = GetNavList(headings);

            //生成树状结构 感谢csdn
            // http://bbs.csdn.net/topics/390112767
            var headnav = GetTree();


            using (var writer = new StringWriter())
            {
                //当树状结构元素大于1的时候 生成html 感谢赵姐夫：
                //http://blog.zhaojie.me/2009/09/rendering-tree-like-structure-recursively.html
                if (headnav != null && headnav.Count > 0)
                {
                    await writer.WriteAsync("<nav id='toc'><p><strong>预览目录</strong></p>");

                    await Render(headnav, async (render, navs) =>
                    {
                        if (navs.Count > 0)
                        {
                            await writer.WriteAsync("<ul>");
                            foreach (var nav in navs)
                            {
                                await writer.WriteAsync("<li>");
                                await writer.WriteAsync($"<a href='#{nav.Name}'>{nav.Name}</a>");
                                await render(render, nav.Child);
                                await writer.WriteAsync(" </li>");
                            }
                            await writer.WriteAsync("</ul>");
                        }
                    });
                    await writer.WriteAsync("</nav>");
                }

                var htmlWriter = new HtmlRenderer(writer) {EnableHtmlForInline = true};
                htmlWriter.Render((MarkdownObject) doc);

                output.Content.SetHtmlContent(writer.ToString());
            }
            output.TagName = "div";

            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }
}