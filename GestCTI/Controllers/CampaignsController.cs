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

            return View(campaign);
        }

        // GET: Campaigns/Create
        public ActionResult Create()
        {
            ViewBag.IdType = new SelectList(db.CampaignType, "Id", "Name");
            ViewBag.IdCompany = new SelectList(db.Company, "Id", "Name");
            return View();
        }

        // POST: Campaigns/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,Name,IdType,IdCompany")] Campaign campaign)
        {
            if (ModelState.IsValid)
            {
                db.Campaign.Add(campaign);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdType = new SelectList(db.CampaignType, "Id", "Name", campaign.IdType);
            ViewBag.IdCompany = new SelectList(db.Company, "Id", "Name", campaign.IdCompany);
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
            return View(campaign);
        }

        // POST: Campaigns/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,Name,IdType,IdCompany")] Campaign campaign)
        {
            if (ModelState.IsValid)
            {
                db.Entry(campaign).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdType = new SelectList(db.CampaignType, "Id", "Name", campaign.IdType);
            ViewBag.IdCompany = new SelectList(db.Company, "Id", "Name", campaign.IdCompany);
            return View(campaign);
        }

        // POST: Campaigns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Campaign campaign = db.Campaign.Find(id);
            db.Campaign.Remove(campaign);
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
