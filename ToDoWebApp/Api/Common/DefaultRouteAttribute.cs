using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoWebApp.Api.Common
{
    public class DefaultRouteAttribute : Attribute, IRouteTemplateProvider
    {
        public const string BASE_ROUTE = "api/v1/";

        private readonly string route;

        public DefaultRouteAttribute(string route = "[controller]")
        {
            this.route = route;
        }

        public string Template => BASE_ROUTE + route;

        public int? Order { get; set; }

        public string Name { get; set; }
    }
}
