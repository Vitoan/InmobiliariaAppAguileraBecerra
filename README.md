# 🏠 Proyecto Inmobiliaria – Gestión de Alquileres

![ASP.NET](https://img.shields.io/badge/Backend-ASP.NET%20Core-blue?logo=dotnet)
![MySQL](https://img.shields.io/badge/Database-MySQL-blue?logo=mysql)
![Bootstrap](https://img.shields.io/badge/Frontend-Bootstrap%205-purple?logo=bootstrap)
![License](https://img.shields.io/badge/License-MIT-green)
![Estado](https://img.shields.io/badge/Estado-En%20Desarrollo-yellow)

---

## 📘 Descripción

El **Proyecto Inmobiliaria** es una aplicación web desarrollada con **ASP.NET Core MVC** para informatizar la gestión de alquileres de propiedades.  
Permite administrar **inquilinos, propietarios, inmuebles, contratos y pagos**, incluyendo el cálculo automático de **multas por finalización anticipada**.


---

## 🚀 Tecnologías utilizadas

| Tipo | Tecnología |
|------|-------------|
| Lenguaje | C# (.NET 8 LTSC |
| Framework | ASP.NET Core MVC |
| Base de datos | MySQL |
| Frontend | HTML5, CSS3, Bootstrap 5 |
| Seguridad | Autenticación por roles (Administrador / Empleado) |
| Control de versiones | Git + GitHub |

---

## 🧩 Funcionalidades principales

- 👥 **ABM de Inquilinos, Propietarios e Inmuebles**
- 🏡 **Gestión de Contratos de Alquiler**
- 💸 **Registro y control de Pagos**
- ⏰ **Finalización anticipada con cálculo automático de multa**
- 🧾 **Auditoría** 
- 🔐 **Login de usuarios con roles y permisos**
- 📊 **Listados e informes** 

---

## ⚙️ Lógica de la multa por finalización anticipada

Según la narrativa del proyecto:

> Cuando un inquilino termina antes de tiempo su contrato:
>
> - Si cumplió **menos del 50% del plazo**, paga **2 meses de alquiler** de multa.  
> - Si cumplió **más del 50%**, paga **1 mes**.  
> - Además, se revisa si tiene **meses de alquiler adeudados**.  
> - El sistema muestra el detalle del cálculo y marca el contrato como **no vigente**.

📌 **Ejemplo:**

> Contrato de $100.000 mensuales, 12 meses.  
> El inquilino se retira al mes 4 habiendo pagado 3.  
> 
> - Multa: $200.000 (por menos del 50%)  
> - Deuda: $100.000 (1 mes)  
> - **Total a abonar: $300.000**
> 

---


## 🧠 Cómo ejecutar el proyecto

### 🪜 1. Clonar el repositorio

```bash
git clone https://github.com/tuusuario/InmobiliariaAppAguileraBecerra.git
cd InmobiliariaAppAguileraBecerra
````

### 🧩 2. Crear la base de datos en MySQL

```sql
CREATE DATABASE inmobiliaria_db;
USE inmobiliaria;
```

Importar `inmobiliaria db.sql`.

### ⚙️ 3. Configurar conexión en `appsettings.json`

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=inmobiliaria_db;User=root;Password=;"
}
```

### ▶️ 4. Ejecutar el proyecto

Desde terminal:

```bash
dotnet run
```

O desde Visual Studio:
🔹 Presionar **F5** → “Iniciar depuración”

### 🌐 5. Acceder desde el navegador

```     
http://localhost:5000/
```

---

## 🎨 Capturas sugeridas

Puedes agregar imágenes dentro de `wwwroot/img/` y referenciarlas aquí:

```markdown
![Pantalla principal](wwwroot/img/home.png)
![Detalles del contrato](wwwroot/img/detalle-contrato.png)
![Finalización anticipada](wwwroot/img/finalizacion.png)
```

---

## 👤 Autores

**Victor Angel Aguilera y Martin Becerra**
📚 Proyecto académico – *Programación Web 2 (2025)*
🔗 [GitHub](https://github.com/Vitoan/InmobiliariaAppAguileraBecerra)

---

> ✨ *“Digitalizando la gestión inmobiliaria, contrato a contrato.”*
