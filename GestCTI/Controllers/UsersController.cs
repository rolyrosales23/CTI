using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GestCTI.Models;
using GestCTI.Util;
using GestCTI.Controllers.Auth;

namespace GestCTI.Controllers
{
    [CustomAuthorize(Roles = "admin")]
    public class UsersController : Controller
    {
        private DBCTIEntities db = new DBCTIEntities();

        // GET: Users
        public ActionResult Index()
        {
            var users = db.Users.Include(u => u.UserLocation).Include(u => u.Company1);
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
            ViewBag.IdLocation = new SelectList(db.UserLocation, "Id", "Name");
            ViewBag.IdCompany = new SelectList(db.Company, "Id", "Name");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Username,Password,email,FirstName,MiddleName,LastName,Role,IdLocation,IdCompany,Active")] Users users)
        {
            if (ModelState.IsValid)
            {
                users.Password = Seguridad.EncryptMD5(users.Password);
                db.Users.Add(users);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdLocation = new SelectList(db.UserLocation, "Id", "Name", users.IdLocation);
            ViewBag.IdCompany = new SelectList(db.Company, "Id", "Name", users.IdCompany);
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
            ViewBag.IdLocation = new SelectList(db.UserLocation, "Id", "Name", users.IdLocation);
            ViewBag.IdCompany = new SelectList(db.Company, "Id", "Name", users.IdCompany);
            return View(users);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Username,email,FirstName,MiddleName,LastName,Role,IdLocation,IdCompany,Active")] Users users)
        {
            if (ModelState.IsValid)
            {
                Users temp_User = db.Users.Find(users.Id);
                temp_User.Username = users.Username;
                temp_User.email = users.email;
                temp_User.FirstName = users.FirstName;
                temp_User.MiddleName = users.MiddleName;
                temp_User.LastName = users.LastName;
                temp_User.Role = users.Role;
                temp_User.IdLocation = users.IdLocation;
                temp_User.IdCompany = users.IdCompany;
                temp_User.Active = users.Active;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdLocation = new SelectList(db.UserLocation, "Id", "Name", users.IdLocation);
            ViewBag.IdCompany = new SelectList(db.Company, "Id", "Name", users.IdCompany);
            return View(users);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Users users = db.Users.Find(id);
            users.Active = false;
            //db.Users.Remove(users);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Users/ChangePassword/5
        public ActionResult ChangePassword(int? id)
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

        // POST: Users/ChangePassword/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword([Bind(Include = "Id,Username,Password")] Users users)
        {
            if (ModelState.IsValid)
            {
                Users temp_User = db.Users.Find(users.Id);
                temp_User.Password = Seguridad.EncryptMD5(users.Password);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(users);
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
