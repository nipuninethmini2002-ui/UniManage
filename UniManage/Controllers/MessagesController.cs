using Microsoft.AspNet.Identity;
using UniManage.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;

namespace UniManage.Controllers
{
    [Authorize] // ලොග් වූ අයට පමණයි
    public class MessagesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // 1. තමන්ට ආපු පණිවිඩ බැලීම (Inbox)
        public ActionResult Inbox()
        {
            string currentUserId = User.Identity.GetUserId();
            var messages = db.UserMessages
                             .Where(m => m.ReceiverId == currentUserId)
                             .OrderByDescending(m => m.SentDate)
                             .ToList();
            return View(messages);
        }

        // GET: Messages/Create
        public ActionResult Create()
        {
            var currentUserId = User.Identity.GetUserId();

            // 1. මුලින්ම අදාළ Roles වල ID ටික ලබාගන්න (පරිගණකය මතදී)
            var studentRoleId = db.Roles.FirstOrDefault(r => r.Name == "Student")?.Id;
            var lecturerRoleId = db.Roles.FirstOrDefault(r => r.Name == "Lecturer")?.Id;
            var adminRoleId = db.Roles.FirstOrDefault(r => r.Name == "Admin")?.Id;

            IQueryable<ApplicationUser> userQuery = db.Users;

            if (User.IsInRole("Student"))
            {
                // ශිෂ්‍යයෙකුට පෙන්වන්නේ Lecturers පමණි
                // u.Roles.Any(ur => ur.RoleId == lecturerRoleId) යන සරල query එක EF මගින් සහය දක්වයි
                userQuery = userQuery.Where(u => u.Roles.Any(ur => ur.RoleId == lecturerRoleId));
            }
            else if (User.IsInRole("Lecturer"))
            {
                // කථිකාචාර්යවරයෙකුට Students සහ Admins පෙන්වයි
                userQuery = userQuery.Where(u => u.Roles.Any(ur => ur.RoleId == studentRoleId || ur.RoleId == adminRoleId));
            }
            else if (User.IsInRole("Admin"))
            {
                // Admin ට Lecturers පෙන්වයි
                userQuery = userQuery.Where(u => u.Roles.Any(ur => ur.RoleId == lecturerRoleId));
            }

            var filteredReceivers = userQuery.ToList();

            ViewBag.Receivers = new SelectList(filteredReceivers, "Id", "UserName");
            return View();
        }

        // POST: Messages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ReceiverId,Subject,Content")] UserMessage message)
        {
            ModelState.Remove("SenderId");
            ModelState.Remove("SentDate");

            if (ModelState.IsValid)
            {
                try
                {
                    message.SenderId = User.Identity.GetUserId();
                    message.SentDate = DateTime.Now;

                    db.UserMessages.Add(message);
                    db.SaveChanges();
                    return RedirectToAction("Inbox");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Database Error: " + ex.Message);
                }
            }

            // --- වැදගත් කොටස මෙතැන් සිට ---
            // මෙතැනදී ModelState.IsValid false වුණොත් (error එකක් ආවොත්), 
            // අපි DropDown එකට අවශ්‍ය දත්ත නැවත ලබාගත යුතුයි. නැතිනම් ඔයාට ලැබුණ Error එක එනවා.

            var studentRoleId = db.Roles.FirstOrDefault(r => r.Name == "Student")?.Id;
            var lecturerRoleId = db.Roles.FirstOrDefault(r => r.Name == "Lecturer")?.Id;
            var adminRoleId = db.Roles.FirstOrDefault(r => r.Name == "Admin")?.Id;

            IQueryable<ApplicationUser> userQuery = db.Users;

            if (User.IsInRole("Student"))
            {
                userQuery = userQuery.Where(u => u.Roles.Any(ur => ur.RoleId == lecturerRoleId || ur.RoleId == adminRoleId));
            }
            else if (User.IsInRole("Lecturer"))
            {
                userQuery = userQuery.Where(u => u.Roles.Any(ur => ur.RoleId == studentRoleId || ur.RoleId == adminRoleId));
            }
            else if (User.IsInRole("Admin"))
            {
                userQuery = userQuery.Where(u => u.Roles.Any(ur => ur.RoleId == lecturerRoleId || ur.RoleId == studentRoleId ));
            }

            // ViewBag.Receivers නැවත පුරවන්න
            ViewBag.Receivers = new SelectList(userQuery.ToList(), "Id", "UserName", message.ReceiverId);

            return View(message);
        }

        // Dropdown එක නැවත පිරවීමට හදාගත් Helper Method එකක්
        private void PrepareReceiversList()
        {
            // මෙතැනදී ඉහත GET Method එකේ ඇති Filtering Logic එකම නැවත ලියන්න හෝ පොදු එකක් ලෙස සකසන්න
            // (කේතය කෙටි කිරීමට මෙහි සවිස්තරව ලියා නැත)
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        // 4. සියලුම පණිවිඩ බැලීම (Admin ට පමණයි)
        [Authorize(Roles = "Admin")]
        public ActionResult AllMessages()
        {
            // Database එකේ ඇති සියලුම පණිවිඩ, යවන්නා සහ ලබන්නාගේ විස්තරත් සමඟ ලබා ගැනීම
            var allMessages = db.UserMessages
                                .OrderByDescending(m => m.SentDate)
                                .ToList();

            return View(allMessages);
        }
        // MessagesController.cs ඇතුළත
        [ChildActionOnly]
        public ActionResult GetUnreadCount()
        {
            if (User.Identity.IsAuthenticated)
            {
                var currentUserId = User.Identity.GetUserId();
                int count = db.UserMessages.Count(m => m.ReceiverId == currentUserId && !m.IsRead);
                return Content(count.ToString());
            }
            return Content("0");
        }
        public ActionResult Details(int id)
        {
            var message = db.UserMessages.Find(id);
            if (message == null) return HttpNotFound();

            // මැසේජ් එක කියවන පුද්ගලයා receiver නම් පමණක් IsRead = true කරන්න
            var currentUserId = User.Identity.GetUserId();
            if (message.ReceiverId == currentUserId && !message.IsRead)
            {
                message.IsRead = true;
                db.Entry(message).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return View(message);
        }
        // POST: Messages/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var message = db.UserMessages.Find(id);
            if (message == null)
            {
                return HttpNotFound();
            }

            var currentUserId = User.Identity.GetUserId();

            // මැසේජ් එක යැවූ කෙනාට හෝ ලැබුණු කෙනාට පමණක් delete කිරීමට ඉඩ දීම
            if (message.SenderId == currentUserId || message.ReceiverId == currentUserId || User.IsInRole("Admin"))
            {
                db.UserMessages.Remove(message);
                db.SaveChanges();
            }

            return RedirectToAction("Inbox");
        }
    }
}