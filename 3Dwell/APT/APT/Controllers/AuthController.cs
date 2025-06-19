using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.Tasks;
using QLCCCC.Models;
using QLCCCC.Repositories.Interfaces;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using QLCCCC.Data;
using QLCCCC.ViewModels;

public class AuthController : Controller
{
    private readonly INguoiDungRepository _repository;
    private readonly ApplicationDbContext _context; // Khai báo biến _context
    private readonly IEmailService _emailService;

    public AuthController(IEmailService emailService, INguoiDungRepository repository, ApplicationDbContext context)
    {
        _emailService = emailService;
        _repository = repository;
        _context = context;
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(NguoiDung model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (string.IsNullOrWhiteSpace(model.Email))
        {
            ModelState.AddModelError("Email", "Email không được để trống!");
            return View(model);
        }

        var existingUser = await _repository.GetByEmailAsync(model.Email);
        if (existingUser != null)
        {
            ModelState.AddModelError("Email", "Email đã tồn tại!");
            return View(model);
        }

        if (string.IsNullOrWhiteSpace(model.MatKhau))
        {
            ModelState.AddModelError("MatKhau", "Mật khẩu không được để trống!");
            return View(model);
        }

        model.LoaiNguoiDung = "Khách"; // Gán quyền mặc định là khách
        await _repository.AddAsync(model);

        return RedirectToAction("Login");
    }

    [HttpPost]
    public async Task<IActionResult> SetAdminRole(int userId)
    {
        var user = await _repository.GetByIdAsync(userId);
        if (user == null)
        {
            return NotFound("Người dùng không tồn tại.");
        }

        user.LoaiNguoiDung = "Ban quản lý";
        await _repository.UpdateAsync(user);

        return Ok("Đã gán quyền Ban quản lý thành công.");
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var user = await _repository.GetByEmailAsync(email);
        if (user == null) // Debug if user isn’t found
        {
            System.Diagnostics.Debug.WriteLine($"No user found for email: {email}");
            ModelState.AddModelError("", "Email không tồn tại");
            return View();
        }
        if (user.MatKhau != password)
        {
            System.Diagnostics.Debug.WriteLine("Password mismatch");
            ModelState.AddModelError("", "Mật khẩu không đúng");
            return View();
        }

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.HoTen ?? "Người dùng"),
        new Claim("UserId", user.ID.ToString()),
        new Claim(ClaimTypes.Role, user.LoaiNguoiDung ?? "Khách"),
        new Claim(ClaimTypes.Email, user.Email)
    };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties { IsPersistent = true };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    public async Task<IActionResult> UserProfile()
    {
        string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        if (email == null)
        {
            return NotFound("Không tìm thấy thông tin tài khoản.");
        }

        var user = await _repository.GetByEmailAsync(email);
        if (user == null)
        {
            return NotFound("Người dùng không tồn tại.");
        }

        var viewModel = new UserProfileViewModel
        {
            NguoiDung = user
        };

        if (user.LoaiNguoiDung == "Cư dân")
        {
            var cuDan = await _context.CuDans
                .Include(c => c.CanHo)
                .Include(c => c.ChungCu)
                .FirstOrDefaultAsync(c => c.ID_NguoiDung == user.ID);

            if (cuDan != null)
            {
                viewModel.CuDan = cuDan;  // Gán thông tin Cư Dân cho ViewModel
            }
            else
            {
                // Log hoặc thông báo không tìm thấy thông tin Cư Dân
                ModelState.AddModelError("", "Không tìm thấy thông tin căn hộ hoặc chung cư.");
            }
        }


        return View(viewModel); // Truyền ViewModel vào view
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }



    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    [HttpPost]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        var user = await _repository.GetByEmailAsync(email);
        if (user == null)
        {
            ModelState.AddModelError("", "Email không tồn tại.");
            return View();
        }

        var newPassword = GenerateRandomPassword();
        user.MatKhau = newPassword;
        await _repository.UpdateAsync(user);

        var emailSent = await SendNewPasswordEmail(user.Email, newPassword);

        if (emailSent)
        {
            // Truyền qua TempData để không cần model
            TempData["ResetEmail"] = user.Email;
            TempData["ResetPassword"] = newPassword;
            return RedirectToAction("ChangePasswordAfterReset");
        }

        ModelState.AddModelError("", "Không thể gửi email.");
        return View();
    }


    private string GenerateRandomPassword()
    {
        var length = 8;
        var random = new Random();
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        return new string(Enumerable.Range(0, length).Select(_ => chars[random.Next(chars.Length)]).ToArray());
    }

    private async Task<bool> SendNewPasswordEmail(string email, string newPassword)
    {
        try
        {
            string subject = "Mật khẩu mới của bạn";
            string message = $"<p>Mật khẩu mới của bạn là: <b>{newPassword}</b></p><p>Vui lòng đăng nhập và thay đổi mật khẩu ngay sau đó.</p>";

            await _emailService.SendEmailAsync(email, subject, message);

            return true;
        }
        catch (Exception ex)
        {
            // Ghi log nếu cần
            Console.WriteLine($"Gửi email thất bại: {ex.Message}");
            return false;
        }
    }

    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(string newPassword, string confirmPassword)
    {
        var userId = int.Parse(User.FindFirst("UserId").Value);
        var user = await _repository.GetByIdAsync(userId);
        if (user == null)
        {
            return NotFound("Người dùng không tồn tại.");
        }

        if (newPassword != confirmPassword)
        {
            ModelState.AddModelError("", "Mật khẩu không khớp.");
            return View();
        }

        user.MatKhau = newPassword;
        await _repository.UpdateAsync(user);
        return RedirectToAction("UserProfile");
    }
    [HttpGet]
    public IActionResult ChangePasswordAfterReset()
    {
        ViewBag.Email = TempData["ResetEmail"];
        ViewBag.TempPassword = TempData["ResetPassword"];
        TempData.Keep(); // Giữ lại TempData cho POST nếu cần
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePasswordAfterReset(string email, string tempPassword, string newPassword, string confirmPassword)
    {
        var user = await _repository.GetByEmailAsync(email);
        if (user == null)
        {
            ModelState.AddModelError("", "Người dùng không tồn tại.");
            return View();
        }

        if (user.MatKhau != tempPassword)
        {
            ModelState.AddModelError("", "Mật khẩu tạm thời không đúng.");
            return View();
        }

        if (newPassword != confirmPassword)
        {
            ModelState.AddModelError("", "Mật khẩu mới không khớp.");
            return View();
        }

        user.MatKhau = newPassword;
        await _repository.UpdateAsync(user);

        // Gửi email cảnh báo khi thay đổi mật khẩu
        var emailSent = await SendPasswordChangeAlertEmail(user.Email);

        if (!emailSent)
        {
            ModelState.AddModelError("", "Không thể gửi email cảnh báo.");
            return View();
        }

        return RedirectToAction("Login");
    }

    // Phương thức gửi email cảnh báo
    private async Task<bool> SendPasswordChangeAlertEmail(string email)
    {
        try
        {
            string subject = "Cảnh báo thay đổi mật khẩu";
            string message = $"<p>Chúng tôi muốn thông báo rằng mật khẩu của bạn đã được thay đổi.</p>" +
                             "<p>Nếu bạn không thực hiện thay đổi này, vui lòng kiểm tra tài khoản của bạn ngay lập tức và liên hệ với chúng tôi.</p>";

            await _emailService.SendEmailAsync(email, subject, message);

            return true;
        }
        catch (Exception ex)
        {
            // Ghi log nếu cần
            Console.WriteLine($"Gửi email cảnh báo thất bại: {ex.Message}");
            return false;
        }
    }


}