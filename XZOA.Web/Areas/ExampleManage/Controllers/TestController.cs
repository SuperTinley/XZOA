using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XZOA.Web.Areas.ExampleManage.Controllers
{
    [HandlerLogin]
    public class TestController : Controller
    {
        // GET: ExampleManage/Test
        public ActionResult Index()
        {
            return View();
        }
    }
}