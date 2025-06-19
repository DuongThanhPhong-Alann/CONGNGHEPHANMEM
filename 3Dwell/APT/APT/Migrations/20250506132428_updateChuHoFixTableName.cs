using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLCCCC.Migrations
{
    /// <inheritdoc />
    public partial class updateChuHoFixTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChungCus",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ten = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChuDauTu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NamXayDung = table.Column<int>(type: "int", nullable: true),
                    SoTang = table.Column<int>(type: "int", nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChungCus", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DichVus",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDichVu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gia = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DichVus", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TinTucs",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TieuDe = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayDang = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgaySuKien = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HinhAnh = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TinTucs", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CanHos",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaCan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ID_ChungCu = table.Column<int>(type: "int", nullable: false),
                    DienTich = table.Column<float>(type: "real", nullable: false),
                    SoPhong = table.Column<int>(type: "int", nullable: false),
                    Gia = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    URLs = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanHos", x => x.ID);
                    table.CheckConstraint("CK_CanHo_TrangThai", "TrangThai IN (N'Đang bán', N'Đã bán', N'Cho thuê', N'Đã thuê')");
                    table.ForeignKey(
                        name: "FK_CanHos_ChungCus_ID_ChungCu",
                        column: x => x.ID_ChungCu,
                        principalTable: "ChungCus",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HinhAnhChungCus",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_ChungCu = table.Column<int>(type: "int", nullable: false),
                    DuongDan = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HinhAnhChungCus", x => x.ID);
                    table.ForeignKey(
                        name: "FK_HinhAnhChungCus_ChungCus_ID_ChungCu",
                        column: x => x.ID_ChungCu,
                        principalTable: "ChungCus",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HinhAnhDichVus",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DuongDan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ID_DichVu = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HinhAnhDichVus", x => x.ID);
                    table.ForeignKey(
                        name: "FK_HinhAnhDichVus_DichVus_ID_DichVu",
                        column: x => x.ID_DichVu,
                        principalTable: "DichVus",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HinhAnhCanHos",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DuongDan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ID_CanHo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HinhAnhCanHos", x => x.ID);
                    table.ForeignKey(
                        name: "FK_HinhAnhCanHos_CanHos_ID_CanHo",
                        column: x => x.ID_CanHo,
                        principalTable: "CanHos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HoaDonDichVus",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_CanHo = table.Column<int>(type: "int", nullable: false),
                    ID_ChungCu = table.Column<int>(type: "int", nullable: false),
                    SoTien = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NgayLap = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDonDichVus", x => x.ID);
                    table.CheckConstraint("CK_HoaDonDichVu_TrangThai", "TrangThai IN (N'Chưa thanh toán', N'Đã thanh toán')");
                    table.ForeignKey(
                        name: "FK_HoaDonDichVus_CanHos_ID_CanHo",
                        column: x => x.ID_CanHo,
                        principalTable: "CanHos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoaDonDichVus_ChungCus_ID_ChungCu",
                        column: x => x.ID_ChungCu,
                        principalTable: "ChungCus",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "HoaDonDichVu_DichVus",
                columns: table => new
                {
                    ID_HoaDon = table.Column<int>(type: "int", nullable: false),
                    ID_DichVu = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDonDichVu_DichVus", x => new { x.ID_HoaDon, x.ID_DichVu });
                    table.ForeignKey(
                        name: "FK_HoaDonDichVu_DichVus_DichVus_ID_DichVu",
                        column: x => x.ID_DichVu,
                        principalTable: "DichVus",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoaDonDichVu_DichVus_HoaDonDichVus_ID_HoaDon",
                        column: x => x.ID_HoaDon,
                        principalTable: "HoaDonDichVus",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChuHos",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_CuDan = table.Column<int>(type: "int", nullable: false),
                    ID_CanHo = table.Column<int>(type: "int", nullable: false),
                    ID_ChungCu = table.Column<int>(type: "int", nullable: false),
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
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ChuHos_ChungCus_ID_ChungCu",
                        column: x => x.ID_ChungCu,
                        principalTable: "ChungCus",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "CuDans",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_NguoiDung = table.Column<int>(type: "int", nullable: false),
                    ID_CanHo = table.Column<int>(type: "int", nullable: false),
                    ID_ChungCu = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuDans", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CuDans_CanHos_ID_CanHo",
                        column: x => x.ID_CanHo,
                        principalTable: "CanHos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CuDans_ChungCus_ID_ChungCu",
                        column: x => x.ID_ChungCu,
                        principalTable: "ChungCus",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NguoiDungs",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiNguoiDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CuDanID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDungs", x => x.ID);
                    table.CheckConstraint("CK_NguoiDung_LoaiNguoiDung", "LoaiNguoiDung IN (N'Cư dân', N'Ban quản lý', N'Khách')");
                    table.ForeignKey(
                        name: "FK_NguoiDungs_CuDans_CuDanID",
                        column: x => x.CuDanID,
                        principalTable: "CuDans",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "PhanAnhs",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_NguoiDung = table.Column<int>(type: "int", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<int>(type: "int", nullable: false),
                    NgayGui = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PhanHoi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HinhAnh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TinTucID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhanAnhs", x => x.ID);
                    table.CheckConstraint("CK_PhanAnh_TrangThai", "TrangThai IN (0, 1, 2)");
                    table.ForeignKey(
                        name: "FK_PhanAnhs_NguoiDungs_ID_NguoiDung",
                        column: x => x.ID_NguoiDung,
                        principalTable: "NguoiDungs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhanAnhs_TinTucs_TinTucID",
                        column: x => x.TinTucID,
                        principalTable: "TinTucs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CanHos_ID_ChungCu",
                table: "CanHos",
                column: "ID_ChungCu");

            migrationBuilder.CreateIndex(
                name: "IX_CanHos_MaCan_ID_ChungCu",
                table: "CanHos",
                columns: new[] { "MaCan", "ID_ChungCu" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChuHos_ID_CanHo",
                table: "ChuHos",
                column: "ID_CanHo");

            migrationBuilder.CreateIndex(
                name: "IX_ChuHos_ID_ChungCu",
                table: "ChuHos",
                column: "ID_ChungCu");

            migrationBuilder.CreateIndex(
                name: "IX_ChuHos_ID_CuDan",
                table: "ChuHos",
                column: "ID_CuDan");

            migrationBuilder.CreateIndex(
                name: "IX_CuDans_ID_CanHo",
                table: "CuDans",
                column: "ID_CanHo");

            migrationBuilder.CreateIndex(
                name: "IX_CuDans_ID_ChungCu",
                table: "CuDans",
                column: "ID_ChungCu");

            migrationBuilder.CreateIndex(
                name: "IX_CuDans_ID_NguoiDung",
                table: "CuDans",
                column: "ID_NguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_HinhAnhCanHos_ID_CanHo",
                table: "HinhAnhCanHos",
                column: "ID_CanHo");

            migrationBuilder.CreateIndex(
                name: "IX_HinhAnhChungCus_ID_ChungCu",
                table: "HinhAnhChungCus",
                column: "ID_ChungCu");

            migrationBuilder.CreateIndex(
                name: "IX_HinhAnhDichVus_ID_DichVu",
                table: "HinhAnhDichVus",
                column: "ID_DichVu");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDonDichVu_DichVus_ID_DichVu",
                table: "HoaDonDichVu_DichVus",
                column: "ID_DichVu");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDonDichVus_ID_CanHo",
                table: "HoaDonDichVus",
                column: "ID_CanHo");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDonDichVus_ID_ChungCu",
                table: "HoaDonDichVus",
                column: "ID_ChungCu");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDungs_CuDanID",
                table: "NguoiDungs",
                column: "CuDanID");

            migrationBuilder.CreateIndex(
                name: "IX_PhanAnhs_ID_NguoiDung",
                table: "PhanAnhs",
                column: "ID_NguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_PhanAnhs_TinTucID",
                table: "PhanAnhs",
                column: "TinTucID");

            migrationBuilder.AddForeignKey(
                name: "FK_ChuHos_CuDans_ID_CuDan",
                table: "ChuHos",
                column: "ID_CuDan",
                principalTable: "CuDans",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_CuDans_NguoiDungs_ID_NguoiDung",
                table: "CuDans",
                column: "ID_NguoiDung",
                principalTable: "NguoiDungs",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CanHos_ChungCus_ID_ChungCu",
                table: "CanHos");

            migrationBuilder.DropForeignKey(
                name: "FK_CuDans_ChungCus_ID_ChungCu",
                table: "CuDans");

            migrationBuilder.DropForeignKey(
                name: "FK_CuDans_CanHos_ID_CanHo",
                table: "CuDans");

            migrationBuilder.DropForeignKey(
                name: "FK_NguoiDungs_CuDans_CuDanID",
                table: "NguoiDungs");

            migrationBuilder.DropTable(
                name: "ChuHos");

            migrationBuilder.DropTable(
                name: "HinhAnhCanHos");

            migrationBuilder.DropTable(
                name: "HinhAnhChungCus");

            migrationBuilder.DropTable(
                name: "HinhAnhDichVus");

            migrationBuilder.DropTable(
                name: "HoaDonDichVu_DichVus");

            migrationBuilder.DropTable(
                name: "PhanAnhs");

            migrationBuilder.DropTable(
                name: "DichVus");

            migrationBuilder.DropTable(
                name: "HoaDonDichVus");

            migrationBuilder.DropTable(
                name: "TinTucs");

            migrationBuilder.DropTable(
                name: "ChungCus");

            migrationBuilder.DropTable(
                name: "CanHos");

            migrationBuilder.DropTable(
                name: "CuDans");

            migrationBuilder.DropTable(
                name: "NguoiDungs");
        }
    }
}
