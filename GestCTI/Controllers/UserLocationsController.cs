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
    public class UserLocationsController : Controller
    {
        private DBCTIEntities db = new DBCTIEntities();

        // GET: UserLocations
        public ActionResult Index()
        {
            return View(db.UserLocation.ToList());
        }

        // GET: UserLocations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserLocation userLocation = db.UserLocation.Find(id);
            if (userLocation == null)
            {
                return HttpNotFound();
            }
            return View(userLocation);
        }

        // GET: UserLocations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserLocations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] UserLocation userLocation)
        {
            if (ModelState.IsValid)
            {
                db.UserLocation.Add(userLocation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(userLocation);
        }

        // GET: UserLocations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserLocation userLocation = db.UserLocation.Find(id);
            if (userLocation == null)
            {
                return HttpNotFound();
            }
            return View(userLocation);
        }

        // POST: UserLocations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] UserLocation userLocation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userLocation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userLocation);
        }

        // GET: UserLocations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserLocation userLocation = db.UserLocation.Find(id);
            if (userLocation == null)
            {
                return HttpNotFound();
            }
            return View(userLocation);
        }

        // POST: UserLocations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserLocation userLocation = db.UserLocation.Find(id);
            db.UserLocation.Remove(userLocation);
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
