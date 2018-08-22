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

            db.SaveChanges();
        }

        public JsonResult GetPauseCodesByUser(string username)
        {
            db = new DBCTIEntities();
            var pauses = db.GetPauseCodes(username).ToList();
            var pausesdate = db.UserPauseCodes.Where(p => p.Users.Username == username && p.Date == System.DateTime.Today).ToList();
            for (int i = pauses.Count - 1; i >= 0; i--)
                for (int j = 0; i < pausesdate.Count; j++)
                {
                    if (pausesdate[j].IdPauseCode == pauses[i].Id)
                    {
                        if (pausesdate[j].QuantDailyEvents >= pauses[i].MaxDailyEvents)
                        {
                            pauses.RemoveAt(i);
                        }
                        break;
                    }
                }

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
                pause.IdUser = db.Users.FirstOrDefault(p => p.Username == username).Id;
                pause.IdPauseCode = pausecode;
                pause.QuantDailyEvents = 1;
                pause.Date = System.DateTime.Today;
                db.UserPauseCodes.Add(pause);
            }
            db.SaveChanges();
        }

        public JsonResult GetCampaignsByUser(string username) {
            db = new DBCTIEntities();
            var result = from camp in db.Campaign join skillCamp in db.CampaignSkills on camp.Id equals skillCamp.IdCampaign
                         join skillUser in db.UserSkill on skillCamp.IdSkill equals skillUser.IdSkill
                         join user in db.Users on skillUser.IdUser equals user.Id
                         where user.Username == username
                         select new { camp.Id, camp.Name };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}