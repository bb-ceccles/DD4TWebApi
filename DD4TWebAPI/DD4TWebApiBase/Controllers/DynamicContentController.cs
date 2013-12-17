using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DD4T.ContentModel.Factories;
using DD4T.Mvc.Html;
using DD4TWebApiBase.Models;
using DD4TWebApiBase.Services;
using Ninject;

namespace DD4TWebApiBase.Controllers
{
    public class DynamicContentController : ApiController
    {
        public IComponentPresentationRenderer componentPresentationRenderer { get; set; }

        public virtual IPageFactory PageFactory { get; set; }
        public virtual IComponentFactory ComponentFactory { get; set; }
        public IComponentPresentationRenderer ComponentPresentationRenderer { get; set; }

        [Inject]
        public DynamicContentService DynamicContentService { get; set; }

        public ComponentModel Get(string tcmId)
        {
            tcmId = "tcm:" + tcmId;

            //Get content from the broker DB based on the schema
            var result = DynamicContentService.GetDynamicComponent(tcmId);
            if (result != null) return ModelFactory.Create(result);
            return null;
        }

        public List<object> Get(string queryFieldName, string queryText)
        {
            var conditions = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(queryFieldName, queryText)
                };

            //Get content from the broker DB based on the schema
            var result = DynamicContentService.GetComponentBySchemaAndCondition(conditions);
            return result.Select(ModelFactory.CreateStronglyTyped).ToList();
        }

    }
}
