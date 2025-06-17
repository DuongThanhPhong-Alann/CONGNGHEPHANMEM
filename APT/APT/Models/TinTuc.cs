using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QLCCCC.Models
{
    public class TinTuc
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(200)]
        public string TieuDe { get; set; } = null!;

        [Required]
        public string NoiDung { get; set; } = null!;

        public DateTime NgayDang { get; set; } = DateTime.Now;

        public DateTime? NgaySuKien { get; set; }

        public string? HinhAnh { get; set; }

        public ICollection<PhanAnh>? DanhSachPhanAnh { get; set; }
    }
}