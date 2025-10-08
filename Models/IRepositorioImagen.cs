using System.Collections.Generic;
using InmobiliariaAppAguileraBecerra.Models;

namespace InmobiliariaAppAguileraBecerra.Models
{
    public interface IRepositorioImagen
    {
        List<Imagen> BuscarPorInmueble(int idInmueble);
        Imagen? ObtenerPorId(int id);
        void Agregar(Imagen imagen);
        void Eliminar(int id);
    }
}
