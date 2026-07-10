# CryptAByte

CryptAByte is a free, open-source C# / .NET toolkit for secure (encrypted) message and file sharing over the web using a public-key infrastructure (PKI). CryptAByte.com is the online drop box built on it; this repo also includes standalone Windows encryption utilities (CryptoPad, OneTimePad) and the shared crypto library behind them.

Messages and files are encrypted using a public key and can only be decrypted using the passphrase entered when your key is created. Your data is never stored in plaintext, and is impossible to decrypt without your passphrase.

==========

Crypt-A-Byte uses public-key encryption just like HTTPS/SSL and OpenPGP/PGP. We encourage you to use these tools whenever possible.

However, PGP requires that you and your recipient install and configure encryption software and create and exchange key pairs. When this is not practical, we make it very simple (and free) to share data using the same algorithms over the web.

We use RSA for key pairs, encrypt messages and files using AES 256, and SHA256 for hashing.

==========

**Components**

- **CryptAByte.WebUI** — the ASP.NET MVC web app behind CryptAByte.com (encrypted drop box).
- **CryptAByte.WindowsClient** — Windows client for the CryptAByte service.
- **CryptAByte.CryptoLibrary / CryptAByte.Domain** — shared RSA/AES/SHA256 encryption library (C#, .NET).
- **CryptoPad** is a simple AES256 encryption/decryption app.
- **OneTimePad** generates one time pads.

==========

**Getting Started**

Clone the repo and open `CryptAByte.sln` in Visual Studio, or build the individual projects from the CLI:

```
git clone https://github.com/DavidVeksler/CryptAByte.git
cd CryptAByte
dotnet build CryptAByte.Domain          # shared crypto library (net9.0)
dotnet build CryptAByte.CryptoLibrary
```

`CryptAByte.WebUI` and `CryptAByte.WindowsClient` target the legacy .NET Framework and are best built/run from Visual Studio.

==========

**Screenshots**

![CryptAByte Windows Client](https://raw.github.com/DavidVeksler/CryptAByte/master/screenshots/WinClient.png)

![CryptoPad](https://raw.github.com/DavidVeksler/CryptAByte/master/screenshots/cryptopad.png)

![OneTimePad](https://raw.github.com/DavidVeksler/CryptAByte/master/screenshots/OneTimePad.png)

![Website](https://raw.github.com/DavidVeksler/CryptAByte/master/screenshots/Website.png)



==========
**Using ILMerge to create a single binary**

*ILMerge.exe /target:CryptoPad /out:CryptoPad2.exe /targetplatform:"v4,C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0" CryptoPad.exe CryptAByte.CryptoLibrary.dll*

==========

**License**

MIT — see [LICENSE](LICENSE).
