using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupermercadoAPI.Data;
using SupermercadoAPI.Models;

namespace SupermercadoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedoresController : ControllerBase
    {
        private readonly SupermercadoContext _context;

        public ProveedoresController(SupermercadoContext context)
        {
            _context = context;
        }

        // GET: api/proveedores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Proveedor>>> GetProveedores()
        {
            try
            {
                var proveedores = await _context.Proveedores
                    .Where(p => p.Activo)
                    .OrderBy(p => p.Nombre)
                    .ToListAsync();

                return Ok(proveedores);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
            }
        }

        // GET: api/proveedores/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Proveedor>> GetProveedor(int id)
        {
            try
            {
                var proveedor = await _context.Proveedores.FindAsync(id);

                if (proveedor == null || !proveedor.Activo)
                {
                    return NotFound(new { message = "Proveedor no encontrado" });
                }

                return Ok(proveedor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
            }
        }

        // GET: api/proveedores/5/productos
        [HttpGet("{id}/productos")]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductosPorProveedor(int id)
        {
            try
            {
                var proveedor = await _context.Proveedores.FindAsync(id);
                if (proveedor == null || !proveedor.Activo)
                {
                    return NotFound(new { message = "Proveedor no encontrado" });
                }

                var productos = await _context.Productos
                    .Include(p => p.Categoria)
                    .Where(p => p.ProveedorId == id && p.Activo)
                    .OrderBy(p => p.Nombre)
                    .ToListAsync();

                return Ok(productos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
            }
        }

        // POST: api/proveedores
        [HttpPost]
        [Authorize] // Solo usuarios autenticados
        public async Task<ActionResult<Proveedor>> PostProveedor(Proveedor proveedor)
        {
            try
            {
                // Verificar si ya existe un proveedor con el mismo email
                if (!string.IsNullOrEmpty(proveedor.Email) &&
                    await _context.Proveedores.AnyAsync(p => p.Email!.ToLower() == proveedor.Email.ToLower() && p.Activo))
                {
                    return BadRequest(new { message = "Ya existe un proveedor con ese email" });
                }

                proveedor.FechaCreacion = DateTime.Now;
                proveedor.Activo = true;

                _context.Proveedores.Add(proveedor);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetProveedor), new { id = proveedor.Id }, proveedor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
            }
        }

        // PUT: api/proveedores/5
        [HttpPut("{id}")]
        [Authorize] // Solo usuarios autenticados
        public async Task<IActionResult> PutProveedor(int id, Proveedor proveedor)
        {
            if (id != proveedor.Id)
            {
                return BadRequest(new { message = "El ID no coincide" });
            }

            try
            {
                var proveedorExistente = await _context.Proveedores.FindAsync(id);
                if (proveedorExistente == null)
                {
                    return NotFound(new { message = "Proveedor no encontrado" });
                }

                // Verificar si ya existe otro proveedor con el mismo email
                if (!string.IsNullOrEmpty(proveedor.Email) &&
                    await _context.Proveedores.AnyAsync(p => p.Email!.ToLower() == proveedor.Email.ToLower() && p.Id != id && p.Activo))
                {
                    return BadRequest(new { message = "Ya existe otro proveedor con ese email" });
                }

                proveedorExistente.Nombre = proveedor.Nombre;
                proveedorExistente.Telefono = proveedor.Telefono;
                proveedorExistente.Email = proveedor.Email;
                proveedorExistente.Direccion = proveedor.Direccion;
                proveedorExistente.Activo = proveedor.Activo;

                await _context.SaveChangesAsync();
                return Ok(proveedorExistente);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
            }
        }

        // DELETE: api/proveedores/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Solo administradores
        public async Task<IActionResult> DeleteProveedor(int id)
        {
            try
            {
                var proveedor = await _context.Proveedores.FindAsync(id);
                if (proveedor == null)
                {
                    return NotFound(new { message = "Proveedor no encontrado" });
                }

                // Verificar si tiene productos asociados
                var tieneProductos = await _context.Productos.AnyAsync(p => p.ProveedorId == id && p.Activo);
                if (tieneProductos)
                {
                    return BadRequest(new { message = "No se puede eliminar el proveedor porque tiene productos asociados" });
                }

                // Eliminación lógica
                proveedor.Activo = false;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Proveedor eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
            }
        }
    }
}