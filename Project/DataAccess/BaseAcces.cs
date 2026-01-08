using Microsoft.Data.Sqlite;
using Dapper;
using Project.DataModels;

public abstract class BaseAccess<T> where T : IIdentifier
{
    protected virtual string Table { get; } = "";
    protected virtual string _connectionString { get; } = "Data Source=DataSources/project.db";
    public abstract void Write(T item);

    public abstract void Update(T item);

    public virtual void Delete(T item)
    {
        string sql = $"DELETE FROM {Table} WHERE ID = @Id";
        using var connection = new SqliteConnection(_connectionString);
        connection.Execute(sql, new { Id = item.ID });
    }
}

// this abstract class forces our access classes to have these 3 methods