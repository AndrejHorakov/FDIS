namespace NetConsoleApp.Models;

public class Account
{
    public int Id { get; }
    public string Login { get; }
    public string Password { get; }
    
    public Account(int id, string login, string password)
    {
        Id = id;
        Login = login;
        Password = password;
    }
}