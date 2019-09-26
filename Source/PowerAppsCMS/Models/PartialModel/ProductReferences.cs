using System.ComponentModel.DataAnnotations;

namespace PowerAppsCMS.Models
{
    [MetadataType(typeof(ProductReferencesDataAnotation))]
    public partial class ProductReferences
    {
    }

    public class ProductReferencesDataAnotation
    {
        [Required(ErrorMessage ="Product Reference is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Product Group is required")]
        public int ProductGroupID { get; set; }

        [Required(ErrorMessage = "Man Hour PB In House is required")]
        [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Man Hour PB In House field can't be less than 0")]
        public decimal MHPBIH { get; set; }

        [Required(ErrorMessage = "Man Hour PB Out House is required")]
        [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Man Hour PB Out House field can't be less than 0")]
        public decimal MHPBOH { get; set; }

        [Required(ErrorMessage = "Man Hour Fab In House is required")]
        [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Man Hour Fab in House field can't be less than 0")]
        public decimal MHFabIH { get; set; }

        [Required(ErrorMessage = "Man Hour Fab Out House is required")]
        [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Man Hour Fab Out House field can't be less than 0")]
        public decimal MHFabOH { get; set; }

        [Required(ErrorMessage = "Man Hour Painting In House is required")]
        [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Man Hour Painting in House field can't be less than 0")]
        public decimal MHPaintingIH { get; set; }

        [Required(ErrorMessage = "Man Hour Painting Out House is required")]
        [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Man Hour Painting Out House field can't be less than 0")]
        public decimal MHPaintingOH { get; set; }

        [Required(ErrorMessage = "Factor is required")]
        [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Factor field can't be less than 0")]
        public decimal Factor { get; set; }

        public bool IsOperatorOr { get; set; }
    }
}