using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;


namespace GestCTI.Controllers.Auth
{
    public class CustomAuthorize : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                //if not logged, it will work as normal Authorize and redirect to the Login
                base.HandleUnauthorizedRequest(filterContext);
            }
            else
            {
                //logged and wihout the role to access it - redirect to the custom controller action
                if( filterContext.HttpContext.User.IsInRole("admin") )
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Admin", action = "Index" }));
                else if (filterContext.HttpContext.User.IsInRole("agent"))
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Agent", action = "Index" }));
                else if (filterContext.HttpContext.User.IsInRole("supervisor"))
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Supervisor", action = "Index" }));
            }
        }
    }
}