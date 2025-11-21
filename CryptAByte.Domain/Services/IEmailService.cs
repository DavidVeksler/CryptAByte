namespace CryptAByte.Domain.Services
{
    public interface IEmailService
    {
        /// <summary>
        /// Sends an email notification.
        /// </summary>
        /// <param name="toAddress">Recipient email address.</param>
        /// <param name="subject">Email subject.</param>
        /// <param name="body">Email body content.</param>
        void SendEmail(string toAddress, string subject, string body);

        /// <summary>
        /// Sends an email notification from a specific sender.
        /// </summary>
        /// <param name="fromAddress">Sender email address.</param>
        /// <param name="toAddress">Recipient email address.</param>
        /// <param name="subject">Email subject.</param>
        /// <param name="body">Email body content.</param>
        void SendEmail(string fromAddress, string toAddress, string subject, string body);
    }
}
