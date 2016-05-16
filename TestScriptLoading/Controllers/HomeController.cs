using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace TestScriptLoading.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        protected internal virtual void PreRender()
        {
            Page page = new Page();
            //System.Type.
            //return page.ClientScript.
            //    ClientScript.GetWebResourceUrl(type, resourceId);
        }

    }
}                                                                                                                                                   