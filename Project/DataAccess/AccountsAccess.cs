using Microsoft.Data.Sqlite;
using Dapper;

public class AccountsAccess
{
    private readonly string _connectionString = "Data Source=DataSources/project.db";
    private readonly string Table = "Accounts";

    public void Write(AccountModel account)
    {
        string sql = $"INSERT INTO {Table} (email, password, fullname) VALUES (@EmailAddress, @Password, @FullName)";
        using var connection = new SqliteConnection(_connectionString);
        connection.Execute(sql, account);
    }

    public AccountModel? GetByEmail(string email)
    {
        string sql = $"SELECT * FROM {Table} WHERE email = @Email";
        using var connection = new SqliteConnection(_connectionString);
        return connection.QueryFirstOrDefault<AccountModel>(sql, new { Email = email });
    }

    public void Update(AccountModel account)
    {
        string sql = $"UPDATE {Table} SET email = @EmailAddress, password = @Password, fullname = @FullName WHERE id = @Id";
        using var connection = new SqliteConnection(_connectionString);
        connection.Execute(sql, account);
    }

    public void Delete(AccountModel account)
    {
        string sql = $"DELETE FROM {Table} WHERE id = @Id";
        using var connection = new SqliteConnection(_connectionString);
        connection.Execute(sql, new { Id = account.Id });
    }
}