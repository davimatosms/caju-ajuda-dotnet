using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CajuAjuda.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddSugestaoIAToChamado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SugestaoIA",
                table: "Chamados",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SugestaoIA",
                table: "Chamados");
        }
    }
}
