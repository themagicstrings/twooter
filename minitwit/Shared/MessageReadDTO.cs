
namespace Shared
{
    public class MessageReadDTO
    {
        UserReadDTO author { get; set; }
        string text { get; set; }
        int pub_date { get; set; }
        int flagged { get; set; }
    }
}