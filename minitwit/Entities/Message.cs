using System.ComponentModel.DataAnnotations;
using System;

namespace Entities
{
    public class Message
    {
        [Key]
        public int message_id { get; set; }
        //public int UserId { get; set; }
        public User User { get; set; }
        [Required]
        public string text { get; set; }
        public DateTime pub_date { get; set; }
        public int? flagged { get; set; }
    }
}
