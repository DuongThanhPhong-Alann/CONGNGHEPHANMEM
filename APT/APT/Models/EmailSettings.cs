namespace QLCCCC.Models
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUser { get; set; } // Tên đăng nhập
        public string SmtpPass { get; set; } // Mật khẩu
        public string FromAddress { get; set; } // Địa chỉ người gửi
        public string FromName { get; set; } // Tên người gửi
        public string ToAddress { get; set; } // (Nếu b
    }
}
