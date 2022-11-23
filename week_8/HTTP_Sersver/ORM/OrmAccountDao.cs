using System.Data.SqlClient;
using NetConsoleApp.Models;

namespace NetConsoleApp.ORM;

public class OrmAccountDao : IOrmAccountDao
{
    private const string ConnectionString =
        @$"Data Source=LAPTOP-FGUJ2MHE;Initial Catalog={DbName};Integrated Security=True";

    private const string TableName = "[dbo].[Accounts]";
    private const string DbName = "SteamDB";
    
    public IEnumerable<Account> GetAll()
    {
        var query = $"select * from {TableName}";
        using var connection = new SqlConnection(ConnectionString);
        connection.Open();

        var cmd = new SqlCommand(query, connection);
        using var reader = cmd.ExecuteReader();
        if (!reader.HasRows) yield break;
        while (reader.Read())
            yield return new Account(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetString(2));
    }

    public Account? GetById(int id)
    {
        var query = $"select * from {TableName} where Id={id}";
        using var connection = new SqlConnection(ConnectionString);
        connection.Open();

        var cmd = new SqlCommand(query, connection);
        using var reader = cmd.ExecuteReader();

        if (!reader.HasRows || !reader.Read()) return null;
        
        return new Account(
            reader.GetInt32(0),
            reader.GetString(1),
            reader.GetString(2));
    }

    public void Insert(string login, string password)
    {
        var query = $"insert into {TableName} values ('{login}', '{password}')";
        using var connection = new SqlConnection(ConnectionString);
        connection.Open();

        var cmd = new SqlCommand(query, connection);
        cmd.ExecuteNonQuery();
    }

    public void Remove(int? id)
    {
        var query = $"delete from {TableName}";
        query += id is not null ? $"where Id={id}" : "";

        using var connection = new SqlConnection(ConnectionString);
        connection.Open();

        var cmd = new SqlCommand(query);
        cmd.ExecuteNonQuery();
    }

    public void Update(string field, string value, int? id)
    {
        var query = $"update {TableName} set {field}={value}";
        query += id is not null ? $"where Id={id}" : "";

        using var connection = new SqlConnection(ConnectionString);
        connection.Open();

        var cmd = new SqlCommand(query);
        cmd.ExecuteNonQuery();
    }
}