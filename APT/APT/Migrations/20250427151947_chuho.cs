using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLCCCC.Migrations
{
    /// <inheritdoc />
    public partial class chuho : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HinhAnh",
                table: "PhanAnhs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ChuHos",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_CuDan = table.Column<int>(type: "int", nullable: false),
                    ID_CanHo = table.Column<int>(type: "int", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChuHos", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ChuHos_CanHos_ID_CanHo",
                        column: x => x.ID_CanHo,
                        principalTable: "CanHos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChuHos_CuDans_ID_CuDan",
                        column: x => x.ID_CuDan,
                        principalTable: "CuDans",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChuHos_ID_CanHo",
                table: "ChuHos",
                column: "ID_CanHo");

            migrationBuilder.CreateIndex(
                name: "IX_ChuHos_ID_CuDan",
                table: "ChuHos",
                column: "ID_CuDan");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChuHos");

            migrationBuilder.DropColumn(
                name: "HinhAnh",
                table: "PhanAnhs");
        }
    }
}
