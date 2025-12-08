Entrega final del proyecto - Seguimiento de Gestiones de Crédito:

**1. Integrantes del grupo:** ⦁ Valeria Chacón Aragón ⦁ Camila Corrales Roca ⦁ Diana Ramírez Aguilar ⦁ Vivian Michelle Velázquez Rojas
**2. Link del Repositorio:** https://github.com/Camilacr10/ProyectoFinal_ProgramacionAvanzadaWeb.git
## 3. Especificación básica del proyecto:
 

## Arquitectura del Proyecto

El proyecto está organizado con una arquitectura por capas dentro de una solución de Visual Studio, compuesta por tres proyectos principales:

### 1. ProyectoFinal (Capa de Presentación)
Proyecto web **ASP.NET Core MVC (net8.0)**. Aquí se encuentran:
- Los **Controllers** que manejan peticiones HTTP.
- Las **Views** en Razor.
- Archivos estáticos dentro de `wwwroot`.
- Configuración de autenticación, rutas, entre otros.

Esta capa es responsable de mostrar la información y recibir las solicitudes del usuario.

### 2. ProyectoFinalBLL (Capa de Lógica de Negocio)
Es una **Class Library (net8.0)** encargada de toda la lógica del sistema. Contiene:
- **DTOs** utilizados para transportar datos.
- **Interfaces de servicio** y sus implementaciones.
- **AutoMapper Profile** para el mapeo entre entidades y DTOs.

Aquí se centralizan las reglas de negocio, manteniendo los controladores limpios.

### 3. ProyectoFinalDAL (Capa de Datos)
Otra **Class Library (net8.0)** donde se maneja el acceso a datos. Incluye:
- El **DbContext** (`SgcDbContext`) de Entity Framework Core.
- Las **entidades** del modelo.
- Migraciones de la base de datos.
- **Repositorios** e interfaces para comunicación con SQL Server.

Esta capa se encarga completamente de la interacción con la base de datos.

---

## Librerías y Paquetes NuGet Utilizados

### ProyectoFinal (Web)
- `Microsoft.AspNetCore.Mvc`
- `Microsoft.AspNetCore.Authentication.Cookies`
- `Microsoft.EntityFrameworkCore`
- `Microsoft.EntityFrameworkCore.SqlServer`
- `Microsoft.EntityFrameworkCore.Tools`
- `Microsoft.VisualStudio.Web.CodeGeneration.Design`
- `System.ComponentModel.Annotations`
- **Front-end:** jQuery, SweetAlert2

### ProyectoFinalBLL (Lógica de Negocio)
- `AutoMapper`
- `Microsoft.EntityFrameworkCore.SqlServer`
- `Microsoft.AspNetCore.Authentication.Cookies`

### ProyectoFinalDAL (Acceso a Datos)
- `Microsoft.EntityFrameworkCore`
- `Microsoft.EntityFrameworkCore.SqlServer`
- `Microsoft.EntityFrameworkCore.Design`
- `Microsoft.EntityFrameworkCore.Tools`
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore`
- `Microsoft.Extensions.Identity.Core`

---

## Principios SOLID Aplicados

### S — Single Responsibility (Responsabilidad Única)
Cada clase cumple una sola función:
- Controllers → manejar solicitudes.
- Services → lógica de negocio.
- Repositories → acceso a datos.
- DTOs → transporte de información.

### O — Open/Closed (Abierto/Cerrado)
El código está preparado para extender funcionalidades sin modificar lo existente, gracias al uso de **interfaces** para servicios y repositorios.

### L — Liskov Substitution
Las implementaciones pueden sustituirse sin afectar el funcionamiento, ya que todo trabaja contra interfaces.

### I — Interface Segregation
Las interfaces están divididas por contexto:
- `IClienteService`
- `ISolicitudService`
- `IUsuarioService`
- `IFlujoSolicitudesServicio`  
Cada módulo implementa solo lo que realmente necesita.

### D — Dependency Inversion
Las capas superiores dependen de abstracciones, no de implementaciones concretas.  
Todo se inyecta mediante **Dependency Injection** en `Program.cs`.

---

## Patrones de Diseño Utilizados

### ✔ Patrón MVC
Organiza el proyecto web en Model–View–Controller.

### ✔ Repository Pattern
Los repositorios abstraen la lógica de acceso a datos:
- `ClienteRepository`
- `UsuarioRepository`
- `SolicitudRepository`
- `TrackingRepositorio`

### ✔ Service Layer
La lógica de negocio está en servicios como:
- `ClienteService`
- `SolicitudService`
- `UsuarioService`
- `FlujoSolicitudesServicio`

### ✔ DTO Pattern + AutoMapper
Se utilizan DTOs para no exponer las entidades directamente y AutoMapper para mapear entre capas.

### ✔ Arquitectura en Capas
Separación clara entre presentación, negocio y datos.

### ✔ Unit of Work (implícito mediante DbContext)
El `SgcDbContext` maneja transacciones agrupadas al ejecutar `SaveChanges`.

---

## Resumen General

El proyecto sigue buenas prácticas de arquitectura, separando responsabilidades mediante capas, utilizando Entity 
Framework Core para la persistencia, AutoMapper para simplificar los mapeos y aplicando los principios SOLID junto 
con patrones de diseño como MVC, Repository y Services. Esta estructura permite mantener el código ordenado, 
fácil de extender y más cómodo de mantener.
