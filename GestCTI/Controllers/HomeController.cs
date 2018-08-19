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
    [CustomAuthorize(Roles = "agent")]
    public class HomeController : Controller
    {
        static DBCTIEntities db;
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetDispositionsByVDN(string vdn) {
            db = new DBCTIEntities();
            var result = from p in db.Dispositions join Dispcampaign in db.DispositionCampaigns on p.Id equals Dispcampaign.IdDisposition join campaign in db.Campaign on Dispcampaign.IdCampaign equals campaign.Id join v in db.VDN on campaign.Id equals v.IdCampaign where v.Value == vdn && p.Active && Dispcampaign.Active select new { p.Id, p.Name };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public void SetDisposition(int disposition) {
            db = new DBCTIEntities();
            //guardar datos en db
        }
    }
}