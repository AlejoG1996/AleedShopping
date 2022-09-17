namespace AleedTiendaShopping.Data.Entities
{
    public class ProductCategory
    {
        public int Id { get; set; }

        public Products Product { get; set; }

        public Category Category { get; set; }

    }
}
