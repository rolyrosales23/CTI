using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GestCTI.Controllers
{
    public class ScriptsController : Controller
    {
        // GET: Scripts
        public ActionResult Display(String id, String data)
        {
            ViewBag.data = data;
            return View(id, (object) data);
        }
    }
}