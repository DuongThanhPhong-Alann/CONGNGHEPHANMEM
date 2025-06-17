using Microsoft.AspNetCore.Mvc;

namespace QLCCCC.Controllers
{
    public class ChatBox : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
