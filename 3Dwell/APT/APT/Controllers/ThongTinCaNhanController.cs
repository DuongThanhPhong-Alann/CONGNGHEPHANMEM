using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using QLCCCC.Data;
using QLCCCC.Models;
using System.Security.Claims;

namespace QLCCCC.Controllers
{
    [Authorize] // ✅ Bắt buộc đăng nhập mới được vào
    public class ThongTinCaNhanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ThongTinCaNhanController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // 🔹 Lấy email từ Claims của người dùng đăng nhập
            var email = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Auth"); // 🔴 Nếu không có email, chuyển về trang đăng nhập
            }

            // 🔹 Tìm thông tin cá nhân của người dùng trong database theo email
            var nguoiDung = _context.NguoiDungs.FirstOrDefault(nd => nd.Email == email);

            if (nguoiDung == null)
            {
                return NotFound("Không tìm thấy thông tin người dùng.");
            }

            return View(nguoiDung); // ✅ Trả về View chứa thông tin cá nhân
        }
    }
}
