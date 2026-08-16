using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniManage.Models
{
    public class Submission
    {
        [Key]
        public int SubmissionId { get; set; }

        public int AssignmentId { get; set; }
        [ForeignKey("AssignmentId")]
        public virtual Assignment Assignment { get; set; }

        public string StudentId { get; set; }
        [ForeignKey("StudentId")]
        public virtual ApplicationUser Student { get; set; }

        public string FileName { get; set; }

        [Column(TypeName = "datetime2")] 
        public DateTime SubmittedDate { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime SubmissionDate { get; set; }
        public bool IsLate { get; set; }

        public string Grade { get; set; } // ලකුණු ලබා දීමට
        public string Feedback { get; set; } // ලෙක්චර්ගේ අදහස් සඳහා
    }
}
