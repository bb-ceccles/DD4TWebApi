using System.Linq;
using System;
using System.Collections.Generic;
using DD4T.ContentModel;
using DD4T.Factories;
using DD4T.Utils;
using Tridion.ContentDelivery.DynamicContent.Query;

namespace DD4TWebApiBase.Services
{
    public class DynamicContentService
    {
        private ComponentFactory ComponentFactory { get; set; }

        private readonly int _publicationId = ConfigurationHelper.PublicationId;

        public DynamicContentService(ComponentFactory componentFactory)
        {
            ComponentFactory = componentFactory;
        }

        public List<IComponent> GetComponentBySchemaAndCondition(List<KeyValuePair<string, string>> conditions)
        {

            var siteComponents = new List<IComponent>();

            try
            {
                var query = new Query();

                var publicationCriteria = new PublicationCriteria(_publicationId);

                var itemTypeCriteria = new ItemTypeCriteria(16);

                var defaultCriteria = CriteriaFactory.And(itemTypeCriteria, publicationCriteria);

                var conditionCriteriaList = new List<Criteria>();

                Criteria criteria = null;

                foreach (var condition in conditions)
                {
                    conditionCriteriaList.Add(
                        new CustomMetaValueCriteria(
                            new CustomMetaKeyCriteria(condition.Key),
                            condition.Value,
                            Criteria.Equal
                            )
                        );
                }
                var conditionCriteria = CriteriaFactory.Or(conditionCriteriaList.ToArray());

                criteria = CriteriaFactory.And(defaultCriteria, conditionCriteria);

                query.Criteria = criteria;

                String[] itemUrIs = query.ExecuteQuery();

                siteComponents.AddRange(itemUrIs.Select(GetDynamicComponent));

            }
            catch (Exception ex)
            {
                LoggerService.Error(ex.Message);
            }


            return siteComponents;
        }

        public IComponent GetDynamicComponent(string itemUrI)
        {
            IComponent component = null;
            try
            {
                ComponentFactory.TryGetComponent(itemUrI, out component);
            }
            catch (Exception e)
            {
                LoggerService.Error(e.Message);
                return null;
            }

            return component;
        }
    }
}