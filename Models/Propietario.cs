using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace InmobiliariaAppAguileraBecerra.Models
{
    [Index(nameof(DNI), IsUnique = true)]
    public class Propietario
    {
        [Key]
        [Display(Name = "Código Int.")]
        public int Id { get; set; }

        [Required, StringLength(20)]
        public string DNI { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Apellido { get; set; } = string.Empty;

        [Required, Phone, StringLength(30)]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(255)]
        [DataType(DataType.Password)]
        public string Clave { get; set; } = string.Empty;

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