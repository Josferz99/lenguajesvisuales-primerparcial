using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupermercadoAPI.Data;
using SupermercadoAPI.DTOs;
using SupermercadoAPI.Models;

namespace SupermercadoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly SupermercadoContext _context;

        public ProductosController(SupermercadoContext context)
        {
            _context = context;
        }

        // GET: api/productos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> GetProductos()
        {
            try
            {
                var productos = await _context.Productos
                    .Include(p => p.Categoria)
                    .Include(p => p.Proveedor)
                    .Where(p => p.Activo)
                    .Select(p => new ProductoDto
                    {
                        Id = p.Id,
                        Nombre = p.Nombre,
                        Descripcion = p.Descripcion,
                        Precio = p.Precio,
                        Stock = p.Stock,
                        CategoriaId = p.CategoriaId,
                        CategoriaNombre = p.Categoria!.Nombre,
                        ProveedorId = p.ProveedorId,
                        ProveedorNombre = p.Proveedor!.Nombre,
                        FechaVencimiento = p.FechaVencimiento,
                        FechaCreacion = p.FechaCreacion,
                        Activo = p.Activo
                    })
                    .OrderBy(p => p.Nombre)
                    .ToListAsync();

                return Ok(productos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
            }
        }

        // GET: api/productos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDto>> GetProducto(int id)
        {
            try
            {
                var producto = await _context.Productos
                    .Include(p => p.Categoria)
                    .Include(p => p.Proveedor)
                    .Where(p => p.Id == id && p.Activo)
                    .Select(p => new ProductoDto
                    {
                        Id = p.Id,
                        Nombre = p.Nombre,
                        Descripcion = p.Descripcion,
                        Precio = p.Precio,
                        Stock = p.Stock,
                        CategoriaId = p.CategoriaId,
                        CategoriaNombre = p.Categoria!.Nombre,
                        ProveedorId = p.ProveedorId,
                        ProveedorNombre = p.Proveedor!.Nombre,
                        FechaVencimiento = p.FechaVencimiento,
                        FechaCreacion = p.FechaCreacion,
                        Activo = p.Activo
                    })
                    .FirstOrDefaultAsync();

                if (producto == null)
                {
                    return NotFound(new { message = "Producto no encontrado" });
                }

                return Ok(producto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
            }
        }

        // POST: api/productos
        [HttpPost]
        [Authorize] // Solo usuarios autenticados
        public async Task<ActionResult<ProductoDto>> PostProducto(ProductoCreateDto productoDto)
        {
            try
            {
                // Verificar que la categoría existe
                var categoria = await _context.Categorias.FindAsync(productoDto.CategoriaId);
                if (categoria == null || !categoria.Activa)
                {
                    return BadRequest(new { message = "La categoría especificada no existe" });
                }

                // Verificar que el proveedor existe
                var proveedor = await _context.Proveedores.FindAsync(productoDto.ProveedorId);
                if (proveedor == null || !proveedor.Activo)
                {
                    return BadRequest(new { message = "El proveedor especificado no existe" });
                }

                var producto = new Producto
                {
                    Nombre = productoDto.Nombre,
                    Descripcion = productoDto.Descripcion,
                    Precio = productoDto.Precio,
                    Stock = productoDto.Stock,
                    CategoriaId = productoDto.CategoriaId,
                    ProveedorId = productoDto.ProveedorId,
                    FechaVencimiento = productoDto.FechaVencimiento,
                    FechaCreacion = DateTime.Now,
                    Activo = true
                };

                _context.Productos.Add(producto);
                await _context.SaveChangesAsync();

                // Cargar las relaciones para la respuesta
                await _context.Entry(producto)
                    .Reference(p => p.Categoria)
                    .LoadAsync();
                await _context.Entry(producto)
                    .Reference(p => p.Proveedor)
                    .LoadAsync();

                var productoResponse = new ProductoDto
                {
                    Id = producto.Id,
                    Nombre = producto.Nombre,
                    Descripcion = producto.Descripcion,
                    Precio = producto.Precio,
                    Stock = producto.Stock,
                    CategoriaId = producto.CategoriaId,
                    CategoriaNombre = producto.Categoria!.Nombre,
                    ProveedorId = producto.ProveedorId,
                    ProveedorNombre = producto.Proveedor!.Nombre,
                    FechaVencimiento = producto.FechaVencimiento,
                    FechaCreacion = producto.FechaCreacion,
                    Activo = producto.Activo
                };

                return CreatedAtAction(nameof(GetProducto), new { id = producto.Id }, productoResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
            }
        }

        // PUT: api/productos/5
        [HttpPut("{id}")]
        [Authorize] // Solo usuarios autenticados
        public async Task<IActionResult> PutProducto(int id, ProductoUpdateDto productoDto)
        {
            try
            {
                var producto = await _context.Productos.FindAsync(id);
                if (producto == null)
                {
                    return NotFound(new { message = "Producto no encontrado" });
                }

                // Verificar que la categoría existe
                var categoria = await _context.Categorias.FindAsync(productoDto.CategoriaId);
                if (categoria == null || !categoria.Activa)
                {
                    return BadRequest(new { message = "La categoría especificada no existe" });
                }

                // Verificar que el proveedor existe
                var proveedor = await _context.Proveedores.FindAsync(productoDto.ProveedorId);
                if (proveedor == null || !proveedor.Activo)
                {
                    return BadRequest(new { message = "El proveedor especificado no existe" });
                }

                // Actualizar campos
                producto.Nombre = productoDto.Nombre;
                producto.Descripcion = productoDto.Descripcion;
                producto.Precio = productoDto.Precio;
                producto.Stock = productoDto.Stock;
                producto.CategoriaId = productoDto.CategoriaId;
                producto.ProveedorId = productoDto.ProveedorId;
                producto.FechaVencimiento = productoDto.FechaVencimiento;
                producto.Activo = productoDto.Activo;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Producto actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
            }
        }

        // DELETE: api/productos/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Solo administradores
        public async Task<IActionResult> DeleteProducto(int id)
        {
            try
            {
                var producto = await _context.Productos.FindAsync(id);
                if (producto == null)
                {
                    return NotFound(new { message = "Producto no encontrado" });
                }

                // Eliminación lógica
                producto.Activo = false;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Producto eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
            }
        }

        // GET: api/productos/categoria/5
        [HttpGet("categoria/{categoriaId}")]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> GetProductosPorCategoria(int categoriaId)
        {
            try
            {
                var productos = await _context.Productos
                    .Include(p => p.Categoria)
                    .Include(p => p.Proveedor)
                    .Where(p => p.CategoriaId == categoriaId && p.Activo)
                    .Select(p => new ProductoDto
                    {
                        Id = p.Id,
                        Nombre = p.Nombre,
                        Descripcion = p.Descripcion,
                        Precio = p.Precio,
                        Stock = p.Stock,
                        CategoriaId = p.CategoriaId,
                        CategoriaNombre = p.Categoria!.Nombre,
                        ProveedorId = p.ProveedorId,
                        ProveedorNombre = p.Proveedor!.Nombre,
                        FechaVencimiento = p.FechaVencimiento,
                        FechaCreacion = p.FechaCreacion,
                        Activo = p.Activo
                    })
                    .OrderBy(p => p.Nombre)
                    .ToListAsync();

                return Ok(productos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
            }
        }
    }
}