using System;
using System.ComponentModel.DataAnnotations;
using UniManage.Models;

namespace UniManage.Models
{
    public class UserMessage
    {
        [Key]
        public int MessageId { get; set; }

        [Required]
        public string SenderId { get; set; } // පණිවිඩය යවන කෙනා (User ID)

        [Required]
        public string ReceiverId { get; set; } // පණිවිඩය ලබන කෙනා (User ID)

        [Required]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        public DateTime SentDate { get; set; }
        public virtual ApplicationUser Sender { get; set; }
        public virtual ApplicationUser Receiver { get; set; }
        public bool IsRead { get; set; } = false;

        public UserMessage()
        {
            SentDate = DateTime.Now;
        }
    }
}
