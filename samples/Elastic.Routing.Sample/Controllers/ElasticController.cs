using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Elastic.Routing.Sample.Controllers
{
    public class ElasticController : Controller
    {
        public ActionResult Page()
        {
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Node1()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            return View();
        }

        protected override ViewResult View(string viewName, string masterName, object model)
        {
            return base.View(viewName ?? "Info", masterName, model);
        }
    }
}
