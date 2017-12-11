using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Asura.TagHelpers
{
    public delegate void SelfApplicable<T>(SelfApplicable<T> self, T arg);

    [HtmlTargetElement("markdown")]
    public class MarkdownTagHelper : TagHelper
    {
        public static void Render<T>(T model, SelfApplicable<T> f)
        {
            f(f, model);
        }

        [HtmlAttributeName("text")]
        public string Text { get; set; }

        [HtmlAttributeName("source")]
        public ModelExpression Source { get; set; }

        public List<Headnav> NavList { get; set; }

        /// <summary>
        /// 生成分级结构
        /// </summary>
        /// <param name="headings"></param>
        public List<Headnav> GetNavList(List<HeadingBlock> headings)
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
            var tops = (from c in NavList where c.Level == 1 select c);
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
        /// <param name="p_id"></param>
        /// <returns></returns>
        public List<Headnav> GetChild(int p_id)
        {
            var query = from c in NavList where c.ParentId == p_id select c;
            foreach (var ite in query)
            {
                ite.Child = GetChild(ite.Id);
            }
            return query.ToList();
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (Source != null)
            {
                Text = Source.Model.ToString();
            }
            //组装markdown 解析处理管道
            var pipeline = new MarkdownPipelineBuilder()
                .UseNoFollowLinks()
                .UseMediaLinks()
                .UseAutoIdentifiers(AutoIdentifierOptions.GitHub)
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
                    writer.Write("<nav id='toc'><p><strong>预览目录</strong></p>");

                    Render(headnav, (render, navs) =>
                    {
                        if (navs.Count > 0)
                        {
                            writer.Write("<ul>");
                            foreach (var nav in navs)
                            {
                                writer.Write("<li>");
                                writer.Write($"<a href='#{nav.Name}'>{nav.Name}</a>");
                                render(render, nav.Child);
                                writer.Write(" </li>");
                            }
                            writer.Write("</ul>");
                        }
                    });
                    writer.Write("</nav>");
                }

                var htmlWriter = new HtmlRenderer(writer) {EnableHtmlForInline = true};
                htmlWriter.Render((MarkdownObject) doc);

                output.Content.SetHtmlContent(writer.ToString());
            }
            output.TagName = "div";

            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }

    public class Headnav
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ParentId { get; set; }
        public int Level { get; set; }
        public List<Headnav> Child { get; set; }
    }

    [HtmlTargetElement("markplain")]
    public class String2HtmlTagHelper : TagHelper
    {
        [HtmlAttributeName("text")]
        public string Text { get; set; }

        [HtmlAttributeName("source")]
        public ModelExpression Source { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (Source != null)
            {
                Text = Source.Model.ToString();
            }

            var result = Markdown.ToPlainText(Text);
            output.TagName = "p";
            output.Content.SetHtmlContent(result);
            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }
}