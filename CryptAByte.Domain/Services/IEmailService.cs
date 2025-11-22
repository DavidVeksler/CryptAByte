using System.Threading;
using System.Threading.Tasks;

namespace CryptAByte.Domain.Services
{
    public interface IEmailService
    {
        /// <summary>
        /// Asynchronously sends an email notification using the default sender address.
        /// </summary>
        /// <param name="recipientAddress">Recipient email address.</param>
        /// <param name="subject">Email subject.</param>
        /// <param name="bodyContent">Email body content.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        Task SendEmailAsync(string recipientAddress, string subject, string bodyContent, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously sends an email notification from a specific sender address.
        /// </summary>
        /// <param name="senderAddress">Sender email address.</param>
        /// <param name="recipientAddress">Recipient email address.</param>
        /// <param name="subject">Email subject.</param>
        /// <param name="bodyContent">Email body content.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        Task SendEmailAsync(string senderAddress, string recipientAddress, string subject, string bodyContent, CancellationToken cancellationToken = default);
    }
}
