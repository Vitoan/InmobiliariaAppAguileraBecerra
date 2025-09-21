using System.Collections.Generic;
using InmobiliariaAppAguileraBecerra.Models;

namespace InmobiliariaAppAguileraBecerra.Models
{
    public interface IRepositorioImagen
    {
        List<Imagen> BuscarPorInmueble(int idInmueble);
        void Agregar(Imagen imagen);
        void Eliminar(int id);
    }
}
