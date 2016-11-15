using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BYOD_manager.Models;
using PagedList;

namespace BYOD_manager.Controllers
{
    public class BYODController : Controller
    {
        private BYODEntities1 db = new BYODEntities1();

        /// <summary>
        /// Specifies the sort order for the BYOD results. Selecting a menu 
        /// in the program interface will activate each sort case.
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="MISID"></param>
        /// <param name="sortOrder"></param>
        /// <param name="yearLevel"></param>
        /// <returns></returns>
        public ActionResult Index(string firstName, string lastName, string MISID, string sortOrder, string yearLevel, string message)
        {
            int resultsPerPage = 50;
            var students = from s in db.BYODs
                         select s;

            if (!String.IsNullOrEmpty(firstName) || !String.IsNullOrEmpty(lastName) || !String.IsNullOrEmpty(MISID) || !String.IsNullOrEmpty(yearLevel))
            {
                students = students.Where(s => s.Fname.StartsWith(firstName));
                students = students.Where(s => s.Lname.StartsWith(lastName));
                students = students.Where(s => s.MISID.StartsWith(MISID));
                students = students.Where(s => s.YearLevel.Contains(yearLevel));
            }

            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "firstName_desc" : "";
            ViewBag.NameSortParm = sortOrder == "Last Name" ? "lastName_desc" : "Last Name";
            ViewBag.NameSortParm = sortOrder == "MIS ID" ? "MISID_desc" : "MIS ID";
            ViewBag.Message = message;

            students = students.OrderBy(s => s.MISID);

            switch (sortOrder)
            {
                case "firstName_desc":
                    students = students.OrderByDescending(s => s.Fname);
                    break;
                case "lastName_desc":
                    students = students.OrderByDescending(s => s.Lname);
                    break;
                case "MISID_desc":
                    students = students.OrderByDescending(s => s.MISID);
                    break;
                case "approved_desc":
                    students = students.OrderByDescending(s => s.Approved);
                    break;
            }

            return View(students.Take(resultsPerPage));
        }

        // GET: BYOD/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BYOD bYOD = db.BYODs.Find(id);
            if (bYOD == null)
            {
                return HttpNotFound();
            }
            return View(bYOD);
        }

        // GET: BYOD/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BYOD/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EQID,MISID,Fname,Lname,YearLevel,Approved")] BYOD bYOD)
        {
            if (ModelState.IsValid)
            {
                db.BYODs.Add(bYOD);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bYOD);
        }

        // GET: BYOD/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BYOD bYOD = db.BYODs.Find(id);
            if (bYOD == null)
            {
                return HttpNotFound();
            }
            return View(bYOD);
        }

        // POST: BYOD/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EQID,MISID,Fname,Lname,YearLevel,Approved")] BYOD bYOD)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bYOD).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bYOD);
        }

        // GET: BYOD/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BYOD bYOD = db.BYODs.Find(id);
            if (bYOD == null)
            {
                return HttpNotFound();
            }
            return View(bYOD);
        }

        // POST: BYOD/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            BYOD bYOD = db.BYODs.Find(id);
            db.BYODs.Remove(bYOD);
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

        // Changes the Approved status of a Student
        public ActionResult Change(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BYOD bYOD = db.BYODs.Find(id);
            if (bYOD == null)
            {
                return HttpNotFound();
            }
            bYOD.Approved = !bYOD.Approved;
            db.Entry(bYOD).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Creates a file containing the usernames of approved students.
        /// </summary>
        /// <returns></returns>
        public ActionResult Export ()
        {
            int numberOfApproved = 0;
            foreach (var d in db.BYODs)
            {
                if (d.Approved)
                {
                    numberOfApproved++;
                }
            }

            string[] studentMISID = new string[numberOfApproved];

            int i = 0;
            foreach (var d in db.BYODs)
            {
                if (d.Approved)
                {
                    studentMISID[i] = d.MISID;
                    i++;
                }
            }

            // Delete the file if it exists
            if (System.IO.File.Exists("\\10.5.129.2\\byod$\\Allow"))
            {
                System.IO.File.Delete("\\10.5.129.2\\byod$\\Allow");
            }

            System.IO.File.WriteAllLines(@"\\10.5.129.2\byod$\Allow", studentMISID);

            string Message = i + " students have been exported";
            return RedirectToAction("Index", new { message = Message });
        }
    }
}
