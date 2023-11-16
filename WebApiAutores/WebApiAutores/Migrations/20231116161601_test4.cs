using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiAutores.Migrations
{
    /// <inheritdoc />
    public partial class test4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "promedio",
                schema: "transaccional",
                table: "reviews");

            migrationBuilder.AlterColumn<double>(
                name: "puntuacion",
                schema: "transaccional",
                table: "reviews",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<double>(
                name: "valoracion",
                schema: "transaccional",
                table: "books",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "valoracion",
                schema: "transaccional",
                table: "books");

            migrationBuilder.AlterColumn<int>(
                name: "puntuacion",
                schema: "transaccional",
                table: "reviews",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<decimal>(
                name: "promedio",
                schema: "transaccional",
                table: "reviews",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
