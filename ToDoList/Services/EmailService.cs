using MailKit.Net.Smtp;
using MimeKit;

namespace ToDoList.Services;

public class EmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var smtpSettings = _configuration.GetSection("SmtpSettings");

        MimeMessage emailService = new MimeMessage();
        emailService.From.Add(new MailboxAddress(smtpSettings["SenderName"], smtpSettings["SenderEmail"]));
        emailService.To.Add(new MailboxAddress("" , email));
        emailService.Subject = subject;

        emailService.Body = new TextPart("html")
        {
            Text = $"<pre>{message}</pre>"
        };

        using SmtpClient client = new SmtpClient();

        try
        {
            await client.ConnectAsync(smtpSettings["Server"], int.Parse(smtpSettings["Port"]),
                MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(smtpSettings["UserName"], smtpSettings["Password"]);
            await client.SendAsync(emailService);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            await client.DisconnectAsync(true);
        }
    }
}