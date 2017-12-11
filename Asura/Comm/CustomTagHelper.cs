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

namespace Asura.TagHelpers
{
    [HtmlTargetElement("markdown")]
    public class MarkdownTagHelper : TagHelper
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
            var pipeline = new MarkdownPipelineBuilder()
                .UseNoFollowLinks()
                .UseMediaLinks()
                .UseAutoIdentifiers(AutoIdentifierOptions.GitHub)
                .Build();
            var doc = Markdown.Parse(Text, pipeline);

            var headings = doc.Descendants<HeadingBlock>().ToList();

            var lastLevel = 1;
            foreach (var heading in headings)
            {
                if (heading.Level == lastLevel + 1)
                {
                }
//                writer.Write($"](#{heading.GetAttributes().Id})");
            }

            var html = string.Empty;
            using (var writer = new StringWriter())
            {
                var htmlWriter = new HtmlRenderer(writer) {EnableHtmlForInline = true};
                htmlWriter.Render((MarkdownObject) doc);
                html = writer.ToString();
            }
            output.TagName = "div";
            output.Content.SetHtmlContent(html);
            output.TagMode = TagMode.StartTagAndEndTag;
        }

        public static void Render<T>(T model, Func<Action<T>, Action<T>> f)
        {
            Fix(f)(model);
        }

        public static Action<T> Fix<T>(Func<Action<T>, Action<T>> f)
        {
            return x => f(Fix(f))(x);
        }
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