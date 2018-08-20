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
            var result = from p in db.Dispositions join Dispcampaign in db.DispositionCampaigns on p.Id equals Dispcampaign.IdDisposition
                         join campaign in db.Campaign on Dispcampaign.IdCampaign equals campaign.Id
                         join v in db.VDN on campaign.Id equals v.IdCampaign
                         where v.Value == vdn && p.Active && Dispcampaign.Active
                         select new { Dispcampaign.Id, p.Name };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public void SetDisposition(string ucid, int disposition, string username, string deviceId, string deviceCustomer) {
            db = new DBCTIEntities();
            Calls call = new Calls();
            Users user = db.Users.FirstOrDefault(p => p.Username == username);

            call.ucid = ucid;
            call.IdDispositionCampaign = disposition;
            call.IdAgent = user.Id;
            call.DeviceId = deviceId;
            call.DeviceCustomer = deviceCustomer;
            call.Date = System.DateTime.Now;

            db.SaveChanges();
        }
    }
}