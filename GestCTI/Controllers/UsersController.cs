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

            ViewBag.skills = (from s in db.Skills
                              join us in db.UserSkill on s.Id equals us.IdSkill
                              where us.IdUser == id
                              select us).ToList();
            return View(users);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            ViewBag.IdLocation = new SelectList(db.UserLocation, "Id", "Name");
            ViewBag.IdCompany = new SelectList(db.Company, "Id", "Name");
            ViewBag.IdSkill = new MultiSelectList(db.Skills, "Id", "Description");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Username,Password,email,FirstName,MiddleName,LastName,Role,IdLocation,IdCompany,Active")] Users users, int[] IdSkill)
        {
            if (ModelState.IsValid)
            {
                if (db.Users.FirstOrDefault(p => p.Username == users.Username) == null)
                {
                    users.Password = Seguridad.EncryptMD5(users.Password);
                    Users new_user = db.Users.Add(users);
                    db.SaveChanges();
                    if (users.Role == "agent")
                    {
                        if (IdSkill != null)
                        {
                            for (int i = 0; i < IdSkill.Count(); i++)
                            {
                                UserSkill newskill = new UserSkill();
                                newskill.IdSkill = IdSkill[i];
                                newskill.IdUser = new_user.Id;
                                db.UserSkill.Add(newskill);
                            }
                            db.SaveChanges();
                        }
                    }
                    return RedirectToAction("Index");
                }
                else
                    TempData["errorNoty"] = Resources.Admin.ExistUsername;
            }

            ViewBag.IdLocation = new SelectList(db.UserLocation, "Id", "Name", users.IdLocation);
            ViewBag.IdCompany = new SelectList(db.Company, "Id", "Name", users.IdCompany);
            ViewBag.IdSkill = new MultiSelectList(db.Skills, "Id", "Description", IdSkill);
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
            var skills = from s in db.Skills join us in db.UserSkill on s.Id equals us.IdSkill where us.IdUser == id select s.Id;
            ViewBag.IdSkill = new MultiSelectList(db.Skills, "Id", "Description", skills);
            return View(users);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Username,email,FirstName,MiddleName,LastName,Role,IdLocation,IdCompany, IdSkill, Active")] Users users, int[] IdSkill)
        {
            if (ModelState.IsValid)
            {
                if (db.Users.FirstOrDefault(p => p.Username == users.Username && p.Id != users.Id) == null)
                { 
                    Users temp_User = db.Users.Find(users.Id);
                    temp_User.email = users.email;
                    temp_User.IdLocation = users.IdLocation;
                    temp_User.IdCompany = users.IdCompany;
                    if (temp_User.Role != "agent")
                    {
                        temp_User.Username = users.Username;
                        temp_User.FirstName = users.FirstName;
                        temp_User.MiddleName = users.MiddleName;
                        temp_User.LastName = users.LastName;
                        temp_User.Role = users.Role;
                        temp_User.Active = users.Active;
                    }

                    
                    //if (users.Role == "agent")
                    //{
                    //    List<UserSkill> actual = db.UserSkill.Where(p => p.IdUser == users.Id).ToList();
                    //    if (IdSkill != null)
                    //        for (int i = 0; i < IdSkill.Count(); i++)
                    //        {
                    //            if (actual.FirstOrDefault(p => p.IdSkill == IdSkill[i]) != null)
                    //                actual.RemoveAll(p => p.IdSkill == IdSkill[i]);
                    //            else
                    //            {
                    //                UserSkill newskill = new UserSkill();
                    //                newskill.IdSkill = IdSkill[i];
                    //                newskill.IdUser = users.Id;
                    //                db.UserSkill.Add(newskill);
                    //            }
                    //        }
                    //    for (int i = 0; i < actual.Count(); i++)
                    //        db.UserSkill.Remove(actual[i]);
                    //}

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                    TempData["errorNoty"] = Resources.Admin.ExistUsername;
        }
            ViewBag.IdLocation = new SelectList(db.UserLocation, "Id", "Name", users.IdLocation);
            ViewBag.IdCompany = new SelectList(db.Company, "Id", "Name", users.IdCompany);
            ViewBag.IdSkill = new MultiSelectList(db.Skills, "Id", "Description", IdSkill);
            return View(users);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Users users = db.Users.Find(id);
            users.Active = false;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: Users/Activate/5
        [HttpPost, ActionName("Activate")]
        [ValidateAntiForgeryToken]
        public ActionResult Activate(int id)
        {
            Users users = db.Users.Find(id);
            users.Active = true;
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

        // POST: Users/DeleteSkill/5
        [HttpPost, ActionName("DeleteSkill")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteSkillConfirmed(int id)
        {
            UserSkill us = db.UserSkill.Find(id);
            db.UserSkill.Remove(us);
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
