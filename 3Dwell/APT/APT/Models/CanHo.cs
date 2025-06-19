using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json; // Thêm namespace này để serialize/deserialize JSON

namespace QLCCCC.Models
{
    public class CanHo
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string MaCan { get; set; } = string.Empty;

        [Required]
        public int ID_ChungCu { get; set; }

        [ForeignKey("ID_ChungCu")]
        public ChungCu? ChungCu { get; set; }

        public float DienTich { get; set; }
        public int SoPhong { get; set; }
        public decimal Gia { get; set; }

        [Required]
        [StringLength(20)]
        public string TrangThai { get; set; } = "Đang bán";

        public string? MoTa { get; set; }

        // Thuộc tính công khai là List<string>
        public List<string> URLs { get; set; } = new List<string>();

        // Thuộc tính private để lưu trữ JSON trong database
        [Column("URLs")] // Đảm bảo ánh xạ đến cột URLs trong bảng
        private string URLsJson
        {
            get => JsonSerializer.Serialize(URLs);
            set => URLs = string.IsNullOrEmpty(value) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(value);
        }

        public ICollection<HinhAnhCanHo> HinhAnhCanHos { get; set; } = new List<HinhAnhCanHo>();
    }
}