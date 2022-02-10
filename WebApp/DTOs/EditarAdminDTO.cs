using System.ComponentModel.DataAnnotations;

namespace WebApp.DTOs
{
    public class EditarAdminDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
