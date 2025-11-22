using System;
using System.Configuration;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace CryptAByte.Domain.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _defaultSenderAddress;

        public EmailService()
        {
            _defaultSenderAddress = ConfigurationManager.AppSettings["DefaultEmailSender"]
                                  ?? "webmaster@cryptabyte.com";
        }

        public EmailService(string defaultSenderAddress)
        {
            if (string.IsNullOrWhiteSpace(defaultSenderAddress))
                throw new ArgumentException("Sender address cannot be empty.", nameof(defaultSenderAddress));

            _defaultSenderAddress = defaultSenderAddress;
        }

        public Task SendEmailAsync(string recipientAddress, string subject, string bodyContent, CancellationToken cancellationToken = default)
        {
            return SendEmailAsync(_defaultSenderAddress, recipientAddress, subject, bodyContent, cancellationToken);
        }

        public async Task SendEmailAsync(string senderAddress, string recipientAddress, string subject, string bodyContent, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(senderAddress))
                throw new ArgumentException("Sender address cannot be empty.", nameof(senderAddress));

            if (string.IsNullOrWhiteSpace(recipientAddress))
                throw new ArgumentException("Recipient address cannot be empty.", nameof(recipientAddress));

            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Subject cannot be empty.", nameof(subject));

            using (var mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(senderAddress);
                mailMessage.To.Add(new MailAddress(recipientAddress));
                mailMessage.Subject = subject;
                mailMessage.Body = bodyContent;

                using (var smtpClient = new SmtpClient())
                {
                    await smtpClient.SendMailAsync(mailMessage).ConfigureAwait(false);
                }
            }
        }
    }
}
