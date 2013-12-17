using System.Collections.Generic;
using System.Runtime.Serialization;
using BuildingBlocks.DD4T.MarkupModels;
using BuildingBlocks.DD4T.MarkupModels.Nested;

namespace ViewModels
{
    [TridionViewModel(SchemaTitle = "Image,Park Plan,Floor Plan")]
    [KnownType(typeof(ImageViewModel))]
    public class ImageViewModel
    {
        public string ComponentId { get; set; }

        [MultimediaUrl(InlineEditable = true)]
        public string Url { get; set; }

        //Metadata feilds
        [TextField("title", IsMetadata = true, InlineEditable = false)]
        public string Title { get; set; }

        [TextField("alt", IsMetadata = true, InlineEditable = false)]
        public string AltText { get; set; }

        [TextField("height", IsMetadata = true, InlineEditable = false)]
        public string Height { get; set; }

        [TextField("width", IsMetadata = true, InlineEditable = false)]
        public string Width { get; set; }

        [NestedComponent("thumbnail", typeof(ImageViewModel))]
        public ImageViewModel Thumbnail { get; set; }

        [TextField("category", IsMetadata = true, InlineEditable = false)]
        public string Category { get; set; }

        [NestedComponent("link", typeof(LinkViewModel), IsMultiValue = true)]
        public IEnumerable<LinkViewModel> Link { get; set; }

    }
}
