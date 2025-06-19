using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLCCCC.Models
{
    public class CuDan
    {
        [Key]
        public int ID { get; set; }

        public int ID_NguoiDung { get; set; }
        public int ID_CanHo { get; set; }
        public int? ID_ChungCu { get; set; }

        [ForeignKey("ID_NguoiDung")]
        public NguoiDung? NguoiDung { get; set; }

        [ForeignKey("ID_CanHo")]
        public CanHo? CanHo { get; set; }

        [ForeignKey("ID_ChungCu")]
        public ChungCu? ChungCu { get; set; }
    }
}