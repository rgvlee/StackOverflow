using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Buffers;

namespace Question60078465.RazorPages.TagHelpers
{
    public class CustomHtmlHelper : HtmlHelper, IHtmlHelper
    {
        public CustomHtmlHelper(IHtmlGenerator htmlGenerator, ICompositeViewEngine viewEngine, IModelMetadataProvider metadataProvider, IViewBufferScope bufferScope, HtmlEncoder htmlEncoder, UrlEncoder urlEncoder) : base(htmlGenerator, viewEngine, metadataProvider, bufferScope, htmlEncoder, urlEncoder) { }

        public IHtmlContent CustomGenerateEditor(ModelExplorer modelExplorer, string htmlFieldName, string templateName, object additionalViewData)
        {
            return GenerateEditor(modelExplorer, htmlFieldName, templateName, additionalViewData);
        }

        protected override IHtmlContent GenerateEditor(ModelExplorer modelExplorer, string htmlFieldName, string templateName, object additionalViewData)
        {
            return base.GenerateEditor(modelExplorer, htmlFieldName, templateName, additionalViewData);
        }
    }
}