using XZOA.Application.SystemManage;
using XZOA.Code;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System;

namespace XZOA.Web
{
    public class HandlerCookieAttribute : ActionFilterAttribute
    {
        public bool Ignore { get; set; }
        public HandlerCookieAttribute(bool ignore = true)
        {
            Ignore = ignore;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session != null)
            {
                if (filterContext.HttpContext.Session.IsNewSession)
                {
                    var sessionCookie = filterContext.HttpContext.Request.Headers["Cookie"];
                    if ((sessionCookie != null) && (sessionCookie.IndexOf("ASP.NET_SessionId", StringComparison.OrdinalIgnoreCase) >= 0))
                    {
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { Controller = "Login", Action = "Index" }));
                    }
                }
            }
        }
    }
}