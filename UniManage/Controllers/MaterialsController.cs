using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniManage.Models;

namespace UniManage.Controllers
{
    // Class එකේ ඉහළින් තිබූ Authorize ඉවත් කළා, එවිට ඕනෑම Login වූ අයෙක්ට ඇතුළු විය හැක
    [Authorize]
    public class MaterialsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // 1. MATERIAL LIST (Students, Lecturers, Admins හැමෝටම පුළුවන්)
        public ActionResult Index()
        {
            var materials = db.CourseMaterials.OrderByDescending(m => m.UploadDate).ToList();
            return View(materials);
        }

        // 2. DOWNLOAD FILE (Students, Lecturers, Admins හැමෝටම පුළුවන්)
        public ActionResult DownloadFile(int id)
        {
            var material = db.CourseMaterials.Find(id);

            if (material == null || string.IsNullOrEmpty(material.FilePath))
            {
                return HttpNotFound("File record not found in database.");
            }

            string physicalPath = Server.MapPath(material.FilePath);

            if (System.IO.File.Exists(physicalPath))
            {
                string fileName = Path.GetFileName(physicalPath);
                return File(physicalPath, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            else
            {
                return Content("Physical file not found on the server.");
            }
        }

        // --- මෙතැන් සිට පහළට ඇති දේවල් වලට Student ට අවසර නැත ---

        // GET: Materials/Create
        [Authorize(Roles = "Lecturer,Admin")]
        public ActionResult Create(int? courseId)
        {
            if (courseId.HasValue)
            {
                ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", courseId.Value);
            }
            else
            {
                ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName");
            }
            return View();
        }

        // POST: Materials/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Lecturer,Admin")]
        public ActionResult Create(CourseMaterial material, HttpPostedFileBase uploadFile)
        {
            if (ModelState.IsValid)
            {
                if (uploadFile != null && uploadFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(uploadFile.FileName);
                    string folderPath = Server.MapPath("~/App_Data/Uploads");

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string path = Path.Combine(folderPath, fileName);
                    uploadFile.SaveAs(path);

                    material.FilePath = "/App_Data/Uploads/" + fileName;
                    material.UploadDate = DateTime.Now;

                    db.CourseMaterials.Add(material);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Please select a file to upload.");
                }
            }
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", material.CourseId);
            return View(material);
        }

        // GET: Materials/Edit/5
        [Authorize(Roles = "Lecturer,Admin")]
        public ActionResult Edit(int id)
        {
            var material = db.CourseMaterials.Find(id);
            if (material == null) return HttpNotFound();

            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", material.CourseId);
            return View(material);
        }

        // POST: Materials/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Lecturer,Admin")]
        public ActionResult Edit(CourseMaterial material, HttpPostedFileBase uploadFile)
        {
            if (ModelState.IsValid)
            {
                var existingMaterial = db.CourseMaterials.Find(material.MaterialId);
                if (existingMaterial == null) return HttpNotFound();

                existingMaterial.Title = material.Title;
                existingMaterial.Description = material.Description;
                existingMaterial.CourseId = material.CourseId;

                if (uploadFile != null && uploadFile.ContentLength > 0)
                {
                    string oldPath = Server.MapPath(existingMaterial.FilePath);
                    if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);

                    string fileName = Path.GetFileName(uploadFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/App_Data/Uploads"), fileName);
                    uploadFile.SaveAs(path);
                    existingMaterial.FilePath = "/App_Data/Uploads/" + fileName;
                }

                db.Entry(existingMaterial).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", material.CourseId);
            return View(material);
        }

        // POST: Materials/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Lecturer,Admin")]
        public ActionResult Delete(int id)
        {
            var material = db.CourseMaterials.Find(id);
            if (material != null)
            {
                string fullPath = Server.MapPath(material.FilePath);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

                db.CourseMaterials.Remove(material);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // GET: Materials/Details/5 (ප්‍රශ්නයක තිබූ නිසා ඇතුළත් කළා)
        [Authorize]
        public ActionResult Details(int id)
        {
            var material = db.CourseMaterials.Find(id);
            if (material == null) return HttpNotFound();
            return View(material);
        }
    }
}