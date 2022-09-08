using AleedTiendaShopping.Common;

namespace AleedTiendaShopping.Helpers
{
    public interface IMailHelper
    {
        Response SendMail(string toName, string toEmail, string subject, string body);
    }
}
