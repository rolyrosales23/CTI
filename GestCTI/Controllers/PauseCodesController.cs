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
    public class PauseCodesController : Controller
    {
        private DBCTIEntities db = new DBCTIEntities();

        // GET: PauseCodes
        public ActionResult Index()
        {
            return View(db.PauseCodes.ToList());
        }

        // GET: PauseCodes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PauseCodes pauseCodes = db.PauseCodes.Find(id);
            if (pauseCodes == null)
            {
                return HttpNotFound();
            }
            return View(pauseCodes);
        }

        // GET: PauseCodes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PauseCodes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Value,Name,Active")] PauseCodes pauseCodes)
        {
            if (ModelState.IsValid)
            {
                db.PauseCodes.Add(pauseCodes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(pauseCodes);
        }

        // GET: PauseCodes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PauseCodes pauseCodes = db.PauseCodes.Find(id);
            if (pauseCodes == null)
            {
                return HttpNotFound();
            }
            return View(pauseCodes);
        }

        // POST: PauseCodes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Value,Name,Active")] PauseCodes pauseCodes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pauseCodes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pauseCodes);
        }

        // GET: PauseCodes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PauseCodes pauseCodes = db.PauseCodes.Find(id);
            if (pauseCodes == null)
            {
                return HttpNotFound();
            }
            return View(pauseCodes);
        }

        // POST: PauseCodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PauseCodes pauseCodes = db.PauseCodes.Find(id);
            db.PauseCodes.Remove(pauseCodes);
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
