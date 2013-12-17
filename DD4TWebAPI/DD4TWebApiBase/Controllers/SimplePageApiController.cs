using System.Web.Http;
using DD4T.ContentModel;
using DD4T.ContentModel.Exceptions;
using DD4T.ContentModel.Factories;
using DD4T.Mvc.Html;
using DD4TWebApiBase.Models;

namespace DD4TWebApiBase.Controllers
{
    public class SimplePageApiController : ApiController
    {
        public IComponentPresentationRenderer componentPresentationRenderer { get; set; }

        public virtual IPageFactory PageFactory { get; set; }
        public virtual IComponentFactory ComponentFactory { get; set; }
        public IComponentPresentationRenderer ComponentPresentationRenderer { get; set; }

        // GET api/values
        public PageModel Get(string pageId)
        {
            return ModelFactory.Create(GetModelForPage(pageId));
        }

        protected IPage GetModelForPage(string PageId)
        {
            IPage page = null;
            if (PageFactory != null)
            {
                if (PageFactory.TryFindPage(string.Format("/{0}", PageId), out page))
                {
                    return page;
                }
            }
            else
                throw new ConfigurationException("No PageFactory configured");

            return page;
        }
    }
}
