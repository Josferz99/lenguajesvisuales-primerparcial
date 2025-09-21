using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SupermercadoAPI.Migrations
{
    /// <inheritdoc />
    public partial class InicialCreacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activa = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Proveedores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Precio = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    CategoriaId = table.Column<int>(type: "int", nullable: false),
                    ProveedorId = table.Column<int>(type: "int", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Productos_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Productos_Proveedores_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistorialStock",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    TipoMovimiento = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialStock", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialStock_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HistorialStock_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "Id", "Activa", "Descripcion", "FechaCreacion", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "Productos lácteos y derivados", new DateTime(2025, 9, 21, 16, 44, 27, 777, DateTimeKind.Local).AddTicks(1282), "Lácteos" },
                    { 2, true, "Carnes rojas, blancas y embutidos", new DateTime(2025, 9, 21, 16, 44, 27, 777, DateTimeKind.Local).AddTicks(1306), "Carnes" },
                    { 3, true, "Productos frescos", new DateTime(2025, 9, 21, 16, 44, 27, 777, DateTimeKind.Local).AddTicks(1310), "Frutas y Verduras" },
                    { 4, true, "Bebidas alcohólicas y no alcohólicas", new DateTime(2025, 9, 21, 16, 44, 27, 777, DateTimeKind.Local).AddTicks(1312), "Bebidas" },
                    { 5, true, "Pan y productos de panadería", new DateTime(2025, 9, 21, 16, 44, 27, 777, DateTimeKind.Local).AddTicks(1315), "Panadería" }
                });

            migrationBuilder.InsertData(
                table: "Proveedores",
                columns: new[] { "Id", "Activo", "Direccion", "Email", "FechaCreacion", "Nombre", "Telefono" },
                values: new object[,]
                {
                    { 1, true, "Av. España 1234, Asunción", "ventas@lacteosvalle.com", new DateTime(2025, 9, 21, 16, 44, 27, 777, DateTimeKind.Local).AddTicks(1730), "Lácteos del Valle S.A.", "0981-123456" },
                    { 2, true, "Ruta 1 Km 25, San Lorenzo", "pedidos@frigorifico.com", new DateTime(2025, 9, 21, 16, 44, 27, 777, DateTimeKind.Local).AddTicks(1740), "Frigorífico Central", "0982-789012" },
                    { 3, true, "Mercado Central, Local 45", "info@frutasfrescas.com", new DateTime(2025, 9, 21, 16, 44, 27, 777, DateTimeKind.Local).AddTicks(1744), "Distribuidora Frutas Frescas", "0983-345678" }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Activo", "Email", "FechaCreacion", "Nombre", "Password", "Rol" },
                values: new object[] { 1, true, "admin@supermercado.com", new DateTime(2025, 9, 21, 16, 44, 28, 8, DateTimeKind.Local).AddTicks(5072), "Administrador", "$2a$11$fWbH7mh4cQ2XYcm72oLoJ.uEzdtE3avnUbDJN3MmdtW2h8be3hZNu", "Admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Categoria_Nombre",
                table: "Categorias",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistorialStock_ProductoId",
                table: "HistorialStock",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialStock_UsuarioId",
                table: "HistorialStock",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_CategoriaId",
                table: "Productos",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_ProveedorId",
                table: "Productos",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistorialStock");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropTable(
                name: "Proveedores");
        }
    }
}
