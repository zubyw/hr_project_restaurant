using Microsoft.Data.Sqlite;
using Dapper;

public class UsersAccess
{
    private readonly string _connectionString = "Data Source=DataSources/project.db;Foreign Keys=False";
    private readonly string Table = "Users";

    public void Write(UserModel user)
    {
        string sql = $"INSERT INTO {Table} (FirstName, LastName, PhoneNumber, EmailAddress, Password, Roles) VALUES (@FirstName, @LastName, @PhoneNumber, @EmailAddress, @Password, @Roles)";
        using var connection = new SqliteConnection(_connectionString);
        connection.Execute(sql, user);
    }

    public UserModel? GetByEmail(string email)
    {
        string sql = $"SELECT * FROM {Table} WHERE EmailAddress = @Email";
        using var connection = new SqliteConnection(_connectionString);
        return connection.QueryFirstOrDefault<UserModel>(sql, new { Email = email });
    }

    public UserModel? GetById(int id)
    {
        string sql = $"SELECT * FROM {Table} WHERE ID = @Id";
        using var connection = new SqliteConnection(_connectionString);
        return connection.QueryFirstOrDefault<UserModel>(sql, new { Id = id });
    }

    public List<UserModel> GetAll()
    {
        string sql = $"SELECT * FROM {Table}";
        using var connection = new SqliteConnection(_connectionString);
        return connection.Query<UserModel>(sql).ToList();
    }

    public List<UserModel> GetByRole(string role)
    {
        string sql = $"SELECT * FROM {Table} WHERE Roles LIKE @Role";
        using var connection = new SqliteConnection(_connectionString);
        return connection.Query<UserModel>(sql, new { Role = $"%{role}%" }).ToList();
    }

    public void Update(UserModel user)
    {
        string sql = $"UPDATE {Table} SET FirstName = @FirstName, LastName = @LastName, PhoneNumber = @PhoneNumber, EmailAddress = @EmailAddress, Password = @Password, Roles = @Roles WHERE ID = @ID";
        using var connection = new SqliteConnection(_connectionString);
        connection.Execute(sql, user);
    }

    public void Delete(UserModel user)
    {
        string sql = $"DELETE FROM {Table} WHERE ID = @ID";
        using var connection = new SqliteConnection(_connectionString);
        connection.Execute(sql, new { ID = user.ID });
    }

    public void DeleteById(int id)
    {
        string sql = $"DELETE FROM {Table} WHERE ID = @Id";
        using var connection = new SqliteConnection(_connectionString);
        connection.Execute(sql, new { Id = id });
    }
}
