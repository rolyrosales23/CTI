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
    public class DispositionsController : Controller
    {
        private DBCTIEntities db = new DBCTIEntities();

        // GET: Dispositions
        public ActionResult Index()
        {
            var dispositions = db.Dispositions.Include(d => d.Campaign);
            return View(dispositions.ToList());
        }

        // GET: Dispositions/Create
        public ActionResult Create()
        {
            ViewBag.IdCampaign = new SelectList(db.Campaign, "Id", "Code");
            return View();
        }

        // POST: Dispositions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,IdCampaign")] Dispositions dispositions)
        {
            if (ModelState.IsValid)
            {
                db.Dispositions.Add(dispositions);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            ViewBag.IdCampaign = new SelectList(db.Campaign, "Id", "Code", dispositions.IdCampaign);
            return View(dispositions);
        }

        // GET: Dispositions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dispositions dispositions = db.Dispositions.Find(id);
            if (dispositions == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdCampaign = new SelectList(db.Campaign, "Id", "Code", dispositions.IdCampaign);
            return View(dispositions);
        }

        // POST: Dispositions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,IdCampaign")] Dispositions dispositions)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dispositions).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdCampaign = new SelectList(db.Campaign, "Id", "Code", dispositions.IdCampaign);
            return View(dispositions);
        }

        // POST: Dispositions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Dispositions dispositions = db.Dispositions.Find(id);
            db.Dispositions.Remove(dispositions);
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
