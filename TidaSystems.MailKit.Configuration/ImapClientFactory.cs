using MailKit;
using MailKit.Net.Imap;
using System.Security.Cryptography.X509Certificates;
using TidaSystems.MailKit.Configuration.Helpers;
using TidaSystems.MailKit.Configuration.Settings;

namespace TidaSystems.MailKit.Configuration;

/// <summary>
/// Creates configured, connected, and optionally authenticated MailKit IMAP clients.
/// </summary>
public sealed class ImapClientFactory
{
    private readonly MailKitConfiguration configuration;

    /// <summary>
    /// Gets whether the IMAP configuration section is available.
    /// </summary>
    public bool IsConfigured => configuration.Current?.Imap is not null;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImapClientFactory"/> class.
    /// </summary>
    /// <param name="configuration">The MailKit configuration provider.</param>
    public ImapClientFactory(MailKitConfiguration configuration)
    {
        this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Creates, connects, and optionally authenticates an IMAP client using the current configuration.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A connected IMAP client owned by the caller.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the IMAP configuration section is missing.</exception>
    public async Task<ImapClient> CreateImapClientAsync(CancellationToken ct = default)
    {
        MailServiceSettings? settings = configuration.Current;
        ImapSettings imap = settings?.Imap ?? throw new InvalidOperationException("MailKit IMAP is not configured. Add the 'MailKit:Imap' section to appsettings.json.");
        IProtocolLogger protocolLogger = string.IsNullOrWhiteSpace(settings.ProtocolLogPath) ? new NullProtocolLogger() : new ProtocolLogger(settings.ProtocolLogPath);
        ConfiguredImapClient client;
        try
        {
            client = new ConfiguredImapClient(protocolLogger);
        }
        catch
        {
            protocolLogger.Dispose();
            throw;
        }

        try
        {
            MailClientSetup.ApplySettings(client, settings, client.AddCertificate);
            await client.ConnectAsync(imap.Host!, imap.Port, imap.SecureSocketOptions, ct);
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
/// Releases certificate resources owned by a configured IMAP client.
/// </summary>
internal sealed class ConfiguredImapClient : ImapClient
{
    private readonly List<X509Certificate2> certificates = [];
    private int certificatesDisposed;

    internal ConfiguredImapClient(IProtocolLogger logger) : base(logger)
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