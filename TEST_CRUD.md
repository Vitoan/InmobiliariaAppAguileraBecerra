# Probar el CRUD

## Propietario

Accede a `http://localhost:5000/Propietario` para gestionar propietarios.

### Leer (Listar)
- **Ruta**: `/Propietario`
- Muestra una tabla con todos los propietarios.
- Verifica que los datos (`Id`, `Nombre`, `Apellido`, `DNI`, `Teléfono`, `Email`) se muestren correctamente.

### Crear
- **Ruta**: `/Propietario/Crear`
- Completa el formulario con:
  - **DNI**: Ejemplo, `12345678`.
  - **Nombre**: Ejemplo, `Juan`.
  - **Apellido**: Ejemplo, `Pérez`.
  - **Teléfono**: Ejemplo, `1234567890`.
  - **Email**: Ejemplo, `juan@example.com`.
  - **Clave**: Ejemplo, `contraseña123`.
- Haz clic en "Crear". Deberías ser redirigido a la lista con un mensaje de éxito.

### Actualizar
- **Ruta**: `/Propietario/Editar/{id}` (por ejemplo, `/Propietario/Editar/1`)
- Modifica algún campo (por ejemplo, cambia el `Teléfono` a `9876543210`).
- Haz clic en "Guardar". Verifica que el cambio se refleje en la lista.

### Eliminar
- **Ruta**: `/Propietario/Eliminar/{id}` (por ejemplo, `/Propietario/Eliminar/1`)
- Confirma la eliminación. El propietario debería desaparecer de la lista.

### Detalles
- **Ruta**: `/Propietario/Detalles/{id}` (por ejemplo, `/Propietario/Detalles/1`)
- Verifica que muestre todos los datos del propietario seleccionado.

## Inquilino

Accede a `http://localhost:5000/Inquilino` para gestionar inquilinos.

### Leer (Listar)
- **Ruta**: `/Inquilino`
- Muestra una tabla con todos los inquilinos.

### Crear
- **Ruta**: `/Inquilino/Crear`
- Completa el formulario con:
  - **DNI**: Ejemplo, `87654321`.
  - **Nombre**: Ejemplo, `María`.
  - **Apellido**: Ejemplo, `Gómez`.
  - **Teléfono**: Ejemplo, `5555555555` (opcional).
  - **Email**: Ejemplo, `maria@example.com` (opcional).
- Haz clic en "Crear".

### Actualizar
- **Ruta**: `/Inquilino/Editar/{id}`
- Modifica un campo y guarda.

### Eliminar
- **Ruta**: `/Inquilino/Eliminar/{id}`
- Confirma la eliminación.

### Detalles
- **Ruta**: `/Inquilino/Detalles/{id}`
- Verifica los datos del inquilino.