using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XZOA.Web.Areas.PurchaseManage
{
    public class PurchaseManageAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "PurchaseManage";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                 this.AreaName + "_Default",
                 this.AreaName + "/{controller}/{action}/{id}",
                 new { area = this.AreaName, controller = "Home", action = "Index", id = UrlParameter.Optional },
                 new string[] { "XZOA.Web.Areas." + this.AreaName + ".Controllers" }
           );
        }
    }
}