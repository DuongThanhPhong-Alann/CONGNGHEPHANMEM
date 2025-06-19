using System.ComponentModel.DataAnnotations;

namespace QLCCCC.Models
{
    public class NguoiDung
    {
        public int ID { get; set; }

        [Required]
        public string HoTen { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]

        public string MatKhau { get; set; } = string.Empty; // Cần mã hóa khi lưu

        [Required]
        public string SoDienThoai { get; set; } = string.Empty;


        public string? LoaiNguoiDung { get; set; }

        public CuDan? CuDan { get; set; }

        
    }
}