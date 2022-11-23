using NetConsoleApp.Models;

namespace NetConsoleApp.ORM;

public interface IOrmAccountDao
{
    public IEnumerable<Account> GetAll();
    public Account? GetById(int id);
    public void Insert(string login, string password);
    public void Remove(int? id);
    public void Update(string field, string value, int? id);
}