# 🛒 API Supermercado - ASP.NET Core

Una API RESTful completa para la gestión de un supermercado, desarrollada con **ASP.NET Core 8.0** y **Entity Framework Core**.

## 📋 Descripción del Proyecto

Esta API proporciona un sistema completo de gestión para supermercados que incluye:

### ✨ Características Principales

- **👥 Gestión de Usuarios**: Registro, autenticación y administración de empleados y administradores
- **📦 Gestión de Productos**: CRUD completo de productos con información detallada
- **🏷️ Categorías**: Organización de productos por categorías  
- **🏢 Proveedores**: Gestión de proveedores y sus productos asociados
- **📊 Historial de Stock**: Seguimiento de movimientos de inventario
- **🔐 Seguridad JWT**: Autenticación segura con tokens JWT
- **🛡️ Autorización por Roles**: Control de acceso basado en roles (Admin/Empleado)

### 🛠️ Tecnologías Utilizadas

- **Backend**: ASP.NET Core 8.0 Web API
- **Base de Datos**: SQL Server / LocalDB
- **ORM**: Entity Framework Core 8.0
- **Autenticación**: JWT (JSON Web Tokens)
- **Encriptación**: BCrypt para contraseñas
- **Documentación**: Swagger/OpenAPI 3.0
- **Arquitectura**: REST API con principios SOLID

### 📐 Arquitectura de la Base de Datos

La base de datos cuenta con **5 tablas principales**:

1. **Usuarios**: Gestión de usuarios del sistema
2. **Categorias**: Clasificación de productos  
3. **Proveedores**: Información de proveedores
4. **Productos**: Catálogo de productos del supermercado
5. **HistorialStock**: Registro de movimientos de inventario

**Relaciones implementadas**:
- Producto → Categoría (N:1)
- Producto → Proveedor (N:1)  
- HistorialStock → Producto (N:1)
- HistorialStock → Usuario (N:1)

---

## 🚀 Instrucciones de Instalación y Ejecución

### Prerrequisitos

Antes de comenzar, asegúrate de tener instalado:

- ✅ [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- ✅ [Visual Studio 2022](https://visualstudio.microsoft.com/) o [VS Code](https://code.visualstudio.com/)
- ✅ [SQL Server Express](https://www.microsoft.com/sql-server/sql-server-downloads) o LocalDB
- ✅ [Git](https://git-scm.com/) (opcional)

### 📥 Instalación

#### 1. Clonar el Repositorio
```bash
git clone [URL-DEL-REPOSITORIO]
cd SupermercadoAPI
```

#### 2. Restaurar Dependencias
```bash
dotnet restore
```

#### 3. Configurar la Base de Datos

**Opción A: LocalDB (Recomendado para desarrollo)**
La configuración por defecto ya está lista para LocalDB. No necesitas cambiar nada.

**Opción B: SQL Server**
Edita `appsettings.json` y cambia la cadena de conexión:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SupermercadoDB;Trusted_Connection=true;TrustServerCertificate=true"
  }
}
```

#### 4. Aplicar Migraciones
```bash
# Instalar herramientas EF (si no están instaladas)
dotnet tool install --global dotnet-ef

# Crear y aplicar migraciones
dotnet ef migrations add InicialCreacion
dotnet ef database update
```

#### 5. Ejecutar el Proyecto
```bash
dotnet run
```

O desde **Visual Studio**: Presiona `F5` o haz clic en el botón ▶️

### 🌐 Acceso a la API

Una vez ejecutado el proyecto:

- **API Base URL**: `https://localhost:7000/api`
- **Swagger UI**: `https://localhost:7000/swagger`
- **HTTP URL**: `http://localhost:5000/api` (sin HTTPS)

---

## 🔑 Datos de Prueba

### 👤 Usuarios Predefinidos

La aplicación incluye datos iniciales para facilitar las pruebas:

#### 🛡️ Administrador
```
Email: admin@supermercado.com  
Contraseña: Admin123!
Rol: Admin
```

**Permisos del Admin:**
- ✅ Acceso completo a todos los endpoints
- ✅ Crear, editar y eliminar cualquier recurso
- ✅ Gestionar usuarios
- ✅ Eliminar categorías, productos y proveedores

#### 👥 Crear Usuario Empleado

Para crear un empleado, usa el endpoint de registro o el admin puede crearlo:

**Usando el endpoint público:**
```http
POST /api/auth/register
Content-Type: application/json

{
  "nombre": "Juan Empleado",
  "email": "empleado@supermercado.com", 
  "password": "Empleado123!",
  "rol": "Empleado"
}
```

### 📦 Datos Iniciales Incluidos

#### Categorías Predefinidas
1. **Lácteos** - Productos lácteos y derivados
2. **Carnes** - Carnes rojas, blancas y embutidos  
3. **Frutas y Verduras** - Productos frescos
4. **Bebidas** - Bebidas alcohólicas y no alcohólicas
5. **Panadería** - Pan y productos de panadería

#### Proveedores Predefinidos
1. **Lácteos del Valle S.A.**
   - Teléfono: 0981-123456
   - Email: ventas@lacteosvalle.com

2. **Frigorífico Central**  
   - Teléfono: 0982-789012
   - Email: pedidos@frigorifico.com

3. **Distribuidora Frutas Frescas**
   - Teléfono: 0983-345678  
   - Email: info@frutasfrescas.com

---

## 🧪 Guía de Pruebas Rápidas

### 1. Probar Autenticación
```bash
# Login con admin
curl -X POST https://localhost:7000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@supermercado.com","password":"Admin123!"}'
```

### 2. Listar Categorías (sin autenticación)
```bash
curl -X GET https://localhost:7000/api/categorias
```

### 3. Crear Producto (con autenticación)
```bash
curl -X POST https://localhost:7000/api/productos \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "nombre": "Leche Entera 1L",
    "descripcion": "Leche entera pasteurizada",
    "precio": 2500.00,
    "stock": 50,
    "categoriaId": 1,
    "proveedorId": 1
  }'
```

### 4. Usando Swagger UI
1. Ve a `https://localhost:7000/swagger`
2. Haz clic en **"Authorize"** 
3. Ingresa: `Bearer {tu-token-jwt}`
4. ¡Ya puedes probar todos los endpoints!

---

## 📁 Estructura del Proyecto

```
SupermercadoAPI/
├── Controllers/          # Controladores de la API
│   ├── AuthController.cs
│   ├── CategoriasController.cs  
│   ├── ProductosController.cs
│   ├── ProveedoresController.cs
│   └── UsuariosController.cs
├── Data/                 # Configuración de EF Core
│   └── SupermercadoContext.cs
├── DTOs/                # Data Transfer Objects  
│   ├── UsuarioDto.cs
│   └── ProductoDto.cs
├── Models/              # Modelos de datos
│   ├── Usuario.cs
│   ├── Categoria.cs
│   ├── Proveedor.cs  
│   ├── Producto.cs
│   └── HistorialStock.cs
├── Middleware/          # Middleware personalizado
│   └── ErrorHandlingMiddleware.cs
├── Migrations/          # Migraciones de EF Core
├── appsettings.json     # Configuración
└── Program.cs          # Punto de entrada
```

---

## 🔧 Configuración Adicional

### Variables de Entorno
```bash
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=https://localhost:7000;http://localhost:5000
```

### Configuración JWT
El token JWT está configurado para durar **60 minutos** y incluye:
- ID del usuario
- Nombre y email  
- Rol (Admin/Empleado)
- Tiempo de expiración

### CORS
La API está configurada para permitir todas las origins en desarrollo. Para producción, configura origins específicos en `Program.cs`.

---

## 🐛 Troubleshooting

### Problemas Comunes

**Error: "No se puede conectar a la base de datos"**
```bash
# Verificar que LocalDB esté ejecutándose
sqllocaldb info mssqllocaldb
sqllocaldb start mssqllocaldb
```

**Error: "Entity Framework tools not found"**
```bash
dotnet tool install --global dotnet-ef
```

**Error: "Puerto en uso"**
```bash
# Cambiar puertos en launchSettings.json o usar:
dotnet run --urls="https://localhost:7001;http://localhost:5001"
```

### Logs
Los logs se guardan en la consola. Para debugging detallado, cambia el nivel en `appsettings.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  }
}
```

---

## 📊 Endpoints Principales

| Método | Endpoint | Descripción | Autenticación |
|--------|----------|-------------|---------------|
| POST | `/api/auth/login` | Iniciar sesión | No |
| POST | `/api/auth/register` | Registrar usuario | No |
| GET | `/api/productos` | Listar productos | No |
| POST | `/api/productos` | Crear producto | Sí |
| GET | `/api/categorias` | Listar categorías | No |
| DELETE | `/api/productos/{id}` | Eliminar producto | Admin |

**📋 Documentación completa:** `https://localhost:7000/swagger`

---

## 🎯 Próximos Pasos

1. **Prueba la API** usando Swagger UI
2. **Experimenta** con diferentes roles de usuario  
3. **Revisa los logs** para entender el flujo
4. **Explora** la documentación completa en Swagger

---

¡La API está lista para usar! 🎉 Si tienes problemas, revisa la sección de troubleshooting o consulta los logs de la aplicación.
