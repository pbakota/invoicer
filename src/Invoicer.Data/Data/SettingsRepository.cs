using System.Text.RegularExpressions;

using Invoicer.Models;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Logging;

namespace Invoicer.Data.Dao;

public interface ISettingsRepository : ICrudRepository<Settings>
{
    Task<Settings> FindFirst();
    string GetCurrentDatabasePath();
    Task Backup(string dest);
    Task Restore(string from);
}

public partial class SettingsRepository(IDbContextFactory<InvoicerContext> contextFactory, ILogger<Settings> logger)
    : CrudRepository<Settings>(contextFactory, logger), ISettingsRepository
{
    public async Task<Settings> FindFirst()
    {
        using var db = _contextFactory.CreateDbContext();
        return await db.Settings.FirstAsync();
    }

    private static readonly Regex RxDataSourcePath = DataSourcePath();
    public string GetCurrentDatabasePath()
    {
        using var db = _contextFactory.CreateDbContext();
        var connectionString = db.Database.GetDbConnection().ConnectionString;

        var m = RxDataSourcePath.Match(connectionString);
        return !m.Success ? string.Empty : m.Groups["path"].Value;
    }

    public async Task Backup(string dest)
    {
        using var db = _contextFactory.CreateDbContext();
        var backup = new SqliteConnection($"Data Source={dest}");
        await backup.OpenAsync();
        try
        {
            await db.Database.OpenConnectionAsync();
            try
            {
                ((SqliteConnection)db.Database.GetDbConnection()).BackupDatabase(backup);
            }
            finally
            {
                await db.Database.CloseConnectionAsync();
            }
        }
        finally
        {
            await backup.CloseAsync();
        }
    }

    public async Task Restore(string from)
    {
        using var db = _contextFactory.CreateDbContext();
        var backup = new SqliteConnection($"Data Source={from}");
        await backup.OpenAsync();
        try
        {
            await db.Database.OpenConnectionAsync();
            try
            {
                backup.BackupDatabase((SqliteConnection)db.Database.GetDbConnection());
            }
            finally
            {
                await db.Database.CloseConnectionAsync();
            }
        }
        finally
        {
            await backup.CloseAsync();
        }
    }

    [GeneratedRegex("Data Source=(?<path>[^$]+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline, "en-US")]
    private static partial Regex DataSourcePath();
}

