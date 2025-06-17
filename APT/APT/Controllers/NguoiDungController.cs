using Microsoft.AspNetCore.Mvc;
using QLCCCC.Models;
using QLCCCC.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using QLCCCC.Data;
using QLCCCC.Services;

public class NguoiDungController : Controller
{
    private readonly INguoiDungRepository _repository;
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;
    public NguoiDungController(INguoiDungRepository repository, ApplicationDbContext context, IEmailService emailService)
    {
        _repository = repository;
        _context = context;
        _emailService = emailService;
    }

    // 🟢 Hiển thị danh sách người dùng
    public async Task<IActionResult> Index()
    {
        var nguoiDungs = await _repository.GetAllAsync();
        return View(nguoiDungs);
    }

    // 🟢 Hiển thị form tạo người dùng mới
    public IActionResult Create()
    {
        ViewBag.LoaiNguoiDungList = new List<SelectListItem>
        {
            new SelectListItem { Value = "Cư dân", Text = "Cư dân" },
            new SelectListItem { Value = "Ban quản lý", Text = "Ban quản lý" },
            new SelectListItem { Value = "Khách", Text = "Khách" }
        };
        return View();
    }

    // 🟢 Xử lý form tạo người dùng mới
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NguoiDung nguoiDung)
    {
        if (ModelState.IsValid)
        {
            await _repository.AddAsync(nguoiDung);
            return RedirectToAction(nameof(Index));
        }

        ViewBag.LoaiNguoiDungList = new List<SelectListItem>
        {
            new SelectListItem { Value = "Cư dân", Text = "Cư dân" },
            new SelectListItem { Value = "Ban quản lý", Text = "Ban quản lý" },
            new SelectListItem { Value = "Khách", Text = "Khách" }
        };
        return View(nguoiDung);
    }

    // 🟢 Hiển thị form chỉnh sửa người dùng
    public async Task<IActionResult> Edit(int id)
    {
        var nguoiDung = await _repository.GetByIdAsync(id);
        if (nguoiDung == null) return NotFound();

        ViewBag.LoaiNguoiDungList = new List<SelectListItem>
        {
            new SelectListItem { Value = "Cư dân", Text = "Cư dân" },
            new SelectListItem { Value = "Ban quản lý", Text = "Ban quản lý" },
            new SelectListItem { Value = "Khách", Text = "Khách" }
        };

        ViewBag.IsBanQuanLy = nguoiDung.LoaiNguoiDung == "Ban quản lý";
        return View(nguoiDung);
    }

    // 🟢 Xử lý form chỉnh sửa người dùng
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, NguoiDung nguoiDung)
    {
        if (id != nguoiDung.ID) return NotFound();

        // Lấy thực thể đã được theo dõi từ cơ sở dữ liệu
        var oldNguoiDung = await _repository.GetByIdAsync(id);
        if (oldNguoiDung == null) return NotFound();

        if (ModelState.IsValid)
        {
            // Kiểm tra logic "Ban quản lý"
            if (oldNguoiDung.LoaiNguoiDung == "Ban quản lý" &&
                (nguoiDung.LoaiNguoiDung == "Cư dân" || nguoiDung.LoaiNguoiDung == "Khách"))
            {
                ModelState.AddModelError("LoaiNguoiDung", "Không thể thay đổi tài khoản Ban quản lý sang Cư dân hoặc Khách.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.LoaiNguoiDungList = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Cư dân", Text = "Cư dân" },
                    new SelectListItem { Value = "Ban quản lý", Text = "Ban quản lý" },
                    new SelectListItem { Value = "Khách", Text = "Khách" }
                };
                ViewBag.IsBanQuanLy = oldNguoiDung.LoaiNguoiDung == "Ban quản lý";
                return View(nguoiDung);
            }

            // Cập nhật các thuộc tính của oldNguoiDung thay vì dùng nguoiDung trực tiếp
            oldNguoiDung.HoTen = nguoiDung.HoTen;
            oldNguoiDung.SoDienThoai = nguoiDung.SoDienThoai;
            oldNguoiDung.Email = nguoiDung.Email;
            oldNguoiDung.MatKhau = nguoiDung.MatKhau;
            oldNguoiDung.LoaiNguoiDung = nguoiDung.LoaiNguoiDung;

            await _repository.UpdateAsync(oldNguoiDung);
            return RedirectToAction(nameof(Index));
        }

        ViewBag.LoaiNguoiDungList = new List<SelectListItem>
        {
            new SelectListItem { Value = "Cư dân", Text = "Cư dân" },
            new SelectListItem { Value = "Ban quản lý", Text = "Ban quản lý" },
            new SelectListItem { Value = "Khách", Text = "Khách" }
        };
        ViewBag.IsBanQuanLy = oldNguoiDung.LoaiNguoiDung == "Ban quản lý";
        return View(nguoiDung);
    }

    // 🟢 Hiển thị form xóa người dùng
    public async Task<IActionResult> Delete(int id)
    {
        var nguoiDung = await _repository.GetByIdAsync(id);
        if (nguoiDung == null) return NotFound();
        return View(nguoiDung);
    }

    // 🟢 Xử lý xác nhận xóa người dùng
    [HttpPost, ActionName("DeleteConfirmed")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _repository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }


    [HttpGet]
    [Authorize]
    public async Task<IActionResult> ChangePassword()
    {
        var userId = int.Parse(User.FindFirst("UserId").Value);
        var user = await _context.NguoiDungs.FindAsync(userId);
        if (user == null) return NotFound();

        return View(user);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(string CurrentPassword, string NewPassword, string ConfirmPassword)
    {
        var userId = int.Parse(User.FindFirst("UserId").Value);
        var user = await _context.NguoiDungs.FindAsync(userId);
        if (user == null) return NotFound();

        if (user.MatKhau != CurrentPassword)
        {
            ViewBag.PasswordError = "Mật khẩu hiện tại không đúng.";
            return View(user);
        }

        if (NewPassword != ConfirmPassword)
        {
            ViewBag.ConfirmError = "Xác nhận mật khẩu không khớp.";
            return View(user);
        }

        user.MatKhau = NewPassword;
        _context.Update(user);
        await _context.SaveChangesAsync();

        // Gửi email cảnh báo khi thay đổi mật khẩu
        var emailSent = await SendPasswordChangeAlertEmail(user.Email);

        if (!emailSent)
        {
            ViewBag.EmailError = "Không thể gửi email cảnh báo.";
            return View(user);
        }

        ViewBag.SuccessMessage = "Đổi mật khẩu thành công!";
        return View(user);
    }

    // Phương thức gửi email cảnh báo
    private async Task<bool> SendPasswordChangeAlertEmail(string email)
    {
        try
        {
            string subject = "Cảnh báo thay đổi mật khẩu";
            string message = $"<p>Chúng tôi muốn thông báo rằng mật khẩu của bạn đã được thay đổi.</p>" +
                             "<p>Nếu bạn không thực hiện thay đổi này, vui lòng kiểm tra tài khoản của bạn ngay lập tức và liên hệ với chúng tôi.</p>";

            // Gửi email
            await _emailService.SendEmailAsync(email, subject, message);

            return true;
        }
        catch (Exception ex)
        {
            // Log lỗi nếu cần
            Console.WriteLine($"Gửi email cảnh báo thất bại: {ex.Message}");
            return false;
        }
    }


}