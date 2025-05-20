using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegisterStudents.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordToStudenttest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Agregar columna Password de tipo nvarchar(max), no nullable, con valor por defecto vacío
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Eliminar la columna Password en caso de rollback
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Students");
        }
    }
}
