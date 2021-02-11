namespace Shared
{
    public class UserCreateDTO
    {
        string username { get;set; }
        string email { get;set; }
        string pw_hash { get;set; }
    }
}