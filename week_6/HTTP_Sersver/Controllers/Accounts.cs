using NetConsoleApp.Attributes;
using NetConsoleApp.Models;
using NetConsoleApp.DataBase;

namespace NetConsoleApp.Controllers;

[HttpController("accounts")]
public class AccountsController
{
    private const string DbName = "SteamDB";
    private const string TableName = "[dbo].[Accounts]";
    
    [HttpGET("/getAccounts")]
    public static List<Account> GetAccounts()
    {
        var db = new DbSteam(DbName);
        var query = $"select * from {TableName}";
        return db.Select<Account>(query).ToList();
    }

    [HttpGET(@"\d")]
    public static Account? GetAccountById(int Id)
    {
        var db = new DbSteam(DbName);
        var query =  $"select * from {TableName} where Id = {Id}";
        return db.Select<Account>(query).FirstOrDefault();
    }

    [HttpPOST]
    public static void SaveAccount(string login, string password)
    {
        var db = new DbSteam(DbName);
        var query = $"insert into {TableName} (login, password) values ('{login}', '{password}')";
        db.ExecuteNonQuery(query);
    }
}