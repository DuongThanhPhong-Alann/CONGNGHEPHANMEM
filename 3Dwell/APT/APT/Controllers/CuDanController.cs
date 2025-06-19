using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QLCCCC.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QLCCCC.Data;

namespace QLCCCC.Controllers
{
    public class CuDanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CuDanController(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // ✅ Hiển thị danh sách cư dân
        public IActionResult Index()
        {
            var danhSachCuDan = _context.CuDans
                .Include(cd => cd.NguoiDung)
                .Include(cd => cd.CanHo)
                .ThenInclude(ch => ch.ChungCu)
                .ToList();

            return View(danhSachCuDan);
        }

        // ✅ Hiển thị form tạo mới cư dân
        public IActionResult Create()
        {
            ViewBag.ChungCuList = _context.ChungCus
                .Select(c => new SelectListItem { Value = c.ID.ToString(), Text = c.Ten })
                .ToList();

            ViewBag.NguoiDungList = _context.NguoiDungs
             .Select(nd => new SelectListItem { Value = nd.ID.ToString(), Text = nd.ID.ToString() })
             .ToList();

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetCanHoByChungCu(int chungCuId)
        {
            var canHos = await _context.CanHos
                .Where(c => c.ID_ChungCu == chungCuId)
                .Select(c => new { c.ID, c.MaCan })
                .ToListAsync();
            return Json(canHos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CuDan cuDan)
        {
            if (!ModelState.IsValid)
            {
                // Load lại dropdown nếu có lỗi
                ViewBag.ChungCuList = _context.ChungCus
                    .Select(c => new SelectListItem { Value = c.ID.ToString(), Text = c.Ten })
                    .ToList();
                ViewBag.NguoiDungList = _context.NguoiDungs
    .Where(nd => nd.LoaiNguoiDung == "Cư dân")
    .Select(nd => new SelectListItem { Value = nd.ID.ToString(), Text = nd.ID.ToString() })
    .ToList();

                return View(cuDan);
            }

            // Thêm mới cư dân
            _context.CuDans.Add(cuDan);
            await _context.SaveChangesAsync(); // Lưu trước để có ID của CuDan

            
            // Cập nhật lại NguoiDung: gán CuDan và nâng từ Khách => Cư dân nếu cần
            var nguoiDung = await _context.NguoiDungs.FindAsync(cuDan.ID_NguoiDung);
            if (nguoiDung != null)
            {
                nguoiDung.CuDan = cuDan;

                if (nguoiDung.LoaiNguoiDung == "Khách")
                {
                    nguoiDung.LoaiNguoiDung = "Cư dân";
                }

                _context.Update(nguoiDung);
                await _context.SaveChangesAsync();
            }


            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var cuDan = await _context.CuDans
                .Include(c => c.CanHo)
                .ThenInclude(ch => ch.ChungCu)
                .FirstOrDefaultAsync(c => c.ID == id);

            if (cuDan == null) return NotFound();

            ViewBag.NguoiDungs = new SelectList(_context.NguoiDungs, "ID", "HoTen", cuDan.ID_NguoiDung);
            ViewBag.ChungCus = new SelectList(_context.ChungCus, "ID", "Ten", cuDan.CanHo?.ID_ChungCu ?? 0);
            ViewBag.CanHos = new SelectList(_context.CanHos.Where(ch => ch.ID_ChungCu == cuDan.CanHo.ID_ChungCu), "ID", "MaCan", cuDan.ID_CanHo);

            return View(cuDan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CuDan cuDan)
        {
            if (id != cuDan.ID) return NotFound();

            if (!ModelState.IsValid) return View(cuDan);

            _context.Update(cuDan);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var cuDan = await _context.CuDans
                .Include(c => c.NguoiDung)
                .Include(c => c.CanHo)
                .ThenInclude(ch => ch.ChungCu)
                .FirstOrDefaultAsync(c => c.ID == id);

            if (cuDan == null) return NotFound();

            return View(cuDan);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var cuDan = await _context.CuDans
                .Include(c => c.NguoiDung)
                .Include(c => c.CanHo)
                .ThenInclude(ch => ch.ChungCu)
                .FirstOrDefaultAsync(c => c.ID == id);

            if (cuDan == null) return NotFound();

            return View(cuDan);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cuDan = await _context.CuDans
                .Include(cd => cd.NguoiDung)
                .FirstOrDefaultAsync(cd => cd.ID == id);

            if (cuDan == null) return NotFound();

            // ✅ Kiểm tra xem cư dân này có là chủ hộ không
            var isChuHo = await _context.ChuHos.AnyAsync(ch => ch.ID_CuDan == id);
            if (isChuHo)
            {
                TempData["ErrorMessage"] = "Cư dân hiện tại đang là chủ hộ, bạn không thể thực hiện hành động xóa.";
                return RedirectToAction(nameof(Index));
            }

            // Nếu là cư dân thì hạ xuống thành khách
            if (cuDan.NguoiDung != null && cuDan.NguoiDung.LoaiNguoiDung == "Cư dân")
            {
                cuDan.NguoiDung.LoaiNguoiDung = "Khách";
                cuDan.NguoiDung.CuDan = null;
                _context.Update(cuDan.NguoiDung);
            }

            _context.CuDans.Remove(cuDan);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetNguoiDungById(int nguoiDungId)
        {
            var nguoiDung = await _context.NguoiDungs
                .Where(nd => nd.ID == nguoiDungId)
                .Select(nd => new { nd.HoTen })
                .FirstOrDefaultAsync();

            if (nguoiDung == null)
            {
                return NotFound();
            }

            return Json(nguoiDung);
        }
    }
}
