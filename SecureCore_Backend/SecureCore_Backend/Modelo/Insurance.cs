using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace SecureCore_Backend.Modelo
{
    public class Insurance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required(ErrorMessage = "El campo Nombre es obligatorio.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "El nombres debe contener solo letras.")]
        [MinLength(1)]
        public string name { get; set; }

        [Required(ErrorMessage = "El campo suma es obligatorio.")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "El campo suma debe contener números con hasta 2 decimales")]
        public decimal sum_Insured { get; set; }

        [Required(ErrorMessage = "El campo prima es obligatorio.")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "El campo prima debe contener números con hasta 2 decimales")]
        public decimal Premium { get; set; } // Porcentaje que pago por el seguro
        
    }
}
