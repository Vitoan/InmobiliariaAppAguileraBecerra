using System;
using System.ComponentModel.DataAnnotations;

namespace InmobiliariaAppAguileraBecerra.Models
{
    public class Auditoria
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Tabla { get; set; } = "";

        [Required]
        public string Operacion { get; set; } = "";

        [Display(Name = "ID Registro")]
        public int RegistroId { get; set; }

        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Display(Name = "Datos Anteriores")]
        public string? DatosAnteriores { get; set; }

        [Display(Name = "Datos Nuevos")]
        public string? DatosNuevos { get; set; }

        [Display(Name = "Usuario")]
        public string? Usuario { get; set; }
    }
}
