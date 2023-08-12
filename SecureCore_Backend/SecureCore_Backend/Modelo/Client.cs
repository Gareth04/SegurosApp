using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SecureCore_Backend.Modelo
{
    public class Client
    {
        [Key]
        public int id_client { get; set; }

        [Required(ErrorMessage = "El campo cedula es obligatorio.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "El campo debe contener exactamente 10 números.")]
        public string cedula { get; set; }

        [Required(ErrorMessage = "El campo Nombre es obligatorio.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "El nombres debe contener solo letras.")]
        public string name { get; set; }

        [Required(ErrorMessage = "El campo telefono es obligatorio.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "El campo debe contener exactamente 10 números.")]
        public string phone { get; set; }

        [RegularExpression(@"^[0-9]+$", ErrorMessage = "El campo edad es obligatorio")]
        [Range(18, 99, ErrorMessage = "El campo edad debe contener solo números")]
        public int age { get; set; }
    }
}
