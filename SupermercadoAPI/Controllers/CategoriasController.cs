using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupermercadoAPI.Data;
using SupermercadoAPI.Models;

namespace SupermercadoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly SupermercadoContext _context;

        public CategoriasController(SupermercadoContext context)
        {
            _context = context;
        }

        // GET: api/categorias
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categoria>>> GetCategorias()
        {
            try
            {
                var categorias = await _context.Categorias
                    .Where(c => c.Activa)
                    .OrderBy(c => c.Nombre)
                    .ToListAsync();

                return Ok(categorias);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
            }
        }

        // GET: api/categorias/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Categoria>> GetCategoria(int id)
        {
            try
            {
                var categoria = await _context.Categorias.FindAsync(id);

                if (categoria == null || !categoria.Activa)
                {
                    return NotFound(new { message = "Categoría no encontrada" });
                }

                return Ok(categoria);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
            }
        }

        // POST: api/categorias
        [HttpPost]
        [Authorize] // Solo usuarios autenticados
        public async Task<ActionResult<Categoria>> PostCategoria(Categoria categoria)
        {
            try
            {
                // Verificar si ya existe una categoría con el mismo nombre
                if (await _context.Categorias.AnyAsync(c => c.Nombre.ToLower() == categoria.Nombre.ToLower() && c.Activa))
                {
                    return BadRequest(new { message = "Ya existe una categoría con ese nombre" });
                }

                categoria.FechaCreacion = DateTime.Now;
                categoria.Activa = true;

                _context.Categorias.Add(categoria);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetCategoria), new { id = categoria.Id }, categoria);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
            }
        }

        // PUT: api/categorias/5
        [HttpPut("{id}")]
        [Authorize] // Solo usuarios autenticados
        public async Task<IActionResult> PutCategoria(int id, Categoria categoria)
        {
            if (id != categoria.Id)
            {
                return BadRequest(new { message = "El ID no coincide" });
            }

            try
            {
                var categoriaExistente = await _context.Categorias.FindAsync(id);
                if (categoriaExistente == null)
                {
                    return NotFound(new { message = "Categoría no encontrada" });
                }

                // Verificar si ya existe otra categoría con el mismo nombre
                if (await _context.Categorias.AnyAsync(c => c.Nombre.ToLower() == categoria.Nombre.ToLower() && c.Id != id && c.Activa))
                {
                    return BadRequest(new { message = "Ya existe otra categoría con ese nombre" });
                }

                categoriaExistente.Nombre = categoria.Nombre;
                categoriaExistente.Descripcion = categoria.Descripcion;
                categoriaExistente.Activa = categoria.Activa;

                await _context.SaveChangesAsync();
                return Ok(categoriaExistente);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
            }
        }

        // DELETE: api/categorias/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Solo administradores
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            try
            {
                var categoria = await _context.Categorias.FindAsync(id);
                if (categoria == null)
                {
                    return NotFound(new { message = "Categoría no encontrada" });
                }

                // Verificar si tiene productos asociados
                var tieneProductos = await _context.Productos.AnyAsync(p => p.CategoriaId == id && p.Activo);
                if (tieneProductos)
                {
                    return BadRequest(new { message = "No se puede eliminar la categoría porque tiene productos asociados" });
                }

                // Eliminación lógica (cambiar estado a inactiva)
                categoria.Activa = false;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Categoría eliminada correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
            }
        }
    }
}