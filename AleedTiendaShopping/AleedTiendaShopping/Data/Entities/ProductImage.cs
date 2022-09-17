using System.ComponentModel.DataAnnotations;

namespace AleedTiendaShopping.Data.Entities
{
    public class ProductImage
    {
        public int Id { get; set; }

        public Products Product { get; set; }


        public String? Name { get; set; }
        //TODO: Pending to change to the correct path
        [Display(Name = "Foto")]
        public string? ImageFullPath { get; set; }

    }
}
