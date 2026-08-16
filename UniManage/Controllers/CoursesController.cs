using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UniManage.Models;

namespace UniManage.Controllers
{
    [Authorize(Roles = "Admin,Lecturer,Student")]
    public class CoursesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Courses
        public ActionResult Index()
        {
            return View(db.Courses.ToList());
        }

        // GET: Courses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        [Authorize(Roles = "Admin")]
        // GET: Courses/Create
        public ActionResult Create()
        {
            var role = db.Roles.FirstOrDefault(r => r.Name == "Lecturer");
            List<ApplicationUser> lecturers = new List<ApplicationUser>();

            if (role != null)
            {
                string lecturerRoleId = role.Id;
                lecturers = db.Users.Where(u => u.Roles.Any(r => r.RoleId == lecturerRoleId)).ToList();
            }

            ViewBag.UserId = new SelectList(lecturers, "Id", "UserName");
            return View();
        }

        // POST: Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "CourseId,CourseName,Description,EnrollmentLimit,Credits")] Course course)
        {
            if (ModelState.IsValid)
            {
                course.UserId = User.Identity.GetUserId();
                db.Courses.Add(course);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(course);
        }

        // GET: Courses/ManagePrerequisites/5
        [Authorize(Roles = "Admin")]
        public ActionResult ManagePrerequisites(int id)
        {
            var course = db.Courses.Find(id);
            if (course == null) return HttpNotFound();

            ViewBag.CourseName = course.CourseName;
            ViewBag.CourseId = id;
            ViewBag.OtherCourses = new SelectList(db.Courses.Where(c => c.CourseId != id), "CourseId", "CourseName");

            var viewModel = db.Prerequisites.Where(p => p.CourseId == id).ToList();
            return View(viewModel);
        }

        // POST: Courses/AddPrerequisite
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult AddPrerequisite(int CourseId, int PrerequisiteCourseId)
        {
            if (CourseId != PrerequisiteCourseId)
            {
                var exists = db.Prerequisites.Any(p => p.CourseId == CourseId && p.PrerequisiteCourseId == PrerequisiteCourseId);
                if (!exists)
                {
                    var pre = new Prerequisite { CourseId = CourseId, PrerequisiteCourseId = PrerequisiteCourseId };
                    db.Prerequisites.Add(pre);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("ManagePrerequisites", new { id = CourseId });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult RemovePrerequisite(int prerequisiteId, int returnCourseId)
        {
            var pre = db.Prerequisites.Find(prerequisiteId);
            if (pre != null)
            {
                db.Prerequisites.Remove(pre);
                db.SaveChanges();
            }
            return RedirectToAction("ManagePrerequisites", new { id = returnCourseId });
        }

        [Authorize(Roles = "Admin")]
        // GET: Courses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Course course = db.Courses.Find(id);
            if (course == null) return HttpNotFound();

            var role = db.Roles.FirstOrDefault(r => r.Name == "Lecturer");
            if (role != null)
            {
                var lecturers = db.Users.Where(u => u.Roles.Any(r => r.RoleId == role.Id)).ToList();
                ViewBag.UserId = new SelectList(lecturers, "Id", "UserName", course.UserId);
            }
            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "CourseId,CourseName,Description,EnrollmentLimit,Credits,UserId")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(course);
        }

        [Authorize(Roles = "Admin")]
        // GET: Courses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Course course = db.Courses.Find(id);
            if (course == null) return HttpNotFound();
            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);
            if (course == null) return HttpNotFound();

            // Check if there are any students enrolled in this course before deleting
            bool hasEnrollments = db.Enrollments.Any(e => e.CourseId == id);

            if (hasEnrollments)
            {
                // Set an error message if the course cannot be deleted
                TempData["ErrorMessage"] = "Cannot delete this course because students are already enrolled in it.";
                return RedirectToAction("Index");
            }

            db.Courses.Remove(course);
            db.SaveChanges();

            TempData["SuccessMessage"] = "Course deleted successfully.";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Reports()
        {
            // 1. Basic Counts (Aggregate Functions)
            ViewBag.CourseCount = db.Courses.Count();

            // --- Student Count එක නිවැරදිව ලබා ගැනීම ---
            // මුලින්ම "Student" කියන Role එක සොයාගන්න
            var studentRole = db.Roles.FirstOrDefault(r => r.Name == "Student");
            if (studentRole != null)
            {
                // එම Role ID එක ඇති Users ලා පමණක් Count කරන්න
                ViewBag.StudentCount = db.Users.Count(u => u.Roles.Any(r => r.RoleId == studentRole.Id));
            }
            else
            {
                // Student Role එකක් නැත්නම් 0 පෙන්වන්න
                ViewBag.StudentCount = 0;
            }
            // ------------------------------------------

            ViewBag.TotalEnrollments = db.Enrollments.Count();
            ViewBag.MessageCount = db.UserMessages.Count();

            // 2. Average Credits (Calculated Logic) - (double?) cast එකක් මගින් null errors මගහරී
            ViewBag.AvgCredits = db.Courses.Any() ? db.Courses.Average(c => (double?)c.Credits) ?? 0 : 0;

            // 3. System Success Analytics (Academic Health)
            ViewBag.PassCount = db.Enrollments.Count(e => e.Grade != "F" && e.Grade != "Pending" && e.Grade != null);
            ViewBag.FailCount = db.Enrollments.Count(e => e.Grade == "F");

            // 4. Enrollment Data for Charts
            var courseData = db.Enrollments
                               .GroupBy(e => e.Course.CourseName)
                               .Select(g => new {
                                   CourseName = g.Key,
                                   Count = g.Count()
                               }).ToList();

            ViewBag.CourseNames = courseData.Select(x => x.CourseName).ToArray();
            ViewBag.EnrollmentCounts = courseData.Select(x => x.Count).ToArray();

            // 5. Grid Report Data
            var reportData = db.Courses.ToList().Select(c => new CourseReportViewModel
            {
                CourseName = c.CourseName,
                StudentCount = db.Enrollments.Count(e => e.CourseId == c.CourseId)
            }).ToList();

            return View(reportData);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}