using System;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Question60078465.MVC.TagHelpers
{
    //https://gist.github.com/vanillajonathan/1f6ae7fb43be41a8c5277b9f5c514192
    [HtmlTargetElement("span", Attributes = AttributeName)]
    public class DescriptionTagHelper : TagHelper
    {
        private const string AttributeName = "asp-description-for";

        /// <summary>
        ///     An expression to be evaluated against the current model.
        /// </summary>
        [HtmlAttributeName(AttributeName)]
        public ModelExpression For { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (!output.IsContentModified)
            {
                output.Content.SetContent(For.Metadata.Description);
            }
        }
    }
}