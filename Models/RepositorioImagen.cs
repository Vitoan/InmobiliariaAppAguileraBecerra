using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using InmobiliariaAppAguileraBecerra.Models; // <- esto es clave para que Reconozca Imagen e InmobiliariaContext

namespace InmobiliariaAppAguileraBecerra.Models
{
    public class RepositorioImagen : IRepositorioImagen
    {
        private readonly InmobiliariaContext _context;

        public RepositorioImagen(InmobiliariaContext context)
        {
            _context = context;
        }

        public List<Imagen> BuscarPorInmueble(int inmuebleId)
        {
            return _context.Imagenes
                           .Where(i => i.InmuebleId == inmuebleId)
                           .ToList();
        }

        public void Agregar(Imagen imagen)
        {
            _context.Imagenes.Add(imagen);
            _context.SaveChanges();
        }

        public void Eliminar(int id)
        {
            var img = _context.Imagenes.Find(id);
            if (img != null)
            {
                _context.Imagenes.Remove(img);
                _context.SaveChanges();
            }
        }
    }
}
