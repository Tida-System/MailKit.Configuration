# Esempi .NET 10

I progetti usano un `ProjectReference` diretto a `TidaSystems.MailKit.Configuration` e sono configurati per la pubblicazione Native AOT. Gli esempi SMTP, POP3 e IMAP usano le factory pubbliche della libreria senza adattarne le API.

## Configurazione sicura

I file `appsettings.json` contengono solo host e indirizzi di esempio. Modificarli con i dati non sensibili del proprio provider e fornire le credenziali tramite variabili d'ambiente, che hanno precedenza sul JSON:

```powershell
$env:MailKit__Username = "user@example.com"
$env:MailKit__Password = "password-o-app-password"
```

In alternativa, per OAuth 2.0 impostare `MailKit__OAuthToken` al posto della password. Non salvare credenziali nei file del repository. Per SMTP, aggiornare anche gli indirizzi del profilo `MailKit:Senders:Info` in `samples/SendEmail/appsettings.json`.

## Esecuzione

Dalla root del repository:

```powershell
dotnet run --project samples/SendEmail
dotnet run --project samples/ReadPop3
dotnet run --project samples/ReadImapInbox
```

`SendEmail` crea il messaggio dal profilo `Info` e usa `MimeKit.BodyBuilder` per includere sia la versione testuale sia quella HTML. Gli esempi POP3 e IMAP mostrano gli ultimi dieci messaggi; IMAP apre la Inbox in sola lettura.

Per verificare l'intera soluzione e la compatibilità AOT dichiarata:

```powershell
dotnet build TidaSystems.MailKit.Configuration.slnx
dotnet publish samples/SendEmail -c Release -r win-x64
dotnet publish samples/ReadPop3 -c Release -r win-x64
dotnet publish samples/ReadImapInbox -c Release -r win-x64
```

Sostituire `win-x64` con il runtime identifier della piattaforma di destinazione.
