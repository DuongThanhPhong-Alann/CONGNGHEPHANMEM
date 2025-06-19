using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLCCCC.Models
{
    public class HinhAnhDichVu
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string DuongDan { get; set; } = string.Empty;

        [Required]
        [ForeignKey("DichVu")]
        public int ID_DichVu { get; set; }
        public DichVu? DichVu { get; set; }
    }
}