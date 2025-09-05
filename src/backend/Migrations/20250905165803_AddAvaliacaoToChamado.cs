using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CajuAjuda.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddAvaliacaoToChamado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ComentarioAvaliacao",
                table: "Chamados",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NotaAvaliacao",
                table: "Chamados",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ComentarioAvaliacao",
                table: "Chamados");

            migrationBuilder.DropColumn(
                name: "NotaAvaliacao",
                table: "Chamados");
        }
    }
}
