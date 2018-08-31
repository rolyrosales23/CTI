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

        public void SaveCallDisposition(string ucid, int disposition, string username, string deviceId, string deviceCustomer) {
            db = new DBCTIEntities();
            Calls call = new Calls();
            Users user = db.Users.FirstOrDefault(p => p.Username == username);

            call.ucid = ucid;
            call.IdDispositionCampaign = disposition;
            call.IdAgent = user.Id;
            call.DeviceId = deviceId;
            call.DeviceCustomer = deviceCustomer;
            call.Date = System.DateTime.Now;
            db.Calls.Add(call);

            db.SaveChanges();
        }

        public JsonResult GetPauseCodesByUser(string username)
        {
            db = new DBCTIEntities();
            var pauses = db.GetPauseCodes(username).ToList();
            return Json(pauses, JsonRequestBehavior.AllowGet);
        }

        public void SavePauseCodeUser(string username, int pausecode) {
            db = new DBCTIEntities();
            UserPauseCodes pause = db.UserPauseCodes.FirstOrDefault(p => p.Users.Username == username && p.IdPauseCode == pausecode && p.Date == System.DateTime.Today);
            if (pause != null)
            {
                pause.QuantDailyEvents++;
            }
            else
            {
                UserPauseCodes userpause = new UserPauseCodes();
                Users tempuser = db.Users.FirstOrDefault(p => p.Username == username);
                userpause.IdUser = tempuser.Id;
                userpause.IdPauseCode = pausecode;
                userpause.QuantDailyEvents = 1;
                userpause.Date = System.DateTime.Today;
                db.UserPauseCodes.Add(userpause);
            }
            db.SaveChanges();
        }

        public JsonResult GetCampaignsByUser(string username) {
            db = new DBCTIEntities();
            var result = from p in db.GetCampaigns(username) select new { p.Id, p.Name };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public String GetUrlScriptByVDN(string vdn)
        {
            db = new DBCTIEntities();
            var result = from c in db.Campaign
                         join v in db.VDN on c.Id equals v.IdCampaign
                         where v.Value == vdn
                         select c.UrlScript;

            return result.FirstOrDefault();
        }
    }
}