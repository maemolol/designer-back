using Models;

public interface IEmailService
{
    Task SendPurchaseEmail(string email, List<Paintings> paintings);
}