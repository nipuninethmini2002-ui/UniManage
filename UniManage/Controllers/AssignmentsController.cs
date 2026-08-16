using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UniManage.Models;

namespace UniManage.Controllers
{
    [Authorize(Roles = "Lecturer, Admin")]
    public class AssignmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Assignments
        public ActionResult Index()
        {
            string currentUserId = User.Identity.GetUserId();
            if (User.IsInRole("Admin"))
            {
                var assignments = db.Assignments.Include(a => a.Course);
                return View(assignments.ToList());
            }
            else
            {
                // Lecturer නම්, තමන්ට අදාළ Courses වල Assignments පමණක් පෙන්වන්න
                var lecturerAssignments = db.Assignments
                                            .Include(a => a.Course)
                                            .Where(a => a.Course.UserId == currentUserId);

                return View(lecturerAssignments.ToList());
            }
        }

        // GET: Assignments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assignment assignment = db.Assignments.Find(id);
            if (assignment == null)
            {
                return HttpNotFound();
            }
            return View(assignment);
        }

        // GET: Assignments/Create
        public ActionResult Create()
        {
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName");
            return View();
        }

        // POST: Assignments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        // 1. මෙතනට 'FilePath' එකතු කළා සහ 'HttpPostedFileBase upload' එකතු කළා
        public ActionResult Create([Bind(Include = "AssignmentId,CourseId,Title,Description,Deadline,FilePath")] Assignment assignment, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                // 2. ෆයිල් එකක් තෝරා ඇත්නම් එය සේව් කරන කොටස
                if (upload != null && upload.ContentLength > 0)
                {
                    // ෆයිල් එකේ නම ගන්නවා
                    var fileName = Path.GetFileName(upload.FileName);

                    // සේව් කරන Path එක (UniManage/Uploads ෆෝල්ඩරයට)
                    var path = Path.Combine(Server.MapPath("~/Uploads"), fileName);

                    // ෆෝල්ඩරය නැත්නම් එක හදනවා (ආරක්ෂාවට)
                    var directory = new DirectoryInfo(Server.MapPath("~/Uploads"));
                    if (!directory.Exists) directory.Create();

                    upload.SaveAs(path);

                    // Database එකේ FilePath තීරුවට නම ඇතුළත් කරනවා
                    assignment.FilePath = fileName;
                }

                db.Assignments.Add(assignment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", assignment.CourseId);
            return View(assignment);
        }

        // GET: Assignments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assignment assignment = db.Assignments.Find(id);
            if (assignment == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", assignment.CourseId);
            return View(assignment);
        }
        // POST: Assignments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AssignmentId,CourseId,Title,Description,Deadline,FilePath")] Assignment assignment, HttpPostedFileBase Uploads)
        {
            if (ModelState.IsValid)
            {
                if (Uploads != null && Uploads.ContentLength > 0)
                {
                    // අලුත් ෆයිල් එකක් අප්ලෝඩ් කරොත්
                    var fileName = Path.GetFileName(Uploads.FileName);
                    var path = Path.Combine(Server.MapPath("~/Uploads"), fileName);

                    if (!Directory.Exists(Server.MapPath("~/Uploads")))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Uploads"));
                    }

                    Uploads.SaveAs(path);
                    assignment.FilePath = fileName; // අලුත් නම දානවා
                }
                // අලුත් ෆයිල් එකක් නැත්නම්, Hidden Field එකෙන් ආපු පරණ නමම ඩේටාබේස් එකට යනවා

                db.Entry(assignment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", assignment.CourseId);
            return View(assignment);
        }

        // GET: Assignments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assignment assignment = db.Assignments.Find(id);
            if (assignment == null)
            {
                return HttpNotFound();
            }
            return View(assignment);
        }

        // POST: Assignments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Assignment assignment = db.Assignments.Find(id);
            db.Assignments.Remove(assignment);
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
