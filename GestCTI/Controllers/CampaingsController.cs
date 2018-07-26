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
    public class CampaingsController : Controller
    {
        private DBCTIEntities db = new DBCTIEntities();

        // GET: Campaings
        public ActionResult Index()
        {
            var campaing = db.Campaing.Include(c => c.CampaingType).Include(c => c.Company);
            return View(campaing.ToList());
        }

        // GET: Campaings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Campaing campaing = db.Campaing.Find(id);
            if (campaing == null)
            {
                return HttpNotFound();
            }
            return View(campaing);
        }

        // GET: Campaings/Create
        public ActionResult Create()
        {
            ViewBag.IdType = new SelectList(db.CampaingType, "Id", "Id");
            ViewBag.IdCompany = new SelectList(db.Company, "Id", "Name");
            return View();
        }

        // POST: Campaings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,Name,IdType,IdCompany")] Campaing campaing)
        {
            if (ModelState.IsValid)
            {
                db.Campaing.Add(campaing);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdType = new SelectList(db.CampaingType, "Id", "Id", campaing.IdType);
            ViewBag.IdCompany = new SelectList(db.Company, "Id", "Name", campaing.IdCompany);
            return View(campaing);
        }

        // GET: Campaings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Campaing campaing = db.Campaing.Find(id);
            if (campaing == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdType = new SelectList(db.CampaingType, "Id", "Id", campaing.IdType);
            ViewBag.IdCompany = new SelectList(db.Company, "Id", "Name", campaing.IdCompany);
            return View(campaing);
        }

        // POST: Campaings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,Name,IdType,IdCompany")] Campaing campaing)
        {
            if (ModelState.IsValid)
            {
                db.Entry(campaing).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdType = new SelectList(db.CampaingType, "Id", "Id", campaing.IdType);
            ViewBag.IdCompany = new SelectList(db.Company, "Id", "Name", campaing.IdCompany);
            return View(campaing);
        }

        // GET: Campaings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Campaing campaing = db.Campaing.Find(id);
            if (campaing == null)
            {
                return HttpNotFound();
            }
            return View(campaing);
        }

        // POST: Campaings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Campaing campaing = db.Campaing.Find(id);
            db.Campaing.Remove(campaing);
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
