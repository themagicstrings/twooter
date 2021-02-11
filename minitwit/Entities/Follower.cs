using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Follower
    {
        public int who_id { get; set; }
        public User who { get; set; }
        public int whom_id { get; set; }
        public User whom { get; set; }
    }
}
