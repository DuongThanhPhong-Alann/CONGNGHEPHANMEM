using QLCCCC.Models;

namespace QLCCCC.ViewModels
{
    public class UserProfileViewModel
    {
        public NguoiDung NguoiDung { get; set; } = null!; // Thông tin người dùng
        public CuDan? CuDan { get; set; } // Thông tin Cư Dân, có thể null nếu không phải Cư Dân
    }
}