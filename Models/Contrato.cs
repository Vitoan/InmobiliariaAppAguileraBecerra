using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InmobiliariaAppAguileraBecerra.Models
{
    public class Contrato
    {
        [Key]
        [Display(Name = "Código")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Fecha de Inicio")]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }

        [Required]
        [Display(Name = "Fecha de Finalización")]
        [DataType(DataType.Date)]
        public DateTime FechaFin { get; set; }

        [Required]
        [Display(Name = "Monto Mensual")]
        public decimal Monto { get; set; }

        [Display(Name = "Finalización Anticipada")]
        [DataType(DataType.Date)]
        public DateTime? FechaFinAnticipada { get; set; }
        
        [Display(Name = "Multa")]
        public decimal? Multa { get; set; }

        [Display(Name = "Estado")]
        public bool Vigente { get; set; } = true;
        
        [Required]
        [Display(Name = "Inquilino")]
        public int InquilinoId { get; set; }

        [ForeignKey(nameof(InquilinoId))]
        public Inquilino? Inquilino { get; set; }

        [Required]
        [Display(Name = "Inmueble")]
        public int InmuebleId { get; set; }

        [ForeignKey(nameof(InmuebleId))]
        public Inmueble? Inmueble { get; set; }

        [NotMapped]
        public string? InquilinoNombre { get; set; }

        [NotMapped]
        public string? InmuebleDireccion { get; set; }
    }
}