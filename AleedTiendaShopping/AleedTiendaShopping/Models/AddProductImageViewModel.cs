using System.ComponentModel.DataAnnotations;

namespace AleedTiendaShopping.Models
{
    public class AddProductImageViewModel
    {
        public int ProductId { get; set; }

        [Display(Name = "Fotos")]
        public int? ImagesNumber { get; set; }

        [Display(Name = "Foto")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public IFormFile ImageFile { get; set; }

    }
}
