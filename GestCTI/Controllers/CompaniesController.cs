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
    public class CompaniesController : Controller
    {
        private DBCTIEntities db = new DBCTIEntities();

        // GET: Companies
        public ActionResult Index()
        {
            var company = db.Company.Include(c => c.Switch).Include(c => c.Users);
            return View(company.ToList());
        }

        // GET: Companies/Create
        public ActionResult Create()
        {
            ViewBag.SwitchId = new SelectList(db.Switch, "Id", "Name");
            ViewBag.CreateBy = new SelectList(db.Users, "Id", "Username");
            return View();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,SwitchCompanyId,SwitchId,CreateBy")] Company company)
        {
            if (ModelState.IsValid)
            {
                db.Company.Add(company);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SwitchId = new SelectList(db.Switch, "Id", "Name", company.SwitchId);
            ViewBag.CreateBy = new SelectList(db.Users, "Id", "Username", company.CreateBy);
            return View(company);
        }

        // GET: Companies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Company.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            ViewBag.SwitchId = new SelectList(db.Switch, "Id", "Name", company.SwitchId);
            ViewBag.CreateBy = new SelectList(db.Users, "Id", "Username", company.CreateBy);
            return View(company);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,SwitchCompanyId,SwitchId,CreateBy")] Company company)
        {
            if (ModelState.IsValid)
            {
                db.Entry(company).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SwitchId = new SelectList(db.Switch, "Id", "Name", company.SwitchId);
            ViewBag.CreateBy = new SelectList(db.Users, "Id", "Username", company.CreateBy);
            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Company company = db.Company.Find(id);
            db.Company.Remove(company);
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
