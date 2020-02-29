using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Question60078465.RazorPages.TagHelpers
{
    [HtmlTargetElement("editor", TagStructure = TagStructure.WithoutEndTag, Attributes = ForAttributeName)]
    public class EditorTagHelper : TagHelper
    {
        private const string ForAttributeName = "asp-for";
        private const string TemplateAttributeName = "asp-template";
        private readonly IHtmlHelper _htmlHelper;

        public EditorTagHelper(IHtmlHelper htmlHelper)
        {
            _htmlHelper = htmlHelper;
        }

        [HtmlAttributeName(ForAttributeName)] public ModelExpression For { get; set; }

        [HtmlAttributeName(TemplateAttributeName)]
        public string Template { get; set; }

        [ViewContext] [HtmlAttributeNotBound] public ViewContext ViewContext { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (output == null)
                throw new ArgumentNullException(nameof(output));

            if (!output.Attributes.ContainsName(nameof(Template)))
            {
                output.Attributes.Add(nameof(Template), Template);
            }

            output.SuppressOutput();

            (_htmlHelper as IViewContextAware).Contextualize(ViewContext);

            var customHtmlHelper = _htmlHelper as CustomHtmlHelper;
            var content = customHtmlHelper.CustomGenerateEditor(For.ModelExplorer, For.Metadata.DisplayName ?? For.Metadata.PropertyName, Template, null);
            output.Content.SetHtmlContent(content);

            await Task.CompletedTask;
        }
    }
}