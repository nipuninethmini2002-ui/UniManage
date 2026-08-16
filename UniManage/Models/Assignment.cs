using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Collections.Generic;

namespace UniManage.Models
{
    public class Assignment
    {
        [Key]
        public int AssignmentId { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Deadline { get; set; }

        public int CourseId { get; set; }
        public virtual Course Course { get; set; }
        // --- Student ගේ වැඩ වලට අදාළ කෑලි ---
        public string StudentId { get; set; } // කවුද Submit කරේ කියලා දැනගන්න
        public string FilePath { get; set; }  // File එක Save වෙන Path එක
        public DateTime? SubmittedDate { get; set; } // භාර දුන් වෙලාව
                                                     // Assignment එකකට Submissions ගොඩක් තිබිය හැක
        public virtual ICollection<Submission> Submissions { get; set; }
    }
}
