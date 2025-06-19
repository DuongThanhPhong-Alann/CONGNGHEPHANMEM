using Microsoft.AspNetCore.Mvc;
using QLCCCC.Repositories.Interfaces;

namespace QLCCCC.Controllers
{
    public class TestEmail : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SendTestEmail([FromServices] IEmailService emailService)
        {
            await emailService.SendEmailAsync("pudyslime@gmail.com", "Test Email", "<b>Thử gửi email thành công!</b>");
            return Content("Đã gửi email (nếu không có lỗi)");
        }
    }
}
