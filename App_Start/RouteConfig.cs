using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Web1_abr13
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Alpha",
                url: "Home/Alpha/{nome}",
                defaults: new { controller = "Home", action = "Alpha", nome = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{nome}",
                defaults: new { controller = "Clientes", action = "GetClientes", nome = UrlParameter.Optional }
            );
        }
    }
}
