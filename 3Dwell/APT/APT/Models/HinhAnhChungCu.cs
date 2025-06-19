namespace QLCCCC.Models
{
    public class HinhAnhChungCu
    {
        public int ID { get; set; }
        public int ID_ChungCu { get; set; }
        public required string DuongDan { get; set; }
        public ChungCu? ChungCu { get; set; }
    }
}