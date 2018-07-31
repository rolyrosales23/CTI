using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using GestCTI.Models;

namespace GestCTI.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Admin()
        {
            string lan = Request.UserLanguages[0];
            return View();
        }
    }
}