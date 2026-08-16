using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using UniManage.Models;

namespace UniManage.Controllers
{
    public class EnrollmentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "Student")]
        // 1. සියලුම විෂයන් පෙන්වීම
        public ActionResult AvailableCourses()
        {
            var courses = db.Courses.ToList();
            return View(courses);
        }

        // 2. ලියාපදිංචි වීමේ Logic එක (GET එක අයින් කර POST එක පමණක් භාවිතා කරමු)
        [HttpPost]
        [Authorize(Roles = "Student")]
        [ValidateAntiForgeryToken]
        public ActionResult Enroll(int courseId)
        {
            string userId = User.Identity.GetUserId();

            // ඒ ඒ පාඨමාලාව ලබා ගැනීම
            var course = db.Courses.Find(courseId);
            if (course == null) return HttpNotFound();

            // A. දැනටමත් මේ විෂයට ලියාපදිංචි වෙලාදැයි බැලීම
            var alreadyEnrolled = db.Enrollments
                .Any(e => e.CourseId == courseId && e.StudentId == userId);

            if (alreadyEnrolled)
            {
                TempData["ErrorMessage"] = "You are already enrolled in this course!";
                return RedirectToAction("AvailableCourses");
            }

            // B. Enrollment Limit එක පරීක්ෂා කිරීම
            var currentEnrollments = db.Enrollments.Count(e => e.CourseId == courseId);
            if (currentEnrollments >= course.EnrollmentLimit)
            {
                TempData["ErrorMessage"] = "Sorry, this course is full!";
                return RedirectToAction("AvailableCourses");
            }

            // C. Prerequisites පරීක්ෂා කිරීම
            var prerequisites = db.Prerequisites
                                  .Where(p => p.CourseId == courseId)
                                  .ToList();

            foreach (var pre in prerequisites)
            {
                // පූර්ව අවශ්‍යතාවය සම්පූර්ණ කර ඇතිදැයි බැලීම (Grade එකක් තිබේ නම් සම්පූර්ණ ලෙස සලකයි)
                var hasCompleted = db.Enrollments.Any(e => e.StudentId == userId &&
                                                           e.CourseId == pre.PrerequisiteCourseId &&
                                                           e.Grade != null && e.Grade != "Pending");

                if (!hasCompleted)
                {
                    var preCourseName = db.Courses.Find(pre.PrerequisiteCourseId).CourseName;
                    TempData["ErrorMessage"] = "You must complete '" + preCourseName + "' before enrolling in this course.";
                    return RedirectToAction("AvailableCourses");
                }
            }

            // D. දත්ත ඇතුළත් කිරීම
            var enrollment = new Enrollment
            {
                CourseId = courseId,
                StudentId = userId,
                EnrollmentDate = DateTime.Now,
                Grade = "Pending"
            };

            db.Enrollments.Add(enrollment);
            db.SaveChanges();

            TempData["SuccessMessage"] = "Successfully enrolled in " + course.CourseName;
            return RedirectToAction("MyCourses");
        }

        [Authorize(Roles = "Student")]
        public ActionResult MyCourses()
        {
            string currentUserId = User.Identity.GetUserId();
            var myEnrollments = db.Enrollments
                .Where(e => e.StudentId == currentUserId)
                .Include(e => e.Course.Assignments.Select(a => a.Submissions)) // Submissions පෙන්වීමට අවශ්‍යයි
                .ToList();

            return View(myEnrollments);
        }

        [Authorize(Roles = "Admin, Lecturer")]
        public ActionResult StudentList(int? courseId) // 👈 මෙතන int? කියා වෙනස් කරන්න
        {
            // කවුරු හරි courseId එකක් නැතුව ආවොත්, crash වෙන්නේ නැතුව Courses පිටුවට හරවා යවයි
            if (courseId == null)
            {
                return RedirectToAction("AvailableCourses");
            }

            ViewBag.CourseId = courseId.Value;

            var studentRoleId = db.Roles.FirstOrDefault(r => r.Name == "Student")?.Id;

            var studentsOnly = db.Users
                .Where(u => u.Roles.Any(r => r.RoleId == studentRoleId))
                .ToList()
                .Select(u => new {
                    Id = u.Id,
                    UserName = u.UserName
                });

            ViewBag.StudentList = new SelectList(studentsOnly, "Id", "UserName");

            // e.CourseId == courseId.Value ලෙස වෙනස් විය යුතුය
            var enrollments = db.Enrollments
                .Where(e => e.CourseId == courseId.Value)
                .Include(e => e.Course)
                .Include(e => e.Student)
                .ToList();

            return View(enrollments);
        }

        // ලකුණු (Grade) ඇතුළත් කිරීමේ Logic එක (POST)
        [HttpPost]
        [Authorize(Roles = "Lecturer")]
        public ActionResult UpdateGrade(int enrollmentId, string grade)
        {
            var enrollment = db.Enrollments.Find(enrollmentId);
            if (enrollment != null)
            {
                enrollment.Grade = grade;
                db.SaveChanges();
            }
            return RedirectToAction("StudentList", new { courseId = enrollment.CourseId });
        }
        // POST: Enrollment/ManualEnroll
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult ManualEnroll(int courseId, string studentId)
        {
            // 1. දැනටමත් Enroll වෙලාදැයි බැලීම පමණක් සිදු කරයි
            var alreadyEnrolled = db.Enrollments.Any(e => e.CourseId == courseId && e.StudentId == studentId);

            if (alreadyEnrolled)
            {
                TempData["ErrorMessage"] = "Student is already enrolled in this course!";
                return RedirectToAction("StudentList", "Enrollment", new { courseId = courseId });
            }

            // 2. කිසිදු Prerequisite පරීක්ෂාවකින් තොරව කෙලින්ම Enroll කිරීම
            var enrollment = new Enrollment
            {
                CourseId = courseId,
                StudentId = studentId,
                EnrollmentDate = DateTime.Now,
                Grade = "Pending"
            };

            db.Enrollments.Add(enrollment);
            db.SaveChanges();

            TempData["SuccessMessage"] = "Student manually enrolled successfully (Override applied).";

            // නැවතත් එම Course එකේ ශිෂ්‍ය ලැයිස්තුවට යොමු කිරීම
            return RedirectToAction("StudentList", "Enrollment", new { courseId = courseId });
        }
    }
}