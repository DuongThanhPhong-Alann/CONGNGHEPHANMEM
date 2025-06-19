using System.ComponentModel.DataAnnotations.Schema;

namespace QLCCCC.Models
{
    public class HoaDonDichVu_DichVu
    {
        public int ID_HoaDon { get; set; }
        public int ID_DichVu { get; set; }

        [ForeignKey("ID_HoaDon")]
        public HoaDonDichVu HoaDonDichVu { get; set; } = null!;

        [ForeignKey("ID_DichVu")]
        public DichVu DichVu { get; set; } = null!;
    }
}