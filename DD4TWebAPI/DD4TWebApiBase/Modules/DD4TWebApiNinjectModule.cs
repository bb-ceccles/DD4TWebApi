using DD4T.ContentModel.Contracts.Providers;
using DD4T.ContentModel.Factories;
using DD4T.Factories;
using DD4T.Mvc.Html;
using DD4T.Providers.SDLTridion2013;
using DD4TWebApiBase.Controllers;
using Ninject;
using Ninject.Modules;

namespace DD4TWebApiBase.Modules
{
    ///<summary>
    /// DD4TNinjectModule is responsible for setting up DD4T bindings for Ninject
    /// Author: Robert Stevenson-Leggett
    /// Date: 2012-10-22
    ///</summary>
    public class DD4TWebApiNinjectModule : NinjectModule
    {
        private int _publicationId;

        public DD4TWebApiNinjectModule(int publicationId)
        {
            _publicationId = publicationId;
        }

        public override void Load()
        {
            Bind<IPageProvider>().ToMethod(context => new TridionPageProvider() { PublicationId = _publicationId });
            Bind<ILinkProvider>().To<TridionLinkProvider>();

            Bind<IPageFactory>().ToMethod(context => new PageFactory()
                {
                    PageProvider = context.Kernel.Get<IPageProvider>(),
                    ComponentFactory = context.Kernel.Get<IComponentFactory>(),
                    LinkFactory = context.Kernel.Get<ILinkFactory>()
                });

            Bind<ILinkFactory>().ToMethod(context => new LinkFactory()
                {
                   LinkProvider = context.Kernel.Get<ILinkProvider>()
                });

            Bind<PageApiController>().ToMethod(context => new PageApiController()
                {
                    PageFactory = context.Kernel.Get<IPageFactory>(),
                    ComponentPresentationRenderer = context.Kernel.Get<IComponentPresentationRenderer>()
                });
            Bind<SimplePageApiController>().ToMethod(context => new SimplePageApiController()
                {
                    PageFactory = context.Kernel.Get<IPageFactory>(),
                    ComponentPresentationRenderer = context.Kernel.Get<IComponentPresentationRenderer>()
                });
            Bind<DynamicContentController>().ToMethod(context => new DynamicContentController()
                {
                    PageFactory = context.Kernel.Get<IPageFactory>(),
                    ComponentPresentationRenderer = context.Kernel.Get<IComponentPresentationRenderer>()
                });
            
            Bind<IComponentProvider>().To<TridionComponentProvider>().InSingletonScope();
            Bind<IComponentFactory>().To<ComponentFactory>().InSingletonScope();
            Bind<IComponentPresentationRenderer>().To<DefaultComponentPresentationRenderer>().InSingletonScope();
        }
    }
}