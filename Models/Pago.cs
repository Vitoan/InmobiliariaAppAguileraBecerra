using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InmobiliariaAppAguileraBecerra.Models
{
    public class Pago
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Número de Pago")]
        public int Numero { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Pago")]
        public DateTime Fecha { get; set; }

        [Required]
        [Display(Name = "Importe")]
        public decimal Importe { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Detalle")]
        public string Detalle { get; set; } = string.Empty;

        [Display(Name = "Anulado")]
        public bool Anulado { get; set; } = false;

        [Required]
        [Display(Name = "Contrato")]
        public int ContratoId { get; set; }

        [ForeignKey(nameof(ContratoId))]
        public Contrato? Contrato { get; set; }
    }
}
