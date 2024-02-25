using Invoicer.Data;
using Microsoft.EntityFrameworkCore;

namespace Invoicer.App.Configuration;

public static class DbContextConfiguration
{
    public static WebApplicationBuilder AddInvoicerContext(this WebApplicationBuilder builder)
    {
        builder.Services.AddPooledDbContextFactory<InvoicerContext>(options =>
        {
            var logger = LoggerFactory.Create(configure =>
            {
                configure.AddConsole();
                configure.SetMinimumLevel(LogLevel.Trace);
            }).CreateLogger<WebApplicationBuilder>();

            logger.LogInformation("Configuring db context");
            var provider = builder.Configuration.GetValue("provider", Provider.Sqlite.Name);
            if (provider == Provider.Sqlite.Name)
            {
                #if ELECTRON_APP
                if(Environment.GetEnvironmentVariable("_ENVIRONMENT") != "Development")
                {
                    var dataPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/Invoicer";
                    if(!Directory.Exists(dataPath))
                    {
                        Directory.CreateDirectory(dataPath);
                    }
                    var connectionString = $"Data Source={dataPath}/invoicer.db";
                    logger.LogInformation("Connection string = {}", connectionString);
                    options.UseSqlite(
                        connectionString,
                        x => x.MigrationsAssembly(Provider.Sqlite.Assembly)
                    );
                }
                else
                {
                    options.UseSqlite(
                        builder.Configuration.GetConnectionString(Provider.Sqlite.Name)!,
                        x => x.MigrationsAssembly(Provider.Sqlite.Assembly)
                    );
                }
                #else
                options.UseSqlite(
                    builder.Configuration.GetConnectionString(Provider.Sqlite.Name)!,
                    x => x.MigrationsAssembly(Provider.Sqlite.Assembly)
                );
                #endif
            }

            if (provider == Provider.Postgres.Name)
            {
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString(Provider.Postgres.Name)!,
                    x => x.MigrationsAssembly(Provider.Postgres.Assembly)
                );
            }

            if (builder.Environment.IsDevelopment())
            {
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
            }
        }).AddScoped(p => p.GetRequiredService<IDbContextFactory<InvoicerContext>>().CreateDbContext());

        return builder;
    }
}