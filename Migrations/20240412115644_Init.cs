using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ByteStorm.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Operativos",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    nombre = table.Column<string>(type: "TEXT", nullable: false),
                    rol = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operativos", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Misiones",
                columns: table => new
                {
                    codigo = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    descripcion = table.Column<string>(type: "TEXT", nullable: false),
                    estado = table.Column<string>(type: "TEXT", nullable: false),
                    idOperativo = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Misiones", x => x.codigo);
                    table.ForeignKey(
                        name: "FK_Misiones_Operativos_idOperativo",
                        column: x => x.idOperativo,
                        principalTable: "Operativos",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Equipos",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    tipo = table.Column<string>(type: "TEXT", nullable: false),
                    descripcion = table.Column<string>(type: "TEXT", nullable: false),
                    estado = table.Column<string>(type: "TEXT", nullable: false),
                    Misioncodigo = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipos", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Equipos_Misiones_Misioncodigo",
                        column: x => x.Misioncodigo,
                        principalTable: "Misiones",
                        principalColumn: "codigo");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Equipos_Misioncodigo",
                table: "Equipos",
                column: "Misioncodigo");

            migrationBuilder.CreateIndex(
                name: "IX_Misiones_idOperativo",
                table: "Misiones",
                column: "idOperativo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Equipos");

            migrationBuilder.DropTable(
                name: "Misiones");

            migrationBuilder.DropTable(
                name: "Operativos");
        }
    }
}
