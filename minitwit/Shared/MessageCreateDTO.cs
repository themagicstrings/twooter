namespace Shared
{
    public class MessageCreateDTO
    {
        public UserReadDTO user { get; set; }
        public string text { get; set; }
        public int pub_date { get; set; }
        public int flagged { get; set; }
    }
}