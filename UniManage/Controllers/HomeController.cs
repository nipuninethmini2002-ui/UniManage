using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniManage.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Net;
using System.Net.Mail;

namespace UniManage.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                // පද්ධතියේ ඉන්න Students සහ Lecturers ලා ප්‍රමාණය ගණනය කිරීම
                // සටහන: මෙහිදී Role Name එක අනුව සෙවීම වඩාත් පහසුයි
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

                if (User.IsInRole("Admin"))
                {
                    ViewBag.CourseCount = db.Courses.Count();

                    // ලියාපදිංචි වී සිටින මුළු පරිශීලකයන් ගණන (නැතිනම් Role අනුව වෙන් කර පෙන්විය හැක)
                    ViewBag.StudentCount = db.Users.ToList().Count(u => userManager.IsInRole(u.Id, "Student"));
                    ViewBag.LecturerCount = db.Users.ToList().Count(u => userManager.IsInRole(u.Id, "Lecturer"));
                }
                else if (User.IsInRole("Lecturer"))
                {
                    // ලෙක්චරර්ට අදාළ දත්ත මෙතනට
                    ViewBag.MyCourseCount = db.Courses.Count(); // උදාහරණයක් ලෙස
                }
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Discover our vision for the future of education.";
            return View();
        }

        // Contact Page එක පෙන්වීමට (GET)
        public ActionResult Contact()
        {
            ViewBag.Message = "We are here to help you.";
            return View();
        }

        // Contact Page එකෙන් පණිවිඩයක් එවූ විට ක්‍රියාත්මක වන කොටස (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(string name, string email, string subject, string message)
        {
            try
            {
                var senderEmail = new MailAddress("isdnsystem@gmail.com", "UniManage Support");
                var receiverEmail = new MailAddress("isdnsystem@gmail.com", "Admin"); // පණිවිඩය ලැබෙන්න ඕන ලිපිනය
                var password = "kzlo brzk lgat ztrn"; // Gmail එකෙන් ගන්නා App Password එක

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(senderEmail.Address, password)
                };

                using (var mess = new MailMessage(senderEmail, receiverEmail)
                {
                    Subject = "UniManage Contact: " + subject,
                    Body = $"Name: {name}\nEmail: {email}\n\nMessage:\n{message}"
                })
                {
                    smtp.Send(mess);
                }

                ViewBag.Status = "Success";
                ViewBag.SuccessMessage = "Thank you! Your email has been sent successfully.";
            }
            catch (Exception)
            {
                ViewBag.Status = "Error";
                ViewBag.ErrorMessage = "Sorry, we couldn't send your message right now.";
            }

            return View();
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