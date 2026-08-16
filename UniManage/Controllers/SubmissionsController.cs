using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniManage.Models;
using Microsoft.AspNet.Identity;

namespace UniManage.Controllers
{
    [Authorize]
    public class SubmissionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // 1. ලෙක්චර්ට අදාළ Assignment එකේ සියලුම සිසුන්ගේ පිළිතුරු බැලීමට
        public ActionResult ViewAnswers(int assignmentId)
        {
            var submissions = db.Submissions
                                .Where(s => s.AssignmentId == assignmentId)
                                .Include(s => s.Student)
                                .ToList();

            ViewBag.AssignmentTitle = db.Assignments.Find(assignmentId)?.Title;
            return View(submissions);
        }

        // 2. ලකුණු (Grade) ඇතුළත් කිරීමට
        [HttpPost]
        [ValidateAntiForgeryToken] // ආරක්ෂාව සඳහා මේක දාන්න
        public ActionResult GiveGrade(int submissionId, string grade, string feedback)
        {
            var submission = db.Submissions.Find(submissionId);
            if (submission != null)
            {
                // View එකේ marks වලට 'grade' සහ feedback වලට 'feedback' පාවිච්චි කරනවා
                submission.Grade = grade;
                submission.Feedback = feedback; // මේ පේළිය අනිවාර්යයෙන්ම තිබිය යුතුයි

                db.Entry(submission).State = EntityState.Modified;
                db.SaveChanges();

                // සාර්ථකව Save වුණාම අදාළ Assignment එකේ List එකට Redirect වෙනවා
                return RedirectToAction("ViewAnswers", new { assignmentId = submission.AssignmentId });
            }

            return RedirectToAction("Index", "Home"); // වැරදීමක් වුණොත් Home යන්න
        }

        public ActionResult Create(int assignmentId)
        {
            var assignment = db.Assignments.Find(assignmentId);
            if (assignment == null) return HttpNotFound();

            ViewBag.AssignmentTitle = assignment.Title;
            ViewBag.AssignmentId = assignmentId;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int AssignmentId, HttpPostedFileBase uploadFile)
        {
            // 1. අදාළ ඇසයිමන්ට් එක ලබාගන්න (DueDate එක පරීක්ෂා කිරීමට අවශ්‍යයි)
            var assignment = db.Assignments.Find(AssignmentId);
            if (assignment == null) return HttpNotFound();

            if (uploadFile != null && uploadFile.ContentLength > 0)
            {
                try
                {
                    // 2. ෆයිල් එක සේව් කිරීම
                    string fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(uploadFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Uploads/Submissions/"), fileName);

                    // Uploads ෆෝල්ඩරය නැත්නම් නිර්මාණය කරන්න
                    string directory = Server.MapPath("~/Uploads/Submissions/");
                    if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

                    uploadFile.SaveAs(path);

                    // 3. අලුත් Submission object එක සෑදීම
                    var submission = new Submission();
                    submission.AssignmentId = AssignmentId;
                    submission.StudentId = User.Identity.GetUserId();
                    submission.FileName = fileName;
                    submission.Grade = null;
                    submission.Feedback = null; // Feedback එක මුලින්ම null වේ

                    // 4. වර්තමාන වෙලාව ලබාගැනීම
                    DateTime submissionTime = DateTime.Now;
                    submission.SubmittedDate = submissionTime;

                    // 5. 💡 IsLate logic එක (පද්ධතිය විසින් පරීක්ෂා කිරීම)
                    // ඉදිරිපත් කරන වෙලාව ඇසයිමන්ට් එකේ අවසාන වෙලාවට (DueDate) වඩා වැඩි නම් IsLate = 1 වේ.
                    if (submissionTime > assignment.Deadline)
                    {
                        submission.IsLate = true;
                    }
                    else
                    {
                        submission.IsLate = false;
                    }

                    db.Submissions.Add(submission);
                    db.SaveChanges();

                    return RedirectToAction("MyCourses", "Enrollment");
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "ගොනුව සුරැකීමේදී දෝෂයක් ඇතිවිය: " + ex.Message;
                    return View();
                }
            }

            ViewBag.Error = "කරුණාකර වලංගු ගොනුවක් තෝරන්න.";
            return View();
        }
    }
}