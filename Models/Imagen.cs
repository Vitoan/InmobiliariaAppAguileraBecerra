using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace InmobiliariaAppAguileraBecerra.Models
{
    public class Imagen
    {
        public int Id { get; set; }
        public int InmuebleId { get; set; }
        public string Url { get; set; } = string.Empty;

        [NotMapped]
        public IFormFile? Archivo { get; set; }
    }
}
