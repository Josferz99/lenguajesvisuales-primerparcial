using Microsoft.EntityFrameworkCore;
using SupermercadoAPI.Models;

namespace SupermercadoAPI.Data
{
    public class SupermercadoContext : DbContext
    {
        public SupermercadoContext(DbContextOptions<SupermercadoContext> options) : base(options)
        {
        }

        // DbSets - Representan nuestras tablas en la base de datos
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<HistorialStock> HistorialStock { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de relaciones y restricciones

            // Relación Producto -> Categoria (Many to One)
            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Categoria)
                .WithMany(c => c.Productos)
                .HasForeignKey(p => p.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict); // No permite eliminar categoría si tiene productos

            // Relación Producto -> Proveedor (Many to One)
            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Proveedor)
                .WithMany(pr => pr.Productos)
                .HasForeignKey(p => p.ProveedorId)
                .OnDelete(DeleteBehavior.Restrict); // No permite eliminar proveedor si tiene productos

            // Relación HistorialStock -> Producto (Many to One)
            modelBuilder.Entity<HistorialStock>()
                .HasOne(h => h.Producto)
                .WithMany(p => p.HistorialStock)
                .HasForeignKey(h => h.ProductoId)
                .OnDelete(DeleteBehavior.Cascade); // Si se elimina producto, se elimina historial

            // Relación HistorialStock -> Usuario (Many to One)
            modelBuilder.Entity<HistorialStock>()
                .HasOne(h => h.Usuario)
                .WithMany()
                .HasForeignKey(h => h.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict); // No permite eliminar usuario si tiene movimientos

            // Configuraciones adicionales

            // Índices únicos
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("IX_Usuario_Email");

            modelBuilder.Entity<Categoria>()
                .HasIndex(c => c.Nombre)
                .IsUnique()
                .HasDatabaseName("IX_Categoria_Nombre");

            // Configuración de precisión para decimales
            modelBuilder.Entity<Producto>()
                .Property(p => p.Precio)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<HistorialStock>()
                .Property(h => h.PrecioUnitario)
                .HasColumnType("decimal(10,2)");

            // Datos iniciales (Seed Data)
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Categorías iniciales
            modelBuilder.Entity<Categoria>().HasData(
                new Categoria { Id = 1, Nombre = "Lácteos", Descripcion = "Productos lácteos y derivados" },
                new Categoria { Id = 2, Nombre = "Carnes", Descripcion = "Carnes rojas, blancas y embutidos" },
                new Categoria { Id = 3, Nombre = "Frutas y Verduras", Descripcion = "Productos frescos" },
                new Categoria { Id = 4, Nombre = "Bebidas", Descripcion = "Bebidas alcohólicas y no alcohólicas" },
                new Categoria { Id = 5, Nombre = "Panadería", Descripcion = "Pan y productos de panadería" }
            );

            // Proveedores iniciales
            modelBuilder.Entity<Proveedor>().HasData(
                new Proveedor { Id = 1, Nombre = "Lácteos del Valle S.A.", Telefono = "0981-123456", Email = "ventas@lacteosvalle.com", Direccion = "Av. España 1234, Asunción" },
                new Proveedor { Id = 2, Nombre = "Frigorífico Central", Telefono = "0982-789012", Email = "pedidos@frigorifico.com", Direccion = "Ruta 1 Km 25, San Lorenzo" },
                new Proveedor { Id = 3, Nombre = "Distribuidora Frutas Frescas", Telefono = "0983-345678", Email = "info@frutasfrescas.com", Direccion = "Mercado Central, Local 45" }
            );

            // Usuario administrador inicial
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    Nombre = "Administrador",
                    Email = "admin@supermercado.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Admin123!"), // Contraseña encriptada
                    Rol = "Admin",
                    FechaCreacion = DateTime.Now
                }
            );
        }
    }
}