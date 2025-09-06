using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CajuAjuda.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddMergeFieldsToChamado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ChamadoPrincipalId",
                table: "Chamados",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chamados_ChamadoPrincipalId",
                table: "Chamados",
                column: "ChamadoPrincipalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chamados_Chamados_ChamadoPrincipalId",
                table: "Chamados",
                column: "ChamadoPrincipalId",
                principalTable: "Chamados",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chamados_Chamados_ChamadoPrincipalId",
                table: "Chamados");

            migrationBuilder.DropIndex(
                name: "IX_Chamados_ChamadoPrincipalId",
                table: "Chamados");

            migrationBuilder.DropColumn(
                name: "ChamadoPrincipalId",
                table: "Chamados");
        }
    }
}
