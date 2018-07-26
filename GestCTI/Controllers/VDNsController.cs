using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GestCTI.Models;

namespace GestCTI.Controllers
{
    public class VDNsController : Controller
    {
        private DBCTIEntities db = new DBCTIEntities();

        // GET: VDNs
        public ActionResult Index()
        {
            var vDN = db.VDN.Include(v => v.Campaing);
            return View(vDN.ToList());
        }

        // GET: VDNs/Details/5
        public ActionResult Details(int? id)
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
            return View(vDN);
        }

        // GET: VDNs/Create
        public ActionResult Create()
        {
            ViewBag.IdCampaing = new SelectList(db.Campaing, "Id", "Code");
            return View();
        }

        // POST: VDNs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Value,Description,IdCampaing")] VDN vDN)
        {
            if (ModelState.IsValid)
            {
                db.VDN.Add(vDN);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdCampaing = new SelectList(db.Campaing, "Id", "Code", vDN.IdCampaing);
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
            ViewBag.IdCampaing = new SelectList(db.Campaing, "Id", "Code", vDN.IdCampaing);
            return View(vDN);
        }

        // POST: VDNs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Value,Description,IdCampaing")] VDN vDN)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vDN).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdCampaing = new SelectList(db.Campaing, "Id", "Code", vDN.IdCampaing);
            return View(vDN);
        }

        // GET: VDNs/Delete/5
        public ActionResult Delete(int? id)
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
