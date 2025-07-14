using Business.Services.Abstract;
using Microsoft.Extensions.Configuration;
using Models.ViewModels;
using System.Net;
using System.Net.Mail;

namespace Business.Services.Concrete;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    #region SMTP gönderici
    public async Task<bool> SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpServer"])
            {
                Port = int.Parse(_configuration["EmailSettings:Port"]!),
                Credentials = new NetworkCredential(
                    _configuration["EmailSettings:SenderEmail"],
                    _configuration["EmailSettings:SenderPassword"]),
                EnableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"]!),
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:SenderEmail"], "EzyShop Support"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(to);

            await smtpClient.SendMailAsync(mailMessage);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Email send failed: {ex.Message}");
            return false;
        }
    }
    #endregion

    public async Task<bool> SendSellerApprovedEmail(string to, string sellerName, string shopName)
    {
        var subject = "Your Application has been Approved!";
        var body = $"""
        <h2>Hi {sellerName},</h2>
        <p>Your shop <strong>{shopName}</strong> has been approved.</p>
        <p>You can now login and start selling.</p>
        """;

        return await SendEmailAsync(to, subject, body);
    }

    public async Task<bool> SendSellerRejectedEmail(string to, string sellerName, string shopName)
    {
        var subject = "Your Application has been rejected";
        var body = $"""
        <h2>Hi {sellerName},</h2>
        <p>Your shop <strong>{shopName}</strong> has been rejected.</p>
        """;

        return await SendEmailAsync(to, subject, body);
    }

    public async Task<bool> SendSellerDeactivatedEmail(string to, string sellerName, string shopName)
    {
        var subject = "Your account has been suspended";
        var body = $"""
        <h2>Hi {sellerName},</h2>
        <p>Your shop <strong>{shopName}</strong> has been deactivated.</p>
        """;

        return await SendEmailAsync(to, subject, body);
    }

    public async Task<bool> SendOrderConfirmationEmail(string to, string orderCode, string customerName)
    {
        var subject = $"Order Confirmation - {orderCode}";
        var body = $"""
        <h2>Hi {customerName},</h2>
        <p>Thank you for your order. Your order code is <strong>{orderCode}</strong>.</p>
        <p>We'll notify you when your items are shipped.</p>
        """;

        return await SendEmailAsync(to, subject, body);
    }
}
