using MailKit.Net.Smtp;
using MimeKit;


namespace Origami.Identity.Api.Core.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            // Email:Provider = "Graph" envía por Microsoft Graph; cualquier otro valor (o vacío) sigue por SMTP.
            // Así se regresa a SMTP cambiando un App Setting, sin volver a publicar.
            if (string.Equals(_config["Email:Provider"], "Graph", StringComparison.OrdinalIgnoreCase))
            {
                await GraphMailClient.EnviarAsync(GraphMailClient.LeerConfiguracion(_config), to, subject, body);
                return;
            }

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config["Email:From"]));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_config["Email:SmtpHost"], int.Parse(_config["Email:SmtpPort"]), MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_config["Email:SmtpUser"], _config["Email:SmtpPass"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }



}