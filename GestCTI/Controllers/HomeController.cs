using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using GestCTI.Models;

namespace GestCTI.Controllers
{
    public class HomeController : Controller
    {
        public static DBCTIEntities db;

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
            db = new DBCTIEntities();

            pass = Seguridad.EncryptMD5(pass);
            usuario = db.Users.SingleOrDefault(a => a.Username == user && a.Password == pass);
            if (usuario != null)
            {
                if (usuario.Active)
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
            db = new DBCTIEntities();
            var listRolls = db.Roles.Select(p => new { IdRoll = p.Id, Description = p.Name }).ToList();
            return Json(listRolls, JsonRequestBehavior.AllowGet);
        }
    }
}