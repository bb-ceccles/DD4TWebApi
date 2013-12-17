using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BuildingBlocks.DD4T.MarkupModels;
using BuildingBlocks.DD4T.MarkupModels.Nested;

namespace ViewModels
{
    [TridionViewModel(SchemaTitle = "Promo Item")]
    [KnownType(typeof(PromoItemViewModel))]
    public class PromoItemViewModel
    {
        //Schema feilds
        [TextField("title", InlineEditable = false)]
        public string Title { get; set; }

        [RichTextField("full", InlineEditable = false)]
        public string Full { get; set; }

        [NestedComponent("image", typeof(ImageViewModel))]
        public ImageViewModel Image { get; set; }

        [NestedComponent("secondary_image", typeof(ImageViewModel))]
        public ImageViewModel SecondaryImage { get; set; }

        [NestedComponent("link", typeof(LinkViewModel))]
        public LinkViewModel RelatedLinks { get; set; }

        //Metadata feilds
        [TextField("promo_type", IsMetadata = true, InlineEditable = false)]
        public string PromoType { get; set; }

        [TextField("cta_colour", IsMetadata = true, InlineEditable = false)]
        public string CtaColour { get; set; }

        [TextField("sites", IsMetadata = true, InlineEditable = false, IsMultiValue = true)]
        public IEnumerable<string> Sites { get; set; }

        [TextField("sites_excluded", IsMetadata = true, InlineEditable = false, IsMultiValue = true)]
        public IEnumerable<string> SitesExcluded { get; set; }

        public string PromoCount { get; set; }
    }
}
