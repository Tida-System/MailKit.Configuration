

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System.Globalization;
using System.Net;
using System.Security.Authentication;

namespace TidaSystems.MailKit.Configuration;

public sealed class MailKitConfiguration : IDisposable
{
    private readonly IConfiguration configuration;
    private readonly ILogger<MailKitConfiguration> logger;
    private readonly IDisposable reloadRegistration;
    private MailKitSettings current;

    public MailKitSettings Current => Volatile.Read(ref current);

    public MailKitConfiguration(IConfiguration configuration, ILogger<MailKitConfiguration> logger)
    {
        this.configuration = configuration;
        this.logger = logger;
        current = Load();
        reloadRegistration = ChangeToken.OnChange(configuration.GetReloadToken, Reload);
    }

    private MailKitSettings Load()
    {
        IConfigurationSection section = configuration.GetSection("MailKit");
        if (!section.Exists())
        {
            throw new InvalidOperationException("MailKit configuration section is missing.");
        }

        return new MailKitSettings
        {
            Timeout = ReadInt(section, "Timeout"),
            CheckCertificateRevocation = ReadBool(section, "CheckCertificateRevocation"),
            SslProtocols = ReadSslProtocols(section),
            LocalEndPoint = ReadLocalEndPoint(section.GetSection("LocalEndPoint"))
        };
    }

    private static int? ReadInt(IConfigurationSection section, string name)
    {
        string? value = section[name];
        if (value is null)
        {
            return null;
        }

        if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result))
        {
            throw new InvalidOperationException($"{section.Path}:{name} is not a valid integer.");
        }

        return result;
    }

    private static bool? ReadBool(IConfigurationSection section, string name)
    {
        string? value = section[name];
        if (value is null)
        {
            return null;
        }

        if (!bool.TryParse(value, out bool result))
        {
            throw new InvalidOperationException($"{section.Path}:{name} must be true or false.");
        }

        return result;
    }

    private static SslProtocols? ReadSslProtocols(IConfigurationSection section)
    {
        string? value = section["SslProtocols"];
        if (value is null)
        {
            return null;
        }

        SslProtocols result = SslProtocols.None;
        foreach (string item in value.Split(',', StringSplitOptions.TrimEntries))
        {
            if (!Enum.TryParse(item, true, out SslProtocols protocol) || !Enum.IsDefined(protocol))
            {
                throw new InvalidOperationException($"MailKit:SslProtocols contains an invalid value: {item}.");
            }

            result |= protocol;
        }

        return result;
    }

    private static IPEndPoint? ReadLocalEndPoint(IConfigurationSection section)
    {
        string? address = section["Address"]?.Trim();
        int port = ReadInt(section, "Port") ?? 0;
        if (port < 0 || port > 65535)
        {
            throw new InvalidOperationException($"{section.Path}:Port must be between 0 and 65535.");
        }

        if (string.IsNullOrWhiteSpace(address))
        {
            if (port != 0)
            {
                throw new InvalidOperationException($"{section.Path}:Address is required when Port is configured.");
            }

            return null;
        }

        if (!IPAddress.TryParse(address, out IPAddress? ip))
        {
            throw new InvalidOperationException($"{section.Path}:Address must be a valid IP address.");
        }

        return new IPEndPoint(ip, port);
    }

    private void Reload()
    {
        try
        {
            MailKitSettings settings = Load();
            Volatile.Write(ref current, settings);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Invalid MailKit configuration. Previous settings have been preserved.");
        }
    }

    public void Dispose()
    {
        reloadRegistration.Dispose();
    }
}
