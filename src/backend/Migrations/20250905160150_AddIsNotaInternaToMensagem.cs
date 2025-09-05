using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CajuAjuda.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddIsNotaInternaToMensagem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsNotaInterna",
                table: "Mensagens",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "TecnicoResponsavelId",
                table: "Chamados",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chamados_TecnicoResponsavelId",
                table: "Chamados",
                column: "TecnicoResponsavelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chamados_Usuarios_TecnicoResponsavelId",
                table: "Chamados",
                column: "TecnicoResponsavelId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chamados_Usuarios_TecnicoResponsavelId",
                table: "Chamados");

            migrationBuilder.DropIndex(
                name: "IX_Chamados_TecnicoResponsavelId",
                table: "Chamados");

            migrationBuilder.DropColumn(
                name: "IsNotaInterna",
                table: "Mensagens");

            migrationBuilder.DropColumn(
                name: "TecnicoResponsavelId",
                table: "Chamados");
        }
    }
}
