using System.Collections.Generic;

namespace Shared
{
    public class UserReadDTO
    {
        public int user_id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public List<string> following { get; set; }
        public List<string> followers { get; set; }
    }
}