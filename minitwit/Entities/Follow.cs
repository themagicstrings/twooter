using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Follow
    {
        public int FollowerId { get; set; }
        public virtual User Follower { get; set; }
        public int FollowedId { get; set; }
        public virtual User Followed { get; set; }
    }
}
