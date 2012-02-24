using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace MovieCatalog.Controllers.Extension
{
    public class ButtonPressedAttribute : ActionMethodSelectorAttribute
    {
        public string ButtonName { get; set; }

        public override bool IsValidForRequest( ControllerContext controllerContext, MethodInfo methodInfo )
        {
            var req = controllerContext.RequestContext.HttpContext.Request;
            return req.Form[this.ButtonName] != null;
        }
    }
}