using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace CajuAjuda.Backend.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(
            _configuration["SmtpSettings:SenderName"], 
            _configuration["SmtpSettings:SenderEmail"]
        ));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = subject;
        email.Body = new TextPart(TextFormat.Html) { Text = body };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(
            _configuration["SmtpSettings:Server"], 
            _configuration.GetValue<int>("SmtpSettings:Port"), 
            SecureSocketOptions.StartTls
        );
        await smtp.AuthenticateAsync(
            _configuration["SmtpSettings:Username"], 
            _configuration["SmtpSettings:Password"]
        );
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}