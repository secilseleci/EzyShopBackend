namespace Business.Services.Abstract
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string body);
        Task<bool> SendSellerApprovedEmail(string to, string sellerName,string shopName);
        Task<bool> SendSellerRejectedEmail(string to, string sellerName, string shopName);
        Task<bool> SendSellerDeactivatedEmail(string to, string sellerName, string shopName);

        Task<bool> SendOrderConfirmationEmail(string to, string orderCode, string customerName);

    }
}
