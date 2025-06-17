using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLCCCC.Models
{
    public class ChuHo
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int ID_CuDan { get; set; }

        [Required]
        public int ID_CanHo { get; set; }

        [Required]
        public int ID_ChungCu { get; set; }

        [ForeignKey("ID_CuDan")]
        public CuDan? CuDan { get; set; }

        [ForeignKey("ID_CanHo")]
        public CanHo? CanHo { get; set; }

        [ForeignKey("ID_ChungCu")]
        public ChungCu? ChungCu { get; set; }

        public DateTime NgayBatDau { get; set; } = DateTime.Now;

        public string? GhiChu { get; set; }
    }
}
