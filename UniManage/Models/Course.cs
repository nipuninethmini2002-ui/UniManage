using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniManage.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Course Name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Course name must be between 3 and 100 characters")]
        [Display(Name = "Course Title")]
        public string CourseName { get; set; }

        [Required(ErrorMessage = "Course Credits are required")]
        [Range(1, 5, ErrorMessage = "Credits must be between 1 and 5")]
        [Display(Name = "Credit Hours")]
        public int Credits { get; set; }

        [Required(ErrorMessage = "Enrollment limit is required")]
        [Range(5, 100, ErrorMessage = "Enrollment limit must be between 5 and 100")]
        [Display(Name = "Capacity")]
        public int EnrollmentLimit { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Course Description")]
        public string Description { get; set; }

        // Navigation Properties for Relationships
        public virtual ICollection<Enrollment> Enrollments { get; set; }
        public virtual ICollection<Assignment> Assignments { get; set; }

        // Prerequisites relationship (Self-referencing relationship support)
        public virtual ICollection<Prerequisite> Prerequisites { get; set; }

        // Lecturer Identification (Foreign Key to ApplicationUser)
        [Display(Name = "Assigned Lecturer")]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}
