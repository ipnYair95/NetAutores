using System.ComponentModel.DataAnnotations;
using WebApp.Validations;

namespace WebApp.DTOs
{
    public class AutorCreacionDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 120, ErrorMessage = "El campo {0} no debe tener más de {1} caracteres")]
        [PrimeraMayuscula]
        public string Nombre { get; set; }
    }
}
