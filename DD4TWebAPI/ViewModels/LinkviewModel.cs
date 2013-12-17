using System.Collections.Generic;
using System.Runtime.Serialization;
using BuildingBlocks.DD4T.MarkupModels;
using BuildingBlocks.DD4T.MarkupModels.Nested;

namespace ViewModels
{
    [TridionViewModel(SchemaTitle = "External Link,Internal Link")]
    [KnownType(typeof(LinkViewModel))]
    public class LinkViewModel
    {

        [TextField("title", InlineEditable = false)]
        public string Title { get; set; }

        [TextField("sub_title", InlineEditable = false)]
        public string SubTitle { get; set; }

        [TextField("summary", InlineEditable = false)]
        public string Summary { get; set; }

        [NestedComponent("image", typeof(ImageViewModel), IsMultiValue = true)]
        public IEnumerable<ImageViewModel> Image { get; set; }

        [InternalOrExternalLink("link", InlineEditable = true)]
        public string Link { get; set; }

        [TextField("target", InlineEditable = false)]
        public string Target { get; set; }

        [TextField("css_class", InlineEditable = false)]
        public string CssClass { get; set; }
    }
}
