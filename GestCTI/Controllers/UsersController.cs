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
    public class UsersController : Controller
    {
        private DBCTIEntities db = new DBCTIEntities();

        // GET: Users
        public ActionResult Index()
        {
            var users = db.Users.Include(u => u.Company).Include(u => u.Roles).Include(u => u.UserLocation);
            return View(users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            ViewBag.IdCompany = new SelectList(db.Company, "Id", "Name");
            ViewBag.IdRole = new SelectList(db.Roles, "Id", "Name");
            ViewBag.IdLocation = new SelectList(db.UserLocation, "Id", "Name");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Username,Password,email,FirstName,MiddleName,LastName,IdRole,IdLocation,IdCompany,Active")] Users users)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(users);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdCompany = new SelectList(db.Company, "Id", "Name", users.IdCompany);
            ViewBag.IdRole = new SelectList(db.Roles, "Id", "Name", users.IdRole);
            ViewBag.IdLocation = new SelectList(db.UserLocation, "Id", "Name", users.IdLocation);
            return View(users);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdCompany = new SelectList(db.Company, "Id", "Name", users.IdCompany);
            ViewBag.IdRole = new SelectList(db.Roles, "Id", "Name", users.IdRole);
            ViewBag.IdLocation = new SelectList(db.UserLocation, "Id", "Name", users.IdLocation);
            return View(users);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Username,Password,email,FirstName,MiddleName,LastName,IdRole,IdLocation,IdCompany,Active")] Users users)
        {
            if (ModelState.IsValid)
            {
                db.Entry(users).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdCompany = new SelectList(db.Company, "Id", "Name", users.IdCompany);
            ViewBag.IdRole = new SelectList(db.Roles, "Id", "Name", users.IdRole);
            ViewBag.IdLocation = new SelectList(db.UserLocation, "Id", "Name", users.IdLocation);
            return View(users);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Users users = db.Users.Find(id);
            db.Users.Remove(users);
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
