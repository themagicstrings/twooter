namespace Shared
{
    public class UserCreateDTO
    {
        public string username { get;set; }
        public string email { get;set; }
        public string pw_hash { get;set; }
    }
}