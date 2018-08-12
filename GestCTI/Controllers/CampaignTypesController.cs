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
    public class CampaignTypesController : Controller
    {
        private DBCTIEntities db = new DBCTIEntities();

        // GET: CampaignTypes
        public ActionResult Index()
        {
            return View(db.CampaignType.ToList());
        }

        // GET: CampaignTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CampaignTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] CampaignType campaignType)
        {
            if (ModelState.IsValid)
            {
                db.CampaignType.Add(campaignType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(campaignType);
        }

        // GET: CampaignTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CampaignType campaignType = db.CampaignType.Find(id);
            if (campaignType == null)
            {
                return HttpNotFound();
            }
            return View(campaignType);
        }

        // POST: CampaignTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] CampaignType campaignType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(campaignType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(campaignType);
        }

        // POST: CampaignTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CampaignType campaignType = db.CampaignType.Find(id);
            db.CampaignType.Remove(campaignType);
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
