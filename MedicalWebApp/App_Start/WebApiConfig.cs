using System.Web.Http;
using System.Web.Mvc;
using CGHSBilling.API.AdminPanel.Interfaces;
using CGHSBilling.API.AdminPanel.Repositories;
using CGHSBilling.Resolver;
using Unity;

namespace CGHSBilling
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var container = new UnityContainer();
            container.RegisterType<IUserCreationInterface, UserCreationRepository>();
            container.RegisterType<IUserAccessInterface, UserAccessRepository>();
            container.RegisterType<IClientMasterInterface, ClientMasterRepository>();
            config.DependencyResolver = new UnityResolver(container);
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
            // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
            // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
            //config.EnableQuerySupport();
        }
    }
}