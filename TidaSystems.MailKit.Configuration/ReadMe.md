{
  "MailKit": {
    "Username": "user@example.com",
    "Password": "",
    "OAuthToken": "",
    "AuthenticationMechanisms": [],
    "Timeout": 30000,
    "LocalEndPoint": {
      "Address": "",
      "Port": 0
    },
    "CheckCertificateRevocation": true,
    "SslProtocols": "None",
    "SslCipherSuitesPolicy": [],
    "ClientCertificates": [],
    "ProtocolLogPath": "",
    "ProxyClient": null,
    "Smtp": {
      "Host": "smtp.example.com",
      "Port": 587,
      "SecureSocketOptions": "StartTls",
      "LocalDomain": "",
      "RequireTLS": false
    },
    "Pop3": {
      "Host": "pop.example.com",
      "Port": 995,
      "SecureSocketOptions": "SslOnConnect"
    },
    "Imap": {
      "Host": "imap.example.com",
      "Port": 993,
      "SecureSocketOptions": "SslOnConnect"
    }
  },
    "Senders": {
          "Info": {
            "From": {
              "Name": "TidaSystems Info",
              "Address": "info@example.com"
            },
            "ReplyTo": [
              {
                "Name": "Support",
                "Address": "support@example.com"
              }
            ],
            "To": [],
            "Cc": [
              {
                "Name": "Administration",
                "Address": "admin@example.com"
              }
            ],
            "Bcc": [
              {
                "Address": "archive@example.com"
              }
            ],
            "Subject": null,
            "Importance": "Normal",
            "Priority": "Normal"
          },
          "Billing": {
            "From": {
              "Name": "TidaSystems Billing",
              "Address": "billing@example.com"
            },
            "ReplyTo": [
              {
                "Address": "accounting@example.com"
              }
            ],
            "To": [],
            "Cc": [],
            "Bcc": []
          }
        }
      }
  }