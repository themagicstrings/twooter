using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Message
    {
        [Key]
        int message_id { get;set; }
        int author_id { get;set; }
        [Required]
        string text { get;set; }
        int? pub_date { get;set; }
        int? flagged { get;set; }
    }
}
