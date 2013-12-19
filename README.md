DD4TWebApi
==========

For the latest Innovation Day here at Building Blocks I started to look into exposing DD4T information as a ASP.NET Web API. I was trying to create a standard way of exposing DD4T content through an API that requires as little customization as possible.

I had not worked with ASP.NET Web API before and thought this would be a good starting point.

The solution that I ended up with does three main things.
Exposes the IPage components based on the published pages from DD4T
Exposes IComponents of dynamic components based on there tcmid or meta data fields.
I also started incorporating strongly typed views using the Mark-up models solution designed by Rob Stevenson-Leggett here at building blocks (more on that here  and the code is available on git hub)

I created a new web API solution using the following required nuget packages
-DD4T
-BuildingBlocks.DD4T.MarkupModels
-Ninject

I configured up the project to use DD4T using ninject as described in another blog post by Rob.

Exposing page content 
==========

I started with a very basic new controller and route. 

ASP.WebAPI is similar to MVC with routes set up to controllers. The route configs for ASP.WebAPI are set up in WebApiConfig within the Appstart in the basic template project.

The first route I set up was to enable people to navigate the site using the same urls but with a prefix of /API/. This would then return the serialised IPage components. The route passes the “pageID” through to the controller, this is the page URL. This is almost the same as the DD4T default route that you would be familiar with in a MVC DD4T implementation.

		config.Routes.MapHttpRoute(
            	name: "Page",
            	routeTemplate: "API/{pageId}",
            	defaults: new { controller = "PageApi", pageId = RouteParameter.Optional }
        	);

There is not an action specified as in WebAPI the action is defined by the web verb that you are accessing the URL with.

The PageApi controller had the one method that took in the page url and using the DD4T PageFactory attempted to get the page based on it. As this was to respond to a “Get” request through HTTP then the action method was called “Get”. We are only dealing with a read only API this is all the actions that we need to implement.

		// GET API/values
    	public Page Get(string pageId)
    	{
        	return (Page)GetModelForPage(pageId);
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

I then extended this slightly as the full page XML might not be as useful as an API and we might want a cut down version of the data. I added a new route and a new controller. This time I passed the IComponent in to a new Model Factory. I created basic Models for:
-PageModel
-ComponentPresentationModel
-ComponentModel
-FeildModel

Having a new Create method on the Model Factory that took in the different DD4T component types and returned a new instance of the new view models after populating the various fields.

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

This reduced the amount of data returned for each page dramatically but this is an ongoing task to refine the minimum data required for a page. This was more an exercise in using a view factory model.
Exposing dynamic components
A possibly more useful implementation of a DD4T API is to easily expose dynamic content. I did this in two ways, through a route that defined the tcmid exactly and another that takes in a key value pair of metadata and returns the results.

For this I set up two separate routes in the WebApiConfig

		config.Routes.MapHttpRoute(
			name: "DynamicContent",
			routeTemplate: "API/component/{queryFieldName}/{queryText}",
			defaults: new { controller = "DynamicContent" }
		);

		config.Routes.MapHttpRoute(
			name: "SpecificDynamicContent",
			routeTemplate: "API/component/{tcmId}",
			defaults: new { controller = "DynamicContent" },
			constraints: new { tcmId  = "(\\d*)-(\\d*)-(\\d*)"}
		);

In the Dynamic content controller I had two get methods with different input parameters to match the two routes. The first taking in a string and the second taking in two strings.

This controller was very simple just passing the parameters through to the relevant methods in the dynamic service I created. The result from the dynamic service I passed to the ModelFactory create method before returning just like the page controllers.

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


The dynamic service that I created has two methods one to get a specified tcmid component from the ComponentFactory and another to query the broker DB and get a list of tcm ids and return a list of components.


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
            catch (Exception ex)
            {
                LoggerService.Error(ex.Message);
            }

            return component;
		}

Strongly typed views
==========

So far we have only been dealing with generic typed models for pages and components, from working with DD4T I have learned it is much easier to work with strongly typed views. The next piece of work I did was to extend the ModelFactory so that the components were returned as  strongly typed models. For generating the strongly typed views I used the markup models solution designed by Rob Stevenson-Leggett here at building blocks (more on that here  and the code is available on git hub). He explains his solution much better than I could so please read his blog post on it .

I wanted to have my solution as a self contained add on to projects which would allow an apI to be quickly added to a solution. To get the possible view models that could be used as strongly typed views for a component I needed a list of all the possible view models in a solution. I did this using reflection taking a web API config setting as the name of the project to look in (I have my view models as a separate project). The method generates a dictionary of all models with a “TridionViewModelAttribute”. This is then used as a lookup table based on the schema name when a IComponsne is passed to the ModelFactory.


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


This Create method takes the schema name of the component that has been passed to it and removes the spaces and adds on “ViewModel”. For example we have a “Promo” schema this would relate to the model “PromoViewModel”. This is the convention that I used as it is the convention that I have found most useful when I have been creating view models for a normal DD4T project. This is then looked up in the “StrongTypes” dictionary. If it is found it is passed to the Markup models Build method to return the strongly typed view. As a further extension a new attribute could be added to the view models so that they can be tied to schemas directly.

		public static object Create(IComponent component)
    	{
        	object stronglyTypedObject = null;

        	string typeName = component.Schema.Title.Replace(" ","") + "ViewModel";
        	if (StrongTypes.ContainsKey(typeName))
        	{

            	stronglyTypedObject = ComponentViewModelBuilder.Build(component,StrongTypes[typeName]);
        	}

        	return stronglyTypedObject;
    	}

Summary
==========

I aimed to get a add on solution to add in to an existing DD4T implementation to enable access to the published content through a simple API. It is by no means a finished product but I think I managed to achieve my goal.

I spit all my methods out into a separate project so that it can be plugged into an existing MVC project.

Adding the API to a project is a simple as adding Web API through Nuget. and making a few changes to the application.

In the Global.ascx the Web Api config needs to be called in the DD4TWebApiBase to include the API routes.

		DD4TWebApiBase.App_Start.WebApiConfig.Register(GlobalConfiguration.Configuration);

The web config needs the name of the project with the view models in it specifying.

		<add key="DD4T.WebApiBase.ViewModelAssemblyName" value="ViewModels" />


Also in the Ninject Web Common “CreateKernel” method the “DD4TWebApiNinjectModule” needs to be called to add the services to the kernel that are needed for the APIs functions.

		var kernel = new StandardKernel(new DD4TWebApiNinjectModule(publicationId));

Also as a side note Ninject doesn't by default support WebApi so the following line needs to be also added to the “CreateKernel” method.

		//Support WebAPI
		GlobalConfiguration.Configuration.DependencyResolver = new NinjectResolver(kernel);