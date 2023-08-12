using System.ComponentModel.DataAnnotations;

namespace SecureCore_Backend.Modelo
{
    public class ClientInsurance
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo Id_Client es requerido")]
        public int Id_Client { get; set; }

        [Required(ErrorMessage = "El campo Id_Insurance es requerido")]
        public int Id_Insurance { get; set; }

        public Client Client { get; set; }
        public Insurance Insurance { get; set; }
    }
}
