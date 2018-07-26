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
    public class CampaingTypesController : Controller
    {
        private DBCTIEntities db = new DBCTIEntities();

        // GET: CampaingTypes
        public ActionResult Index()
        {
            return View(db.CampaingType.ToList());
        }

        // GET: CampaingTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CampaingType campaingType = db.CampaingType.Find(id);
            if (campaingType == null)
            {
                return HttpNotFound();
            }
            return View(campaingType);
        }

        // GET: CampaingTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CampaingTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] CampaingType campaingType)
        {
            if (ModelState.IsValid)
            {
                db.CampaingType.Add(campaingType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(campaingType);
        }

        // GET: CampaingTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CampaingType campaingType = db.CampaingType.Find(id);
            if (campaingType == null)
            {
                return HttpNotFound();
            }
            return View(campaingType);
        }

        // POST: CampaingTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] CampaingType campaingType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(campaingType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(campaingType);
        }

        // GET: CampaingTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CampaingType campaingType = db.CampaingType.Find(id);
            if (campaingType == null)
            {
                return HttpNotFound();
            }
            return View(campaingType);
        }

        // POST: CampaingTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CampaingType campaingType = db.CampaingType.Find(id);
            db.CampaingType.Remove(campaingType);
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
