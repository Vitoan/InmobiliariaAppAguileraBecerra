using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InmobiliariaAppAguileraBecerra.Models
{
    public enum enTipoInmueble
    {
        Casa = 1,
        Departamento = 2,
        Local = 3
    }

    public class Inmueble
    {
        [Key]
        [Display(Name = "Código")]
        public int Id { get; set; }

        [Required]
        public string Direccion { get; set; } = string.Empty;

        [Required]
        public string Uso { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Tipo de inmueble")]
        public int Tipo { get; set; }

        [NotMapped]
        public string TipoNombre => Tipo > 0 ? ((enTipoInmueble)Tipo).ToString() : string.Empty;

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

        public bool Habilitado { get; set; } = true;
        public string? Portada { get; set; }
        public List<Imagen>? Imagenes { get; set; }

        public static IDictionary<int, string> ObtenerTipos()
        {
            var tipos = new SortedDictionary<int, string>();
            Type tipoEnum = typeof(enTipoInmueble);
            foreach (var valor in Enum.GetValues(tipoEnum))
            {
                tipos.Add((int)valor, Enum.GetName(tipoEnum, valor) ?? string.Empty);
            }
            return tipos;
        }
    }
}
