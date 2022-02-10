using System.ComponentModel.DataAnnotations;
using WebApp.Validations;

namespace WebApp.DTOs
{
    public class LibroCreacionDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [PrimeraMayuscula]
        [StringLength(maximumLength: 120, ErrorMessage = "El campo {0} no debe tener más de {1} caracteres")]
        public string Titulo { get; set; }

        public DateTime FechaPublicacion { get; set; }

        public List<int> AutoresId { get; set; }
    }
}
