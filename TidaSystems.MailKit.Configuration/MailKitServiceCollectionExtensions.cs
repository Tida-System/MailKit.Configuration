using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TidaSystems.MailKit.Configuration.Helpers;

namespace TidaSystems.MailKit.Configuration;

/// <summary>
/// Provides dependency injection registration for MailKit configuration and client factories.
/// </summary>
public static class MailKitServiceCollectionExtensions
{
    /// <summary>
    /// Registers MailKit configuration and client factories as singleton services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining registrations.</returns>
    public static IServiceCollection AddMailKit(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.TryAddSingleton<MailKitConfiguration>();
        services.TryAddSingleton<SmtpClientFactory>();
        services.TryAddSingleton<Pop3ClientFactory>();
        services.TryAddSingleton<ImapClientFactory>();
        services.TryAddSingleton<MimeMessageFactory>();
        return services;
    }
}