using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace MovieCatalog.Controllers.Extension
{
    public class AcceptParameterAttribute : ActionMethodSelectorAttribute
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public override bool IsValidForRequest( ControllerContext controllerContext, MethodInfo methodInfo )
        {
            var req = controllerContext.RequestContext.HttpContext.Request;
            return req.Form[this.Name] == this.Value;
        }
    }
}