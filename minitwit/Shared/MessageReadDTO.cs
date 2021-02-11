namespace Shared
{
    public class MessageReadDTO
    {
        public int message_id { get; set; }
        public int author_id { get; set; }
        public string text { get; set; }
        public int? pub_date { get; set; }
        public int? flagged { get; set; }
    }
}