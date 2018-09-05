using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GestCTI.Models;
using GestCTI.Controllers.Auth;
using GestCTI.Core.Service;

namespace GestCTI.Controllers
{
    [CustomAuthorize(Roles = "admin")]
    public class CampaignsController : Controller
    {
        private DBCTIEntities db = new DBCTIEntities();

        // GET: Campaigns
        public ActionResult Index()
        {
            var campaign = db.Campaign.Include(c => c.CampaignType).Include(c => c.Company);
            return View(campaign.ToList());
        }

        // GET: Campaigns/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Campaign campaign = db.Campaign.Find(id);
            if (campaign == null)
            {
                return HttpNotFound();
            }
            ViewBag.dispositions = (from dc in db.DispositionCampaigns
                                   join p in db.Dispositions on dc.IdDisposition equals p.Id 
                                   where dc.IdCampaign == id
                                   select dc).ToList();

            ViewBag.pausecodes = (from pc in db.CampaignPauseCodes
                                               join p in db.PauseCodes on pc.IdPauseCode equals p.Id
                                               where pc.IdCampaign == id
                                               select pc).ToList();

            ViewBag.vdns = (from v in db.VDN where v.IdCampaign == id select v).ToList();

            ViewBag.skills = (from s in db.Skills
                              join cs in db.CampaignSkills on s.Id equals cs.IdSkill
                              where cs.IdCampaign == id
                              select cs).ToList();

            return View(campaign);
        }

        // GET: Campaigns/Create
        public ActionResult Create()
        {
            ViewBag.IdType = new SelectList(db.CampaignType, "Id", "Name");
            ViewBag.IdCompany = new SelectList(db.Company, "Id", "Name");
            ViewBag.IdSkill = new MultiSelectList(db.Skills, "Id", "Description");
            return View();
        }

        // POST: Campaigns/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,Name,IdType,UrlScript,IdCompany")] Campaign campaign, int[] IdSkill)
        {
            if (ModelState.IsValid)
            {
                campaign.Active = false;
                Campaign new_campaign = db.Campaign.Add(campaign);
                db.SaveChanges();

                if (IdSkill != null)
                {
                    for (int i = 0; i < IdSkill.Count(); i++)
                    {
                        CampaignSkills newskill = new CampaignSkills();
                        newskill.IdSkill = IdSkill[i];
                        newskill.IdCampaign = new_campaign.Id;
                        db.CampaignSkills.Add(newskill);
                    }
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            ViewBag.IdType = new SelectList(db.CampaignType, "Id", "Name", campaign.IdType);
            ViewBag.IdCompany = new SelectList(db.Company, "Id", "Name", campaign.IdCompany);
            ViewBag.IdSkill = new MultiSelectList(db.Skills, "Id", "Description", IdSkill);
            return View(campaign);
        }

        // GET: Campaigns/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Campaign campaign = db.Campaign.Find(id);
            if (campaign == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdType = new SelectList(db.CampaignType, "Id", "Name", campaign.IdType);
            ViewBag.IdCompany = new SelectList(db.Company, "Id", "Name", campaign.IdCompany);
            var skills = from s in db.Skills join cs in db.CampaignSkills on s.Id equals cs.IdSkill where cs.IdCampaign == id select s.Id;
            ViewBag.IdSkill = new MultiSelectList(db.Skills, "Id", "Description", skills);
            return View(campaign);
        }

        // POST: Campaigns/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,Name,IdType,UrlScript,IdCompany")] Campaign campaign, int[] IdSkill)
        {
            if (ModelState.IsValid)
            {
                db.Entry(campaign).State = EntityState.Modified;

                List<CampaignSkills> actual = db.CampaignSkills.Where(p => p.IdCampaign == campaign.Id).ToList();                
                if (IdSkill != null)
                    for (int i = 0; i < IdSkill.Count(); i++)
                    {
                        if (actual.FirstOrDefault(p => p.IdSkill == IdSkill[i]) != null)
                            actual.RemoveAll(p => p.IdSkill == IdSkill[i]);
                        else
                        {
                            CampaignSkills newskill = new CampaignSkills();
                            newskill.IdSkill = IdSkill[i];
                            newskill.IdCampaign = campaign.Id;
                            db.CampaignSkills.Add(newskill);
                        }
                    }
                for (int i = 0; i < actual.Count(); i++)
                    db.CampaignSkills.Remove(actual[i]);

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdType = new SelectList(db.CampaignType, "Id", "Name", campaign.IdType);
            ViewBag.IdCompany = new SelectList(db.Company, "Id", "Name", campaign.IdCompany);
            ViewBag.IdSkill = new MultiSelectList(db.Skills, "Id", "Description", IdSkill);
            return View(campaign);
        }

        // POST: Campaigns/Start/5
        [HttpPost, ActionName("Start")]
        [ValidateAntiForgeryToken]
        public ActionResult Start(int id)
        {
            Campaign campaign = db.Campaign.Find(id);

            if (campaign.CampaignSkills.Count > 0)
            {
                if (campaign.VDN.Count > 0)
                {
                    if (campaign.CampaignPauseCodes.Count > 0)
                    {
                        if (campaign.DispositionCampaigns.Count > 0)
                        {
                            string url = "http://" + Request.Url.Host;
                            if (ServiceCoreHttp.CampaignStart(id, url, campaign.IdType).Result)
                            {
                                campaign.Active = true;
                                db.SaveChanges();
                                TempData["successNoty"] = Resources.Admin.TheCampaign + " " + campaign.Name + " " + Resources.Admin.StartOk;
                            }
                            else
                                TempData["errorNoty"] = "No se pudo iniciar la campaña " + campaign.Name;
                        }
                        else
                            TempData["errorNoty"] = "Debe asociar Dispositions a esta campaña para poder iniciarla.";
                    }
                    else
                        TempData["errorNoty"] = "Debe asociar Pause Codes a esta campaña para poder iniciarla.";
                }
                else
                    TempData["errorNoty"] = "Debe asociar VDNs a esta campaña para poder iniciarla.";
            }
            else
                TempData["errorNoty"] = "Debe asociar Skills a esta campaña para poder iniciarla.";

            return RedirectToAction("Index");
        }

        // POST: Campaigns/Stop/5
        [HttpPost, ActionName("Stop")]
        [ValidateAntiForgeryToken]
        public ActionResult Stop(int id)
        {
            Campaign campaign = db.Campaign.Find(id);

            if (ServiceCoreHttp.CampaignStop(id).Result)
            {
                campaign.Active = false;
                db.SaveChanges();
                TempData["successNoty"] = "La campaña " + campaign.Name + " ha sido detenida correctamente.";
            }
            else
                TempData["errorNoty"] = "No se pudo detener la campaña " + campaign.Name;
            return RedirectToAction("Index");
        }

        // POST: Users/DeleteSkill/5
        [HttpPost, ActionName("DeleteSkill")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteSkillConfirmed(int id)
        {
            CampaignSkills cs = db.CampaignSkills.Find(id);
            db.CampaignSkills.Remove(cs);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
