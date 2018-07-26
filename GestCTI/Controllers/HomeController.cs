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
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        public ActionResult Login()
        {
            ViewBag.Message = "Your Login page.";

            return View();
        }


        public JsonResult LogUser(string user, string pass)
        {

            Users usuario = new Users();
            usuario.Username = user;
            usuario.Password = Seguridad.EncryptMD5(pass);
            usuario = UserDAO.VerifyUser(usuario);
            if (usuario != null)
            {
                if (usuario.Active)          // UserState = ACTIVE
                {
                    Session["UserActive"] = usuario;
                    //TrazasDAO.AddTraza
                    return Json(new { msg = "log_ok", Roll = usuario.IdRole }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { msg = "inactive", Roll = usuario.IdRole }, JsonRequestBehavior.AllowGet);
            }
            else return null;
        }

        public JsonResult GetAllRolls() {
            var listRolls = UserDAO.GetAllRolls().ToList();
            return Json(listRolls, JsonRequestBehavior.AllowGet);
        }
    }
}