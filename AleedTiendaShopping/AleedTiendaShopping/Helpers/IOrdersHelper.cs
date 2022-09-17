using AleedTiendaShopping.Common;
using AleedTiendaShopping.Models;

namespace AleedTiendaShopping.Helpers
{
    public interface IOrdersHelper
    {
        Task<Response> ProcessOrderAsync(ShowCartViewModel model);

        Task<Response> CancelOrderAsync(int id);


    }
}
