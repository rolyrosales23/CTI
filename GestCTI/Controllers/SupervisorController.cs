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
    [CustomAuthorize(Roles = "supervisor")]
    public class SupervisorController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetTelephone() {
            return PartialView("_PartialPhone");
        }
    }
}