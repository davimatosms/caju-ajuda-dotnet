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
        Console.WriteLine($"[EMAIL] Preparando email para: {toEmail}");
        Console.WriteLine($"[EMAIL] Assunto: {subject}");
        Console.WriteLine($"[EMAIL] Body length: {body.Length} caracteres");
        
        // Vamos verificar se o token está no body
        if (body.Contains("token="))
        {
            var tokenStart = body.IndexOf("token=") + 6;
            var tokenEnd = body.IndexOf("'", tokenStart);
            if (tokenEnd == -1) tokenEnd = body.IndexOf("\"", tokenStart);
            if (tokenEnd == -1) tokenEnd = body.IndexOf("&", tokenStart);
            if (tokenEnd == -1) tokenEnd = Math.Min(tokenStart + 100, body.Length);
            
            var tokenPreview = body.Substring(tokenStart, Math.Min(50, tokenEnd - tokenStart));
            Console.WriteLine($"[EMAIL] Token no body: '{tokenPreview}'");
        }
        
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
        
        Console.WriteLine($"[EMAIL] Email enviado com sucesso para: {toEmail}");
    }
}