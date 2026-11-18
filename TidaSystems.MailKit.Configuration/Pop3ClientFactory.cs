using MailKit;
using MailKit.Net.Pop3;
using System.Security.Cryptography.X509Certificates;
using TidaSystems.MailKit.Configuration.Helpers;
using TidaSystems.MailKit.Configuration.Settings;

namespace TidaSystems.MailKit.Configuration;

/// <summary>
/// Creates configured, connected, and optionally authenticated MailKit POP3 clients.
/// </summary>
public sealed class Pop3ClientFactory
{
    private readonly MailKitConfiguration configuration;

    /// <summary>
    /// Gets whether the POP3 configuration section is available.
    /// </summary>
    public bool IsConfigured => configuration.Current?.Pop3 is not null;

    /// <summary>
    /// Initializes a new instance of the <see cref="Pop3ClientFactory"/> class.
    /// </summary>
    /// <param name="configuration">The MailKit configuration provider.</param>
    public Pop3ClientFactory(MailKitConfiguration configuration)
    {
        this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Creates, connects, and optionally authenticates a POP3 client using the current configuration.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A connected POP3 client owned by the caller.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the POP3 configuration section is missing.</exception>
    public async Task<Pop3Client> CreatePop3ClientAsync(CancellationToken ct = default)
    {
        MailServiceSettings? settings = configuration.Current;
        Pop3Settings pop3 = settings?.Pop3 ?? throw new InvalidOperationException("MailKit POP3 is not configured. Add the 'MailKit:Pop3' section to appsettings.json.");
        IProtocolLogger protocolLogger = string.IsNullOrWhiteSpace(settings.ProtocolLogPath) ? new NullProtocolLogger() : new ProtocolLogger(settings.ProtocolLogPath);
        ConfiguredPop3Client client;
        try
        {
            client = new ConfiguredPop3Client(protocolLogger);
        }
        catch
        {
            protocolLogger.Dispose();
            throw;
        }

        try
        {
            MailClientSetup.ApplySettings(client, settings, client.AddCertificate);
            await client.ConnectAsync(pop3.Host!, pop3.Port, pop3.SecureSocketOptions, ct);
            await MailClientSetup.AuthenticateAsync(client, settings, ct);
            return client;
        }
        catch
        {
            try
            {
                await client.DisposeAsync();
            }
            catch
            {
                // Preserve the original exception.
            }

            throw;
        }
    }
}

/// <summary>
/// Releases certificate resources owned by a configured POP3 client.
/// </summary>
internal sealed class ConfiguredPop3Client : Pop3Client
{
    private readonly List<X509Certificate2> certificates = [];
    private int certificatesDisposed;

    internal ConfiguredPop3Client(IProtocolLogger logger) : base(logger)
    {
    }

    internal void AddCertificate(X509Certificate2 certificate)
    {
        certificates.Add(certificate);
    }

    protected override void Dispose(bool disposing)
    {
        try
        {
            base.Dispose(disposing);
        }
        finally
        {
            if (disposing && Interlocked.Exchange(ref certificatesDisposed, 1) == 0)
            {
                foreach (X509Certificate2 certificate in certificates)
                {
                    certificate.Dispose();
                }

                certificates.Clear();
            }
        }
    }
}