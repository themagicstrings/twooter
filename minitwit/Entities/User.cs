using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class User
    {
        [Key]
        int user_id { get;set; }
        [Required]
        string username { get;set; }
        [Required]
        string email { get;set; }
        [Required]
        string pw_hash { get;set; }
        ICollection<User> followers { get;set; }
    }
}
