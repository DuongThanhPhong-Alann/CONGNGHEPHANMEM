using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLCCCC.Models
{
    public class HoaDonDichVu
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int ID_CanHo { get; set; }

        [Required]
        public int ID_ChungCu { get; set; }

        [Required]
        public decimal SoTien { get; set; }

        [Required]
        public DateTime NgayLap { get; set; } = DateTime.Now;

        [Required]
        [StringLength(50)]
        public string TrangThai { get; set; } = "Chưa thanh toán";

        [ForeignKey("ID_CanHo")]
        public virtual CanHo? CanHo { get; set; }

        [ForeignKey("ID_ChungCu")]
        public virtual ChungCu? ChungCu { get; set; }

        public List<HoaDonDichVu_DichVu> HoaDonDichVu_DichVus { get; set; } = new();
    }
}