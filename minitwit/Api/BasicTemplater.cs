using System.Collections.Generic;
using Shared;
using System.Text;

namespace Api
{
    public class BasicTemplater
    {
        public static string GenerateTimeline(ICollection<MessageReadDTO> messages)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<html>");
            foreach(MessageReadDTO msg in messages) sb.Append($"<p>{msg.author.username} [{msg.pub_date}]: {msg.text}</p>");
            sb.Append("</html>");
            return sb.ToString();
        }
    }
}