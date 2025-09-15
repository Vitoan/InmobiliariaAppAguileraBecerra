using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InmobiliariaAppAguileraBecerra.Models
{
    public class Inmueble
    {
        [Key]
        [Display(Name = "Código")]
        public int Id { get; set; }

        [Required]
        public string Direccion { get; set; } = string.Empty;

        [Required]
        public string Uso { get; set; } = string.Empty; // "Comercial" o "Residencial"

        [Required]
        public string Tipo { get; set; } = string.Empty; // "Local", "Casa", "Departamento"

        public int Ambientes { get; set; }
        
        public decimal Latitud { get; set; }

        public decimal Longitud { get; set; }

        [Required]
        public decimal Precio { get; set; }

        [Required]
        public bool Disponible { get; set; } = true;

        [Required]
        public int PropietarioId { get; set; }

        [ForeignKey(nameof(PropietarioId))]
        public Propietario? Duenio { get; set; }
    }
}