using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaApp.Models
{
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
        public string? Telefono { get; set; }

        [Required, EmailAddress, StringLength(100)]
        public string? Email { get; set; }
        
        // Propiedad 'Clave' añadida para coincidir con la base de datos.
        [Required, StringLength(255)]
        [DataType(DataType.Password)]
        public string? Clave { get; set; }
        
        public override string ToString()
        {

            var res = $"{Nombre} {Apellido}";
            if (!String.IsNullOrEmpty(DNI))
            {
                res += $" ({DNI})";
            }
            return res;
        }
    }
}
