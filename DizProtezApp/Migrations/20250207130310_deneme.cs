using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DizProtezApp.Migrations
{
    /// <inheritdoc />
    public partial class deneme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Force",
                table: "PlcData",
                newName: "ForceFemoralN");

            migrationBuilder.AddColumn<double>(
                name: "ForceAxialN",
                table: "PlcData",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ForceAxialN",
                table: "PlcData");

            migrationBuilder.RenameColumn(
                name: "ForceFemoralN",
                table: "PlcData",
                newName: "Force");
        }
    }
}
