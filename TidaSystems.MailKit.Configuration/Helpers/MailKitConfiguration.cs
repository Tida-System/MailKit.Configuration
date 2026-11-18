using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Primitives;
using System.Diagnostics;
using TidaSystems.MailKit.Configuration.Settings;

namespace TidaSystems.MailKit.Configuration.Helpers;

/// <summary>
/// Manages MailKit configuration loaded from the application's configuration sources.
/// </summary>
/// <remarks>
/// MailKit configuration is optional and does not prevent application startup.
/// Settings are automatically reloaded when the underlying configuration changes.
/// If a reload fails, the last successfully loaded configuration is preserved.
/// Diagnostic messages are sent to ILogger and to the attached debugger.
/// </remarks>
public sealed class MailKitConfiguration : IDisposable
{
    private const string DebugCategory = "TidaSystems.MailKit.Configuration";
    private readonly IConfiguration configuration;
    private readonly ILogger<MailKitConfiguration> logger;
    private readonly IDisposable? reloadRegistration;
    private MailServiceSettings? current;

    /// <summary>
    /// Gets the current MailKit configuration, or null if no configuration is available.
    /// </summary>
    /// <remarks>
    /// The configuration reference is accessed atomically to support concurrent
    /// reads while configuration reloads are occurring.
    /// </remarks>
    public MailServiceSettings? Current => Volatile.Read(ref current);

    /// <summary>
    /// Initializes a new instance of the <see cref="MailKitConfiguration"/> class.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="logger">An optional logger for configuration diagnostics.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="configuration"/> is null.
    /// </exception>
    public MailKitConfiguration(IConfiguration configuration, ILogger<MailKitConfiguration>? logger = null)
    {
        this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        this.logger = logger ?? NullLogger<MailKitConfiguration>.Instance;
        LogDebug("Initializing MailKit configuration.");
        Reload(true);
        try
        {
            reloadRegistration = ChangeToken.OnChange(configuration.GetReloadToken, OnConfigurationChanged);
            LogDebug("MailKit configuration change listener registered successfully.");
        }
        catch (Exception exception)
        {
            LogError(exception, "Unable to register MailKit configuration change notifications. Automatic reload is unavailable.");
        }
    }

    private MailServiceSettings? Load()
    {
        IConfigurationSection section = configuration.GetSection("MailKit");
        if (!section.Exists())
        {
            return null;
        }

        return section.Get<MailServiceSettings>() ?? throw new InvalidOperationException("The 'MailKit' configuration section could not be bound to MailServiceSettings.");
    }

    private void OnConfigurationChanged()
    {
        LogDebug("MailKit configuration change detected. Reloading settings.");
        Reload(false);
    }

    private void Reload(bool initialLoad)
    {
        try
        {
            MailServiceSettings? settings = Load();
            Volatile.Write(ref current, settings);
            if (settings is null)
            {
                LogDebug("MailKit configuration section not found. No mail protocols are configured.");
            }
            else
            {
                LogDebug(initialLoad ? "MailKit configuration initialized successfully." : "MailKit configuration reloaded successfully.");
            }
        }
        catch (Exception exception)
        {
            if (initialLoad)
            {
                LogError(exception, "Invalid initial MailKit configuration. No MailKit settings have been loaded.");
            }
            else
            {
                LogError(exception, "Invalid MailKit configuration. Previous settings have been preserved.");
            }
        }
    }

    private void LogDebug(string message)
    {
        logger.LogDebug("{Message}", message);
        if (Debugger.IsAttached)
        {
            Debugger.Log(0, DebugCategory, $"[{DebugCategory}] {message}{Environment.NewLine}");
        }
    }

    private void LogError(Exception exception, string message)
    {
        logger.LogError(exception, "{Message}", message);
        if (Debugger.IsAttached)
        {
            Debugger.Log(0, DebugCategory, $"[{DebugCategory}] ERROR: {message}{Environment.NewLine}{exception}{Environment.NewLine}");
        }
    }

    /// <summary>
    /// Releases the configuration change subscription.
    /// </summary>
    /// <remarks>
    /// Stops listening for configuration changes without disposing the
    /// application's IConfiguration or ILogger instances.
    /// </remarks>
    public void Dispose()
    {
        reloadRegistration?.Dispose();
    }
}