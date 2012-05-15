using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using how.web.Models;

namespace how.web.Controllers
{
    public class DoneItController : Controller
    {
        private ModelContext db = new ModelContext();

        //
        // GET: /DoneIt/

        public ActionResult Index()
        {
            var doneits = db.DoneIts.Include(d => d.Goal);
            return View(doneits.ToList());
        }

        //
        // GET: /DoneIt/Details/5

        public ActionResult Details(int id = 0)
        {
            DoneIt doneit = db.DoneIts.Find(id);
            if (doneit == null)
            {
                return HttpNotFound();
            }
            return View(doneit);
        }

        //
        // GET: /DoneIt/Create

        public ActionResult Create()
        {
            ViewBag.Model = new DoneIt { Date = DateTime.Now };
            ViewBag.GoalId = new SelectList(db.Goals, "Id", "Title");
            return View();
        }

        //
        // POST: /DoneIt/Create

        [HttpPost]
        public ActionResult Create(DoneIt doneit)
        {
            if (ModelState.IsValid)
            {
                db.DoneIts.Add(doneit);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.GoalId = new SelectList(db.Goals, "Id", "Title", doneit.GoalId);
            return View(doneit);
        }

        //
        // GET: /DoneIt/Edit/5

        public ActionResult Edit(int id = 0)
        {
            DoneIt doneit = db.DoneIts.Find(id);
            if (doneit == null)
            {
                return HttpNotFound();
            }
            ViewBag.GoalId = new SelectList(db.Goals, "Id", "Title", doneit.GoalId);
            return View(doneit);
        }

        //
        // POST: /DoneIt/Edit/5

        [HttpPost]
        public ActionResult Edit(DoneIt doneit)
        {
            if (ModelState.IsValid)
            {
                db.Entry(doneit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GoalId = new SelectList(db.Goals, "Id", "Title", doneit.GoalId);
            return View(doneit);
        }

        //
        // GET: /DoneIt/Delete/5

        public ActionResult Delete(int id = 0)
        {
            DoneIt doneit = db.DoneIts.Find(id);
            if (doneit == null)
            {
                return HttpNotFound();
            }
            return View(doneit);
        }

        //
        // POST: /DoneIt/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            DoneIt doneit = db.DoneIts.Find(id);
            db.DoneIts.Remove(doneit);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}