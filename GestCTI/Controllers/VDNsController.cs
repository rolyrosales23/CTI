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
    public class VDNsController : Controller
    {
        private DBCTIEntities db = new DBCTIEntities();

        // GET: VDNs
        public ActionResult Index()
        {
            var vDN = db.VDN.Include(v => v.Campaign);
            return View(vDN.ToList());
        }

        // GET: VDNs/Create
        public ActionResult Create()
        {
            ViewBag.IdCampaign = new SelectList(db.Campaign, "Id", "Code");
            return View();
        }

        // POST: VDNs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Value,Description,IdCampaign")] VDN vDN)
        {
            if (ModelState.IsValid)
            {
                db.VDN.Add(vDN);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdCampaign = new SelectList(db.Campaign, "Id", "Code", vDN.IdCampaign);
            return View(vDN);
        }

        // GET: VDNs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VDN vDN = db.VDN.Find(id);
            if (vDN == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdCampaign = new SelectList(db.Campaign, "Id", "Code", vDN.IdCampaign);
            return View(vDN);
        }

        // POST: VDNs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Value,Description,IdCampaign")] VDN vDN)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vDN).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdCampaign = new SelectList(db.Campaign, "Id", "Code", vDN.IdCampaign);
            return View(vDN);
        }

        // POST: VDNs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            VDN vDN = db.VDN.Find(id);
            db.VDN.Remove(vDN);
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
