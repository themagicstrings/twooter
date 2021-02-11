namespace Shared
{
    public class MessageCreateDTO
    {
        UserReadDTO user { get; set; }
        string text { get; set; }
        int pub_date { get; set; }
        int flagged { get; set; }
    }
}