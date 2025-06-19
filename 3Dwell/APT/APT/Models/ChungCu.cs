namespace QLCCCC.Models
{
    public class ChungCu
    {
        public int ID { get; set; }
        public string Ten { get; set; } = string.Empty;
        public string DiaChi { get; set; } = string.Empty;
        public string? ChuDauTu { get; set; }
        public int? NamXayDung { get; set; }
        public int? SoTang { get; set; }
        public string? MoTa { get; set; }
        public List<CanHo> CanHos { get; set; } = new List<CanHo>();
        public List<HinhAnhChungCu> HinhAnhChungCus { get; set; } = new List<HinhAnhChungCu>();
    }
}