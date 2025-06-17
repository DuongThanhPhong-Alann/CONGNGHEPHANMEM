using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLCCCC.Models
{
    public enum TrangThaiPhanAnh
    {
        [Display(Name = "Chưa xử lý")]
        ChuaXuLy,
        [Display(Name = "Chờ xử lý")]
        ChoXuLy,
        [Display(Name = "Hoàn thành")]
        HoanThanh
    }

    public class PhanAnh
    {
        [Key]
        public int ID { get; set; }

        [ForeignKey("NguoiDung")]
        public int ID_NguoiDung { get; set; }

        [Required]
        public string NoiDung { get; set; } = string.Empty;

        [Required]
        public TrangThaiPhanAnh TrangThai { get; set; } = TrangThaiPhanAnh.ChuaXuLy;

        public DateTime NgayGui { get; set; } = DateTime.Now;

        public string? PhanHoi { get; set; }  // 🟢 Thêm phản hồi từ Ban quản lý
        public string? HinhAnh { get; set; }

        public virtual NguoiDung? NguoiDung { get; set; }

        
    }

}