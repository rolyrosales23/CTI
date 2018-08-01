using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using GestCTI.Models;
using GestCTI.Controllers.Auth;

namespace GestCTI.Controllers
{
    [CustomAuthorize(Roles = "admin")]
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            string lan = Request.UserLanguages[0];
            return View();
        }
    }
}