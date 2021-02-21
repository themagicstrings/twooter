using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class User
    {
        [Key]
        public int user_id { get; set; }
        [StringLength(20)]
        [Required]
        public string username { get; set; }
        [Required]
        public string email { get; set; }
        [StringLength(100)]
        [Required]
        public string pw_hash { get; set; }
        public List<Follow> Following { get; set; }
        public List<Follow> FollowedBy { get; set; }
        public List<Message> Messages { get; set; }
    }
}
