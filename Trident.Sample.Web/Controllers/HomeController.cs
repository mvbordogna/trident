using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Trident.Sample.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
