using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace InmobiliariaAppAguileraBecerra.Models
{
    [Index(nameof(DNI), IsUnique = true)]
    public class Inquilino
    {
        [Key]
        [Display(Name = "Código")]
        public int Id { get; set; }

        [Required, StringLength(20)]
        public string DNI { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Apellido { get; set; } = string.Empty;

        [Phone, StringLength(30)]
        public string? Telefono { get; set; }

        [EmailAddress, StringLength(100)]
        public string? Email { get; set; }

        public override string ToString()
        {
            var res = $"{Nombre} {Apellido}";
            if (!string.IsNullOrEmpty(DNI))
            {
                res += $" ({DNI})";
            }
            return res;
        }
    }
}