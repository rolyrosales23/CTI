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
    public class DispositionCampaignsController : Controller
    {
        private DBCTIEntities db = new DBCTIEntities();

        // GET: DispositionCampaigns
        public ActionResult Index()
        {
            var dispositionCampaigns = db.DispositionCampaigns.Include(d => d.Campaign).Include(d => d.Dispositions);
            return View(dispositionCampaigns.ToList());
        }

        // GET: DispositionCampaigns/Create
        public ActionResult Create()
        {
            ViewBag.IdCampaign = new SelectList(db.Campaign, "Id", "Code");
            ViewBag.IdDisposition = new SelectList(db.Dispositions, "Id", "Name");
            return View();
        }

        // POST: DispositionCampaigns/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,IdDisposition,IdCampaign,Description,Active")] DispositionCampaigns dispositionCampaigns)
        {
            if (ModelState.IsValid)
            {
                if (db.DispositionCampaigns.FirstOrDefault(p => p.IdCampaign == dispositionCampaigns.IdCampaign && p.IdDisposition == dispositionCampaigns.IdDisposition) == null)
                {
                    db.DispositionCampaigns.Add(dispositionCampaigns);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                TempData["errorNoty"] = Resources.Admin.TheDisposition + " " + dispositionCampaigns.Dispositions.Name + " " + Resources.Admin.ExistIntheCampaign + " " + dispositionCampaigns.Campaign.Name;
            }

            ViewBag.IdCampaign = new SelectList(db.Campaign, "Id", "Code", dispositionCampaigns.IdCampaign);
            ViewBag.IdDisposition = new SelectList(db.Dispositions, "Id", "Name", dispositionCampaigns.IdDisposition);
            return View(dispositionCampaigns);
        }

        // GET: DispositionCampaigns/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DispositionCampaigns dispositionCampaigns = db.DispositionCampaigns.Find(id);
            if (dispositionCampaigns == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdCampaign = new SelectList(db.Campaign, "Id", "Code", dispositionCampaigns.IdCampaign);
            ViewBag.IdDisposition = new SelectList(db.Dispositions, "Id", "Name", dispositionCampaigns.IdDisposition);
            return View(dispositionCampaigns);
        }

        // POST: DispositionCampaigns/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IdDisposition,IdCampaign,Description,Active")] DispositionCampaigns dispositionCampaigns)
        {
            if (ModelState.IsValid)
            {
                if (db.DispositionCampaigns.FirstOrDefault(p => p.IdCampaign == dispositionCampaigns.IdCampaign && p.IdDisposition == dispositionCampaigns.IdDisposition) == null)
                {
                    DispositionCampaigns TempdispositionCampaigns = db.DispositionCampaigns.Find(dispositionCampaigns.Id);
                    TempdispositionCampaigns.Description = dispositionCampaigns.Description;
                    TempdispositionCampaigns.Active = dispositionCampaigns.Active;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                TempData["errorNoty"] = Resources.Admin.TheDisposition + " " + dispositionCampaigns.Dispositions.Name + " " + Resources.Admin.ExistIntheCampaign + " " + dispositionCampaigns.Campaign.Name;
            }
            ViewBag.IdCampaign = new SelectList(db.Campaign, "Id", "Code", dispositionCampaigns.IdCampaign);
            ViewBag.IdDisposition = new SelectList(db.Dispositions, "Id", "Name", dispositionCampaigns.IdDisposition);
            return View(dispositionCampaigns);
        }

        // POST: DispositionCampaigns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DispositionCampaigns dispositionCampaigns = db.DispositionCampaigns.Find(id);
            dispositionCampaigns.Active = false;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: DispositionCampaigns/Activate/5
        [HttpPost, ActionName("Activate")]
        [ValidateAntiForgeryToken]
        public ActionResult Activate(int id)
        {
            DispositionCampaigns dispositionCampaigns = db.DispositionCampaigns.Find(id);
            dispositionCampaigns.Active = true;
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
