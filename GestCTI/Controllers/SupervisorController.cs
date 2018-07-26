using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using GestCTI.Models;
using GestCTI.DAO;

namespace GestCTI.Controllers
{
    public class SupervisorController : Controller
    {
        public ActionResult Supervisor()
        {
            return View();
        }
    }
}