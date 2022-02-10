using System.ComponentModel.DataAnnotations;
using WebApp.Validations;

namespace WebApp.Entity
{
    public class Libro
    {
        public int Id { get; set; }

        [Required]
        [PrimeraMayuscula]
        [StringLength(maximumLength: 120, ErrorMessage = "El campo {0} no debe tener más de {1} caracteres")]
        public string Titulo { get; set; }

        public DateTime? FechaPublicacion { get; set; }

        public List<Comentario> Comentarios { get; set; }

        public List<AutorLibro> AutoresLibros { get; set; }

    }
}
