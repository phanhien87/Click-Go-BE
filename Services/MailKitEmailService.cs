using Click_Go.Helper;
using Click_Go.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Click_Go.Services
{
    public class MailKitEmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public MailKitEmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException("Recipient email is required", nameof(toEmail));

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Click&Go", _config["EmailSettings:SenderEmail"]));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject ?? "(No Subject)";

            var builder = new BodyBuilder { HtmlBody = htmlBody ?? "<p>(No content)</p>" };
            email.Body = builder.ToMessageBody();

            try
            {
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(
                    _config["EmailSettings:SMTPServer"],
                    int.Parse(_config["EmailSettings:Port"]),
                    SecureSocketOptions.StartTls);

                await smtp.AuthenticateAsync(
                    _config["EmailSettings:SenderEmail"],
                    _config["EmailSettings:SenderPassword"]);

                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch
            {
                throw new AppException("Lỗi gửi email!");
            }
        }

    }
}
