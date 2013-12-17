using System.Collections.Generic;

namespace DD4TWebApiBase.Models
{
    public class PageModel
    {
        public string PageTemplate { get; set; }
        public string Filename { get; set; }

        public List<ComponentPresentationModel> ComponentPresentations { get; set; }
    }
}