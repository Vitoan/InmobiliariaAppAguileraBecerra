using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using InmobiliariaAppAguileraBecerra.Models;

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
            return _context.imagen
                           .Where(i => i.InmuebleId == inmuebleId)
                           .ToList();
        }

        public void Agregar(Imagen imagen)
        {
            _context.imagen.Add(imagen);
            _context.SaveChanges();
        }

        public void Eliminar(int id)
        {
            var img = _context.imagen.Find(id);
            if (img != null)
            {
                _context.imagen.Remove(img);
                _context.SaveChanges();
            }
        }
    }
}
