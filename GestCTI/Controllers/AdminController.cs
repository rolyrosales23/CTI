using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using GestCTI.Models;
using GestCTI.Controllers.Auth;
using GestCTI.Core.Service;

namespace GestCTI.Controllers
{
    [CustomAuthorize(Roles = "admin")]
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            DBCTIEntities db = new DBCTIEntities();
            ViewBag.CampaingsCount = db.Campaign.Count();
            ViewBag.CampaignTypesCount = db.CampaignType.Count();
            ViewBag.CampaignPauseCodesCount = db.CampaignPauseCodes.Count();
            ViewBag.DispositionCampaignsCount = db.DispositionCampaigns.Count();
            ViewBag.VDNsCount = db.VDN.Count();
            ViewBag.UsersCount = db.Users.Count();
            ViewBag.SkillsCount = db.Skills.Count();
            ViewBag.PauseCodesCount = db.PauseCodes.Count();
            ViewBag.DispositionsCount = db.Dispositions.Count();
            ViewBag.CompaniesCount = db.Company.Count();
            ViewBag.SwitchesCount = db.Switch.Count();
            ViewBag.UserLocationsCount = db.UserLocation.Count();

            return View();
        }

        [HttpPost]
        public void Sincronizar() {
            Sinchronize.sincronizar();
        }
    }
}