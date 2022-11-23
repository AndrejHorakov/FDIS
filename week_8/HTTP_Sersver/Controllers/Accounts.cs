using NetConsoleApp.Attributes;
using NetConsoleApp.Models;
using NetConsoleApp.ORM;

namespace NetConsoleApp.Controllers;

[HttpController("accounts")]
public class AccountsController
{
    private const string DbName = "SteamDB";
    private const string TableName = "[dbo].[Accounts]";
    
    [HttpGET("/getAccounts")]
    public static List<Account> GetAccounts()
    {
        var db = new OrmAccountDao();
        var query = $"select * from {TableName}";
        return db.GetAll().ToList();
    }

    [HttpGET(@"\d")]
    public static Account? GetAccountById(int id)
    {
        var db = new OrmAccountDao();
        return db.GetById(id);
    }

    [HttpPOST]
    public static void SaveAccount(string login, string password)
    {
        var db = new OrmAccountDao();
        db.Insert(login, password);
    }
}