using Microsoft.EntityFrameworkCore;

namespace InmobiliariaAppAguileraBecerra.Models
{
    public class InmobiliariaContext : DbContext
    {
        public InmobiliariaContext(DbContextOptions<InmobiliariaContext> options)
            : base(options)
        {
        }

        public DbSet<Inmueble> Inmuebles { get; set; }
        public DbSet<Propietario> Propietarios { get; set; }
        public DbSet<Imagen> imagen { get; set; }
    }
}
