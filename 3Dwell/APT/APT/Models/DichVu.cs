using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLCCCC.Models
{
    public class DichVu
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(100)]
        public string TenDichVu { get; set; } = string.Empty;

        public string? MoTa { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Gia { get; set; }

        public List<HoaDonDichVu_DichVu> HoaDonDichVu_DichVus { get; set; } = new List<HoaDonDichVu_DichVu>();
        public List<HinhAnhDichVu> HinhAnhDichVus { get; set; } = new List<HinhAnhDichVu>();
    }
}