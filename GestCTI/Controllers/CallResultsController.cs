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
    public class CallResultsController : Controller
    {
        private DBCTIEntities db = new DBCTIEntities();

        // GET: CallResults
        public ActionResult Index()
        {
            return View(db.CallResult.ToList());
        }

        // GET: CallResults/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CallResults/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,Result")] CallResult callResult)
        {
            if (ModelState.IsValid)
            {
                db.CallResult.Add(callResult);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(callResult);
        }

        // GET: CallResults/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CallResult callResult = db.CallResult.Find(id);
            if (callResult == null)
            {
                return HttpNotFound();
            }
            return View(callResult);
        }

        // POST: CallResults/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,Result")] CallResult callResult)
        {
            if (ModelState.IsValid)
            {
                db.Entry(callResult).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(callResult);
        }

        // POST: CallResults/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CallResult callResult = db.CallResult.Find(id);
            db.CallResult.Remove(callResult);
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
