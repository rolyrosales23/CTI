using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GestCTI.Models;

namespace GestCTI.Controllers
{
    [RoutePrefix("API")]
    public class WebServiceController : Controller
    {
        DBCTIEntities db = new DBCTIEntities();

        // Lista de Skills de una campaña
        [Route("CampaignSkills/{id}")]
        public JsonResult SkillsByCampaign(int id)
        {
            var lista = from s in db.Skills
                        join cs in db.CampaignSkills on s.Id equals cs.IdSkill
                        where cs.IdCampaign == id
                        select s.Value;

            return Json(lista.ToList(), JsonRequestBehavior.AllowGet);
        }

        // Lista de VDNs de una campaña
        [Route("CampaignVDNs/{id}")]
        public JsonResult VDNsByCampaign(int id)
        {
            var lista = db.VDN.Where(v => v.IdCampaign == id).Select(v => v.Value);

            return Json(lista.ToList(), JsonRequestBehavior.AllowGet);
        }
    }
}