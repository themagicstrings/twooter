using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Message
    {
        [Key]
        public int message_id { get;set; }
        public int author_id { get;set; }
        [Required]
        public string text { get;set; }
        public int? pub_date { get;set; }
        public int? flagged { get;set; }
    }
}
