using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using UOrders.EFModel.Options;

namespace UOrders.EFModel.Extensions;

public static class IServiceCollectionExtensions
{
    #region Public Methods

    public static IServiceCollection AddUOrdersDbContext(this IServiceCollection services)
    {
        void DbConfig(IServiceProvider serviceProvider, DbContextOptionsBuilder dbBuilder)
        {
            var dbConfig = serviceProvider.GetRequiredService<IOptions<Db>>().Value;

            var connStr = dbConfig.Provider.ToLowerInvariant() switch
            {
                "mysql" => $"Server={dbConfig.Host};Port={dbConfig.Port:0};Database={dbConfig.DbName};Uid={dbConfig.User};Pwd={dbConfig.Password};",
                "mssql" => $"Server={dbConfig.Host},{dbConfig.Port:0};User Id={dbConfig.User};Password={dbConfig.Password};",
                "postgres" => $"Server={dbConfig.Host};Port={dbConfig.Port:0};Database={dbConfig.DbName};User Id={dbConfig.User};Password={dbConfig.Password};",
                _ => string.Empty
            };

            switch (dbConfig.Provider.ToLowerInvariant())
            {
                case "mysql":
                    dbBuilder.UseMySql(connStr, ServerVersion.AutoDetect(connStr), x => x
                        .MigrationsAssembly("UOrders.EFModel.Mysql")
                        .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
                    );
                    break;

                case "mssql":
                    dbBuilder.UseSqlServer(connStr, x => x
                        .MigrationsAssembly("UOrders.EFModel.SqlServer")
                        .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
                    );
                    break;

                case "postgres":
                    dbBuilder.UseNpgsql(connStr, x => x
                        .MigrationsAssembly("UOrders.EFModel.Postgres")
                        .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
                        .UseNodaTime()
                    );
                    break;

            }
            dbBuilder.UseLazyLoadingProxies();
        }

        services.AddOptions<Db>().Configure<IConfiguration>((options, configuration) =>
            {
                configuration.GetSection(Db.SECTION_NAME).Bind(options);
            });
        services.AddDbContext<UOrdersDbContext>(DbConfig);

        return services;
    }

    #endregion Public Methods
}