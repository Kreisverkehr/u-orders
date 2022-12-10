using ESCPOS_NET;
using ESCPOS_NET.Emitters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UOrders.PrintService.Options;
using UOrders.PrintService.Services;
using UOrders.Shared.Extensions;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
        ValidAudience = "u-orders Printer",
        ValidIssuer = "u-orders API",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration[$"AuthSecret"]))
    };
});

builder.Services.AddSharedServices();
builder.Services.Configure<PrinterSettings>(builder.Configuration.GetSection(PrinterSettings.Printer));
builder.Services.AddSingleton(sp =>
{
    var settings = sp.GetRequiredService<IOptions<PrinterSettings>>();
    IPrinter ptr = settings.Value.Type switch
    {
        PrinterType.File => new FilePrinter(settings.Value.FilePath, true),
        PrinterType.Serial => new SerialPrinter(settings.Value.SerialComPort, settings.Value.SerialBaudRate ?? 19200),
        PrinterType.Network => new NetworkPrinter(new() { ConnectionString = $"{settings.Value.NetworkAddress}:{settings.Value.NetworkPort ?? 9000}" }),
        _ => new MemoryPrinter()
    };
    return ptr;
});
builder.Services.AddSingleton<ICommandEmitter, EPSON>();
builder.Services.AddSingleton<IPrintFormatter, PrintFormatter>();
builder.Services.AddSingleton<IPrinterQueue, PrinterQueue>();
builder.Services.AddHostedService<PrinterService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
