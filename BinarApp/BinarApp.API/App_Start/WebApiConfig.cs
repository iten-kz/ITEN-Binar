using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using BinarApp.API.App_Start;
using BinarApp.API.Models;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Restier.Providers.EntityFramework;
using Microsoft.Restier.Publishers.OData;
using Microsoft.Restier.Publishers.OData.Batch;
using Newtonsoft.Json.Serialization;

namespace BinarApp.API
{
    public static class WebApiConfig
    {
        //public static void Register(HttpConfiguration config)
        //{
        //    // Конфигурация и службы Web API
        //    // Настройка Web API для использования только проверки подлинности посредством маркера-носителя.
        //    config.SuppressDefaultHostAuthentication();
        //    config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

        //    // Маршруты Web API
        //    config.MapHttpAttributeRoutes();

        //    config.Routes.MapHttpRoute(
        //        name: "DefaultApi",
        //        routeTemplate: "api/{controller}/{id}",
        //        defaults: new { id = RouteParameter.Optional }
        //    );
        //}

        public async static void Register(HttpConfiguration config)
        {
            // enable query options for all properties
            //config.Filter().Expand().Select().OrderBy().MaxTop(null).Count();

            //config.EnableCors(new EnableCorsAttribute("*", "*", "*"));

            config.MapHttpAttributeRoutes();

            config.Formatters.XmlFormatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("multipart/form-data"));

            await config.MapRestierRoute<EntityFrameworkApi<BinarContext>>(
                "BinarODATA",
                "odata",
                new RestierBatchHandler(GlobalConfiguration.DefaultServer));

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
