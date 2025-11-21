using System;
using System.Configuration;
using System.Net.Mail;

namespace CryptAByte.Domain.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _defaultFromAddress;

        public EmailService()
        {
            // Default sender address - can be overridden via configuration
            _defaultFromAddress = ConfigurationManager.AppSettings["DefaultEmailSender"]
                                  ?? "webmaster@cryptabyte.com";
        }

        public EmailService(string defaultFromAddress)
        {
            if (string.IsNullOrWhiteSpace(defaultFromAddress))
                throw new ArgumentException("Default from address cannot be empty.", nameof(defaultFromAddress));

            _defaultFromAddress = defaultFromAddress;
        }

        public void SendEmail(string toAddress, string subject, string body)
        {
            SendEmail(_defaultFromAddress, toAddress, subject, body);
        }

        public void SendEmail(string fromAddress, string toAddress, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(fromAddress))
                throw new ArgumentException("From address cannot be empty.", nameof(fromAddress));

            if (string.IsNullOrWhiteSpace(toAddress))
                throw new ArgumentException("To address cannot be empty.", nameof(toAddress));

            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Subject cannot be empty.", nameof(subject));

            using (var mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(fromAddress);
                mailMessage.To.Add(new MailAddress(toAddress));
                mailMessage.Subject = subject;
                mailMessage.Body = body;

                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.Send(mailMessage);
                }
            }
        }
    }
}
