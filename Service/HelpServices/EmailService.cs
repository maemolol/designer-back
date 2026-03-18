using MailKit.Net.Smtp;
using MimeKit;
using Models;

public class EmailService : IEmailService
{
    string senderAddress = Environment.GetEnvironmentVariable("SENDER_EMAIL") ?? "maemolol2@gmail.com";
    string senderPassword = Environment.GetEnvironmentVariable("SENDER_PASS") ?? "arfhaddhtswnbkke";
    string recipientAddress = Environment.GetEnvironmentVariable("RECIPIENT_EMAIL") ?? "maemolol2@gmail.com";

    public async Task SendPurchaseEmail(string formEmail, List<Paintings> paintings)
    {
        var body = $"New purchase\n\nE-mail: {formEmail}\n\n";

        foreach (var p in paintings)
        {
            body += $"{p.Name} - €{p.Price}\n";
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Gallery", senderAddress));
        message.To.Add(new MailboxAddress("", recipientAddress));
        message.Subject = "Your Painting Purchase";
        message.Body = new TextPart("plain") { Text = body };

        using var client = new SmtpClient();
        await client.ConnectAsync("smtp.gmail.com", 587, false);
        await client.AuthenticateAsync(senderAddress, senderPassword);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}