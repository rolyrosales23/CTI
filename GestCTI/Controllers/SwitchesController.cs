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
    public class SwitchesController : Controller
    {
        private DBCTIEntities db = new DBCTIEntities();

        // GET: Switches
        public ActionResult Index()
        {
            return View(db.Switch.ToList());
        }

        // GET: Switches/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Switches/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,WebSocketIP,ApiServerIP")] Switch @switch)
        {
            if (ModelState.IsValid)
            {
                db.Switch.Add(@switch);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(@switch);
        }

        // GET: Switches/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Switch @switch = db.Switch.Find(id);
            if (@switch == null)
            {
                return HttpNotFound();
            }
            return View(@switch);
        }

        // POST: Switches/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,WebSocketIP,ApiServerIP")] Switch @switch)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@switch).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(@switch);
        }

        // POST: Switches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Switch @switch = db.Switch.Find(id);
            db.Switch.Remove(@switch);
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
