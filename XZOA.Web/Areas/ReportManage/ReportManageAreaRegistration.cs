﻿using System.Web.Mvc;

namespace XZOA.Web.Areas.ReportManage
{
    public class ReportManageAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "ReportManage";
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
