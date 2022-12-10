using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Polly;
using Polly.Contrib.WaitAndRetry;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using UOrders.Api.Extensions;
using UOrders.Api.Options;
using UOrders.Api.Services;
using UOrders.EFModel;
using UOrders.EFModel.Extensions;
using UOrders.Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiVersioning(setup =>
{
    setup.DefaultApiVersion = new ApiVersion(1, 0);
    setup.AssumeDefaultVersionWhenUnspecified = true;
    setup.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();

builder.Services.AddCors(options => options
    .AddPolicy("allow all", builder => builder
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithExposedHeaders(
            "X-Pagination-PageIdx",
            "X-Pagination-PageSize",
            "X-Pagination-TotalRecords",
            "X-Pagination-TotalPages",
            "X-Pagination-HasNext",
            "X-Pagination-HasPrev"
        )
    )
);

builder.Services.AddUOrdersDbContext();
builder.Services.AddSharedServices();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SECTION_NAME));
builder.Services.Configure<PrinterOptions>(builder.Configuration.GetSection(PrinterOptions.SECTION_NAME));

builder.Services.AddIdentity<UOrdersUser, IdentityRole>()
    .AddEntityFrameworkStores<UOrdersDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})

// Adding Jwt Bearer
.AddJwtBearer((options) =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidAudience = builder.Configuration[$"{JwtOptions.SECTION_NAME}:{nameof(JwtOptions.ValidAudience)}"],
        ValidIssuer = builder.Configuration[$"{JwtOptions.SECTION_NAME}:{nameof(JwtOptions.ValidIssuer)}"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration[$"{JwtOptions.SECTION_NAME}:{nameof(JwtOptions.Secret)}"]))
    };
});

builder.Services.AddTransient<IOrdersRepository, OrdersRepository>();

builder.Services.AddSingleton<IPrintQueue, PrintQueue>();
builder.Services.AddTransient<IPrinter, Printer>();
builder.Services.AddHttpClient<IPrinter, Printer>((services, client) =>
{
    var printerOptions = services.GetRequiredService<IOptions<PrinterOptions>>().Value;

    var token = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
        issuer: "u-orders API",
        audience: "u-orders Printer",
        notBefore: DateTime.Now,
        expires: DateTime.Now.AddMinutes(6),
        claims: new List<Claim>
        {
            new Claim(ClaimTypes.Name, Environment.MachineName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString()),
            new Claim(ClaimTypes.Role, "printer")
        },
        signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(printerOptions.Secret)), SecurityAlgorithms.HmacSha256)
        ));

    client.DefaultRequestHeaders.Authorization = new("bearer", token);
    client.BaseAddress = new Uri($"http://{printerOptions.Host}:{printerOptions.Port:0}/api/v1/");
}).AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(5), 6)));
builder.Services.AddHostedService<PrintWorker>();

builder.Services.AddSingleton<INotifyQueue, NotifyQueue>();
builder.Services.AddSingleton<INotifyQueueReader>(services => services.GetRequiredService<INotifyQueue>());
builder.Services.AddSingleton<INotifyQueueWriter>(services => services.GetRequiredService<INotifyQueue>());
builder.Services.AddTransient<INotifier, EmailNotifier>();
builder.Services.AddTransient<IEmailBuilder, EmailBuilder>();
builder.Services.AddHostedService<NotificationWorker>();


builder.Services.AddAutoMapper(cfg =>
{
    cfg.ConfigureApiV1();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());
        }
    });
}

//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseCors("allow all");
app.MapControllers();

if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER")?.ToLowerInvariant() == "true")
    app.WaitForScheduler(new(0, 0, 15), 20, () => Environment.Exit(-1));

app.Run();