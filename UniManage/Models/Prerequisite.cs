using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniManage.Models
{
    public class Prerequisite
    {
        [Key]
        public int Id { get; set; }

        // ප්‍රධාන පාඨමාලාව (උදා: Advanced Programming)
        [Required]
        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public virtual Course MainCourse { get; set; }

        // පූර්ව අවශ්‍යතාවය වන පාඨමාලාව (උදා: Basic Programming)
        [Required]
        public int PrerequisiteCourseId { get; set; }
        [ForeignKey("PrerequisiteCourseId")]
        public virtual Course PrerequisiteCourse { get; set; }
        public virtual Course Course { get; set; }
    }
}
