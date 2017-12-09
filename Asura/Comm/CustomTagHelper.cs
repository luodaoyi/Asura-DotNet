using System.Threading.Tasks;
using HeyRed.MarkdownSharp;
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
            var markdownTransformer = new Markdown();
            if (Source != null)
            {
                Text = Source.Model.ToString();
            }

            var result = markdownTransformer.Transform(Text);
            output.TagName = "div";
            output.Content.SetHtmlContent(result);
            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }
    
    
    [HtmlTargetElement("string2html")]
    public class String2HtmlTagHelper : TagHelper
    {
        [HtmlAttributeName("text")]
        public string Text { get; set; }

        [HtmlAttributeName("source")]
        public ModelExpression Source { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var markdownTransformer = new Markdown();
            if (Source != null)
            {
                Text = Source.Model.ToString();
            }

            var result = markdownTransformer.Transform(Text);
            output.TagName = "p";
            output.Content.SetHtmlContent(result);
            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }
}