using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using BuildingBlocks.DD4T.MarkupModels;
using DD4T.ContentModel;

namespace DD4TWebApiBase.Models
{
    public static class ModelFactory
    {
        private static string viewModelAssemblyName = ConfigurationManager.AppSettings["DD4T.WebApiBase.ViewModelAssemblyName"];

        private static IDictionary<string, Type> _strongTypes = null;
        private static IDictionary<string, Type> StrongTypes
        {
            get
            {
                if (_strongTypes == null)
                {
                    _strongTypes = new Dictionary<string, Type>();

                    var strongTypes = from t in Assembly.Load(viewModelAssemblyName).GetTypes()
                                      let attributes = t.GetCustomAttributes(typeof(TridionViewModelAttribute), true)
                                      where attributes != null && attributes.Length > 0
                                      select new { StrongType = t, Attribute = attributes[0] };

                    foreach (var s in strongTypes)
                    {
                        _strongTypes.Add(s.StrongType.Name, s.StrongType);
                    }
                }
                return _strongTypes;
            }
        }

        public static PageModel Create(IPage page)
        {
            return new PageModel
                {
                    PageTemplate = page.PageTemplate.Id,
                    Filename = page.Filename,
                    ComponentPresentations = page.ComponentPresentations.Select(Create).ToList()
                };
        }

        public static ComponentPresentationModel Create(IComponentPresentation componentPresentation)
        {
            return new ComponentPresentationModel
                {
                    Template = componentPresentation.ComponentTemplate.Title,
                    Component = Create(componentPresentation.Component)
                };
        }

        public static ComponentModel Create(IComponent component)
        {
            return new ComponentModel
                {
                    Title = component.Title,
                    TcmId = component.Id,
                    Feilds = component.Fields.Select(Create).ToList()
                };
        }

        public static FeildModel Create(KeyValuePair<string, IField> arg)
        {
            return new FeildModel
                {
                    Name = arg.Key,
                    Value = arg.Value.Value,
                    RelatedFeilds = arg.Value.LinkedComponentValues.Select(Create).ToList()
                };
        }

        public static object CreateStronglyTyped(IComponent component)
        {
            object stronglyTypedObject = null;

            string typeName = component.Schema.Title.Replace(" ","") + "ViewModel";
            if (StrongTypes.ContainsKey(typeName))
            {

                stronglyTypedObject = ComponentViewModelBuilder.Build(component,StrongTypes[typeName]);
            }

            return stronglyTypedObject;
        }
    }
}