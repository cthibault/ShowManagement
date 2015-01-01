using Microsoft.Practices.Unity;
using Repository.Pattern.DataContext;
using Repository.Pattern.Ef6.Repository;
using Repository.Pattern.Ef6.UnitOfWork;
using Repository.Pattern.Repository;
using Repository.Pattern.UnitOfWork;
using ShowManagement.Entity;
using ShowManagement.Service;
using System.Web.Http;
using Unity.WebApi;

namespace ShowManagent.WebApi
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            
            // register all your components with the container here
            // it is NOT necessary to register your controllers
            
            // e.g. container.RegisterType<ITestService, TestService>();

            container.RegisterType<IShowService, ShowService>(new PerThreadLifetimeManager());
            
            container.RegisterType<IDataContextAsync, ShowManagementDataContext>(new PerThreadLifetimeManager());
            container.RegisterType<IUnitOfWorkAsync, UnitOfWork>(new PerResolveLifetimeManager());
            
            container.RegisterType<IRepositoryProvider, RepositoryProvider>(
                new PerResolveLifetimeManager(),
                new InjectionConstructor(new object[] { new RepositoryFactories() }));

            container.RegisterType<IRepositoryAsync<Show>, Repository<Show>>();
            container.RegisterType<IRepositoryAsync<ShowDownload>, Repository<ShowDownload>>();

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}