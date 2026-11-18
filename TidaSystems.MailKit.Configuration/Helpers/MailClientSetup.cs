using MailKit;
using MailKit.Net.Proxy;
using MailKit.Security;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using TidaSystems.MailKit.Configuration.Settings;

namespace TidaSystems.MailKit.Configuration.Helpers;

internal static class MailClientSetup
{
    internal static void ApplySettings(IMailService client, MailServiceSettings settings, Action<X509Certificate2> ownCertificate)
    {
        if (settings.Timeout.HasValue)
        {
            client.Timeout = settings.Timeout.Value;
        }

        if (settings.CheckCertificateRevocation.HasValue)
        {
            client.CheckCertificateRevocation = settings.CheckCertificateRevocation.Value;
        }

        if (settings.SslProtocols.HasValue)
        {
            client.SslProtocols = settings.SslProtocols.Value;
        }

        if (settings.SslCipherSuitesPolicy is { Length: > 0 } && !OperatingSystem.IsWindows() && !OperatingSystem.IsAndroid())
        {
            client.SslCipherSuitesPolicy = new CipherSuitesPolicy(settings.SslCipherSuitesPolicy);
        }

        IPEndPoint? localEndPoint = CreateEndPoint(settings.LocalEndPoint);
        if (localEndPoint is not null)
        {
            client.LocalEndPoint = localEndPoint;
        }

        if (settings.ClientCertificates is { Length: > 0 })
        {
            client.ClientCertificates = LoadCertificates(settings.ClientCertificates, ownCertificate);
        }

        if (settings.ProxyClient is not null)
        {
            client.ProxyClient = CreateProxy(settings.ProxyClient, ownCertificate);
        }
    }

    internal static async Task AuthenticateAsync(IMailService client, MailServiceSettings settings, CancellationToken ct)
    {
        if (settings.AuthenticationMechanisms is { Length: > 0 })
        {
            client.AuthenticationMechanisms.RemoveWhere(name => !settings.AuthenticationMechanisms.Contains(name, StringComparer.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(settings.OAuthToken))
        {
            if (client.AuthenticationMechanisms.Contains("OAUTHBEARER"))
            {
                await client.AuthenticateAsync(new SaslMechanismOAuthBearer(settings.Username!, settings.OAuthToken), ct);
            }
            else
            {
                await client.AuthenticateAsync(new SaslMechanismOAuth2(settings.Username!, settings.OAuthToken), ct);
            }
        }
        else if (!string.IsNullOrWhiteSpace(settings.Username))
        {
            await client.AuthenticateAsync(settings.Username, settings.Password ?? string.Empty, ct);
        }
    }

    private static IPEndPoint? CreateEndPoint(LocalEndPointSettings? settings)
    {
        if (settings is null || (string.IsNullOrWhiteSpace(settings.Address) && settings.Port.GetValueOrDefault() == 0))
        {
            return null;
        }

        return new IPEndPoint(IPAddress.Parse(settings.Address!), settings.Port ?? 0);
    }

    private static X509CertificateCollection LoadCertificates(CertificateFileSettings[] settings, Action<X509Certificate2> ownCertificate)
    {
        X509CertificateCollection collection = [];
        foreach (CertificateFileSettings item in settings)
        {
#if NET8_0
            X509Certificate2 certificate = new(item.Path!, item.Password, X509KeyStorageFlags.EphemeralKeySet);
#else
            X509Certificate2 certificate = X509CertificateLoader.LoadPkcs12FromFile(item.Path!, item.Password, X509KeyStorageFlags.EphemeralKeySet);
#endif
            try
            {
                ownCertificate(certificate);
                collection.Add(certificate);
            }
            catch
            {
                certificate.Dispose();
                throw;
            }
        }

        return collection;
    }

    private static IProxyClient CreateProxy(ProxyClientSettings settings, Action<X509Certificate2> ownCertificate)
    {
        ProxyCredentialsSettings? configuredCredentials = settings.ProxyCredentials;
        NetworkCredential? credentials = configuredCredentials is null ? null : new NetworkCredential(configuredCredentials.UserName, configuredCredentials.Password, configuredCredentials.Domain);
        IProxyClient proxy = settings.Type switch
        {
            "SystemProxy" => ProxyClient.SystemProxy,
            "Socks4Client" => credentials is null ? new Socks4Client(settings.ProxyHost!, settings.ProxyPort!.Value) : new Socks4Client(settings.ProxyHost!, settings.ProxyPort!.Value, credentials),
            "Socks4aClient" => credentials is null ? new Socks4aClient(settings.ProxyHost!, settings.ProxyPort!.Value) : new Socks4aClient(settings.ProxyHost!, settings.ProxyPort!.Value, credentials),
            "Socks5Client" => credentials is null ? new Socks5Client(settings.ProxyHost!, settings.ProxyPort!.Value) : new Socks5Client(settings.ProxyHost!, settings.ProxyPort!.Value, credentials),
            "HttpProxyClient" => credentials is null ? new HttpProxyClient(settings.ProxyHost!, settings.ProxyPort!.Value) : new HttpProxyClient(settings.ProxyHost!, settings.ProxyPort!.Value, credentials),
            "HttpsProxyClient" => credentials is null ? new HttpsProxyClient(settings.ProxyHost!, settings.ProxyPort!.Value) : new HttpsProxyClient(settings.ProxyHost!, settings.ProxyPort!.Value, credentials),
            _ => throw new ArgumentException($"Unsupported MailKit proxy client type: '{settings.Type}'.", nameof(settings))
        };
        IPEndPoint? localEndPoint = CreateEndPoint(settings.LocalEndPoint);
        if (localEndPoint is not null)
        {
            if (settings.Type == "SystemProxy")
            {
                throw new NotSupportedException("MailKit:ProxyClient:LocalEndPoint cannot be configured with SystemProxy.");
            }

            proxy.LocalEndPoint = localEndPoint;
        }

        if (proxy is HttpsProxyClient https)
        {
            if (settings.CheckCertificateRevocation.HasValue)
            {
                https.CheckCertificateRevocation = settings.CheckCertificateRevocation.Value;
            }

            if (settings.SslProtocols.HasValue)
            {
                https.SslProtocols = settings.SslProtocols.Value;
            }

            if (settings.ClientCertificates is { Length: > 0 })
            {
                https.ClientCertificates = LoadCertificates(settings.ClientCertificates, ownCertificate);
            }
        }
        else if (settings.CheckCertificateRevocation.HasValue || settings.SslProtocols.HasValue || settings.ClientCertificates is { Length: > 0 })
        {
            throw new NotSupportedException("MailKit:ProxyClient TLS settings require HttpsProxyClient.");
        }

        return proxy;
    }
}