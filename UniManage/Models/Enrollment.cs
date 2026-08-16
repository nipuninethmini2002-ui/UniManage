using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace UniManage.Models
{
    [Authorize(Roles = "Student,Lecturer,Admin")]
    public class Enrollment
    {
        [Key]
        public int EnrollmentId { get; set; }
        public int CourseId { get; set; }
        public string StudentId { get; set; } // User ගේ ID එක
        public string Grade { get; set; }

        public virtual Course Course { get; set; }
        public DateTime EnrollmentDate { get; set; }
        [ForeignKey("StudentId")]
        public virtual ApplicationUser Student { get; set; } // <--- මේ නම 'Student' විය යුතුයි
        // --
    }
}
