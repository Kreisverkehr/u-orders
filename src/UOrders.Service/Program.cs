using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using UOrders.Api.Extensions;
using UOrders.EFModel;
using UOrders.EFModel.Extensions;
using UOrders.Service.Extensions;
using UOrders.Service.Jobs;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) => services
        .AddLogging()
        .Configure<QuartzOptions>(hostContext.Configuration.GetSection("Quartz"))
        .AddQuartz(q =>
        {
            q.SchedulerId = Environment.MachineName;
            q.UseMicrosoftDependencyInjectionJobFactory();
            q.UseSimpleTypeLoader();
            q.UseInMemoryStore();
            q.UseDefaultThreadPool(tp =>
            {
                tp.MaxConcurrency = 10;
            });

            var readyBroadcastJobKey = new JobKey("ReadyBroadcast");
            q.AddJob<BroadcastDbReadyEventJob>(readyBroadcastJobKey);
            q.AddTrigger(t => t
                .ForJob(readyBroadcastJobKey)
                .StartNow()
                .WithSimpleSchedule(s => s
                    .WithIntervalInSeconds(1)
                    .WithRepeatCount(60)
                )
            );
            q.AddTrigger(t => t
                .ForJob(readyBroadcastJobKey)
                .StartAt(DateTime.Now.AddMinutes(1))
                .WithSimpleSchedule(s => s
                    .WithIntervalInMinutes(1)
                    .RepeatForever()
                )
            );

            var deleteUnusedTextsJobKey = new JobKey("DeleteUnusedTexts");
            q.AddJob<DeleteUnusedTextsJob>(deleteUnusedTextsJobKey);
            q.AddTrigger(t => t
                .ForJob(deleteUnusedTextsJobKey)
                .StartAt(DateTime.Now.NextFullHour())
                .WithSimpleSchedule(s => s
                    .WithIntervalInHours(1)
                    .RepeatForever()
                )
            );

            var cleanupOrdersJobKey = new JobKey("CleanupOrders");
            q.AddJob<CleanupOrdersJob>(cleanupOrdersJobKey);
            q.AddTrigger(t => t
                .ForJob(cleanupOrdersJobKey)
                .StartAt(DateTime.Now.NextDay())
                .WithSimpleSchedule(s => s
                    .WithInterval(new(days: 1, hours: 0, minutes: 0, seconds: 0))
                    .RepeatForever()
                )
            );

            var cleanupMenuObjects = new JobKey("CleanupMenuObjects");
            q.AddJob<CleanupOldMenuObjects>(cleanupMenuObjects);
            q.AddTrigger(t => t
                .ForJob(cleanupMenuObjects)
                .StartAt(DateTime.Now.NextQuarterPastHour())
                .WithSimpleSchedule(s => s
                    .WithIntervalInHours(1)
                    .RepeatForever()
                )
            );

            var cleanupOldPricesJobKey = new JobKey("CleanupOldPrices");
            q.AddJob<CleanupOldPricesJob>(cleanupOldPricesJobKey);
            q.AddTrigger(t => t
                .ForJob(cleanupOldPricesJobKey)
                .StartAt(DateTime.Now.NextHalfHour())
                .WithSimpleSchedule(s => s
                    .WithIntervalInHours(1)
                    .RepeatForever()
                )
            );
        })
        .AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        })

        .AddUOrdersDbContext()
        .AddIdentityCore<UOrdersUser>()
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<UOrdersDbContext>()
    );

var host = builder.Build();
host.WaitForDb(new(0, 0, 15), 20, () => Environment.Exit(-1));
host.MigrateDb<UOrdersDbContext>();
await host.CreateAdminIfNeeded();
host.SeedData();
host.Run();