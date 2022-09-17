using AleedTiendaShopping.Common;
using AleedTiendaShopping.Data.Entities;

namespace AleedTiendaShopping.Models
{
    public class HomeViewModel
    {
        

        public float Quantity { get; set; }
        public ICollection<Category> Categories { get; set; }
        public PaginatedList<Products> Products { get; set; }

    }
}
