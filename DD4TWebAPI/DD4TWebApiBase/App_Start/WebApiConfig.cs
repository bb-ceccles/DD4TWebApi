using System.Web.Http;

namespace DD4TWebApiBase.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            config.Routes.MapHttpRoute(
                name: "DynamicContent",
                routeTemplate: "api/component/{queryFieldName}/{queryText}",
                defaults: new { controller = "DynamicContent" }
            );

            config.Routes.MapHttpRoute(
                name: "SpecificDynamicContent",
                routeTemplate: "api/component/{tcmId}",
                defaults: new { controller = "DynamicContent" },
                constraints: new { tcmId  = "(\\d*)-(\\d*)-(\\d*)"}
            );

            config.Routes.MapHttpRoute(
                name: "Page",
                routeTemplate: "api/{pageId}",
                defaults: new { controller = "PageApi", pageId = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "SimplePage",
                routeTemplate: "api/simple/{pageId}",
                defaults: new { controller = "SimplePageApi", pageId = RouteParameter.Optional }
            );

            

            var json = config.Formatters.JsonFormatter;
            json.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
            json.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            
        }
    }
}
