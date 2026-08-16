using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UniManage.Models
{
    public class CourseMaterial
    {
        [Key]
        public int MaterialId { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        // File එක Save වන ස්ථානය (Path)
        public string FilePath { get; set; }

        public DateTime UploadDate { get; set; }

        // මෙම Material එක අයිති Course එක
        public int CourseId { get; set; }
        public virtual Course Course { get; set; }
        public string UploadedBy { get; set; }
    }
}
