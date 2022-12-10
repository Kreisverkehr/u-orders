using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using UOrders.Shared.Options;
using UOrders.Shared.Services;

namespace UOrders.Shared.Extensions;

public static class ServiceCollectionExtensions
{
    #region Public Methods

    public static IServiceCollection AddMailService(this IServiceCollection services) => services
        .AddOptions<MailOptions>()
            .Configure((Action<MailOptions, IConfiguration>)((options, configuration) => configuration.GetSection(MailOptions.SECTION_NAME).Bind(options))).Services
        .AddSingleton(services =>
        {
            MailOptions mailOptions = services.GetRequiredService<IOptions<MailOptions>>().Value;
            SmtpClient smtpClient = new SmtpClient(mailOptions.SmtpHost, mailOptions.SmtpPort);
            smtpClient.EnableSsl = mailOptions.UseSsl;
            if (!string.IsNullOrWhiteSpace(mailOptions.User))
            {
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(mailOptions.User, mailOptions.Password);
            }
            return smtpClient;
        });

    public static IServiceCollection AddSharedServices(this IServiceCollection services) => services
        .AddMailService()
        .AddWebContext()
        .AddSingleton<ILinkGenerator, LinkGenerator>()
        ;

    public static IServiceCollection AddWebContext(this IServiceCollection services) => services
        .AddOptions<WebContextOptions>()
            .Configure((Action<WebContextOptions, IConfiguration>)((options, configuration) => configuration.GetSection(WebContextOptions.SECTION_NAME).Bind(options))).Services;

    #endregion Public Methods
}