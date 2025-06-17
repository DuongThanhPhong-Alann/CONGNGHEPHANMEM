using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLCCCC.Models
{
    public class HinhAnhCanHo
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string DuongDan { get; set; } = string.Empty;

        public int ID_CanHo { get; set; }

        [ForeignKey("ID_CanHo")]
        public CanHo CanHo { get; set; } = null!;
    }
}