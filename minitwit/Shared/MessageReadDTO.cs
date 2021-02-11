
namespace Shared
{
    public class MessageReadDTO
    {
        public int id { get; set; }
        public UserReadDTO author { get; set; }
        public string text { get; set; }
        public int? pub_date { get; set; }
        public int? flagged { get; set; }
    }
}