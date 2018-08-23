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
    public class CampaignPauseCodesController : Controller
    {
        private DBCTIEntities db = new DBCTIEntities();

        // GET: CampaignPauseCodes
        public ActionResult Index()
        {
            var campaignPauseCodes = db.CampaignPauseCodes.Include(c => c.Campaign).Include(c => c.PauseCodes);
            return View(campaignPauseCodes.ToList());
        }

        // GET: CampaignPauseCodes/Create
        public ActionResult Create()
        {
            ViewBag.IdCampaign = new SelectList(db.Campaign, "Id", "Code");
            ViewBag.IdPauseCode = new SelectList(db.PauseCodes, "Id", "Name");
            return View();
        }

        // POST: CampaignPauseCodes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,IdCampaign,IdPauseCode,MaxDuration,MaxDailyEvents,AutoReady,Active")] CampaignPauseCodes campaignPauseCodes)
        {
            if (ModelState.IsValid)
            {
                if (db.CampaignPauseCodes.FirstOrDefault(p => p.IdCampaign == campaignPauseCodes.IdCampaign && p.IdPauseCode == campaignPauseCodes.IdPauseCode) == null)
                {
                    db.CampaignPauseCodes.Add(campaignPauseCodes);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                //Notificar: El pauseCode campaignPauseCodes.PauseCodes.Name ya existe en la campaña campaignPauseCodes.Campaign.Name
            }

            ViewBag.IdCampaign = new SelectList(db.Campaign, "Id", "Code", campaignPauseCodes.IdCampaign);
            ViewBag.IdPauseCode = new SelectList(db.PauseCodes, "Id", "Name", campaignPauseCodes.IdPauseCode);
            return View(campaignPauseCodes);
        }

        // GET: CampaignPauseCodes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CampaignPauseCodes campaignPauseCodes = db.CampaignPauseCodes.Find(id);
            if (campaignPauseCodes == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdCampaign = new SelectList(db.Campaign, "Id", "Code", campaignPauseCodes.IdCampaign);
            ViewBag.IdPauseCode = new SelectList(db.PauseCodes, "Id", "Name", campaignPauseCodes.IdPauseCode);
            return View(campaignPauseCodes);
        }

        // POST: CampaignPauseCodes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IdCampaign,IdPauseCode,MaxDuration,MaxDailyEvents,AutoReady,Active")] CampaignPauseCodes campaignPauseCodes)
        {
            if (ModelState.IsValid)
            {
                
                db.Entry(campaignPauseCodes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdCampaign = new SelectList(db.Campaign, "Id", "Code", campaignPauseCodes.IdCampaign);
            ViewBag.IdPauseCode = new SelectList(db.PauseCodes, "Id", "Name", campaignPauseCodes.IdPauseCode);
            return View(campaignPauseCodes);
        }

        // POST: CampaignPauseCodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CampaignPauseCodes campaignPauseCodes = db.CampaignPauseCodes.Find(id);
            campaignPauseCodes.Active = false;
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
