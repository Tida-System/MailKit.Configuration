using MailKit;
using MailKit.Net.Smtp;
using System.Security.Cryptography.X509Certificates;
using TidaSystems.MailKit.Configuration.Helpers;
using TidaSystems.MailKit.Configuration.Settings;

namespace TidaSystems.MailKit.Configuration;

/// <summary>
/// Creates configured, connected, and optionally authenticated MailKit SMTP clients.
/// </summary>
/// <remarks>
/// Each call creates an independent SMTP client using the current MailKit configuration.
/// The factory does not cache connections or retain references to created clients.
/// </remarks>
public sealed class SmtpClientFactory
{
    private readonly MailKitConfiguration configuration;

    /// <summary>
    /// Gets whether the SMTP configuration section is available.
    /// </summary>
    /// <remarks>
    /// This property does not validate credentials or test connectivity.
    /// </remarks>
    public bool IsConfigured => configuration.Current?.Smtp is not null;

    /// <summary>
    /// Initializes a new instance of the <see cref="SmtpClientFactory"/> class.
    /// </summary>
    /// <param name="configuration">The MailKit configuration provider.</param>
    public SmtpClientFactory(MailKitConfiguration configuration)
    {
        this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Creates, configures, connects, and optionally authenticates a MailKit SMTP client.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A connected SMTP client owned by the caller.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the MailKit:Smtp configuration section is missing.
    /// </exception>
    /// <remarks>
    /// The caller is responsible for disposing the returned client.
    /// Connection and authentication exceptions are propagated without translation.
    /// </remarks>
    public async Task<SmtpClient> CreateSmtpClientAsync(CancellationToken ct = default)
    {
        MailServiceSettings? settings = configuration.Current;
        SmtpSettings smtp = settings?.Smtp ?? throw new InvalidOperationException("MailKit SMTP is not configured. Add the 'MailKit:Smtp' section to appsettings.json.");
        IProtocolLogger protocolLogger = string.IsNullOrWhiteSpace(settings.ProtocolLogPath) ? new NullProtocolLogger() : new ProtocolLogger(settings.ProtocolLogPath);
        ConfiguredSmtpClient client;
        try
        {
            client = new ConfiguredSmtpClient(protocolLogger);
        }
        catch
        {
            protocolLogger.Dispose();
            throw;
        }

        try
        {
            MailClientSetup.ApplySettings(client, settings, client.AddCertificate);
            if (smtp.LocalDomain is not null)
            {
                client.LocalDomain = smtp.LocalDomain;
            }

            if (smtp.RequireTLS.HasValue)
            {
                client.RequireTLS = smtp.RequireTLS.Value;
            }

            await client.ConnectAsync(smtp.Host!, smtp.Port, smtp.SecureSocketOptions, ct);
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

    private sealed class ConfiguredSmtpClient : SmtpClient
    {
        private readonly List<X509Certificate2> certificates = [];
        private int certificatesDisposed;

        internal ConfiguredSmtpClient(IProtocolLogger logger) : base(logger)
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
}