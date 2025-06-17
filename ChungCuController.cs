using Microsoft.AspNetCore.Mvc;
using QLCCCC.Models;
using QLCCCC.Repositories.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace QLCCCC.Controllers
{
    public class ChungCuController : Controller
    {
        private readonly IChungCuRepository _chungCuRepository;

        public ChungCuController(IChungCuRepository chungCuRepository)
        {
            _chungCuRepository = chungCuRepository;
        }

        // GET: ChungCu
        public async Task<IActionResult> Index()
        {
            var chungCus = await _chungCuRepository.GetAllAsync();
            return View(chungCus);
        }

        // GET: ChungCu/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var chungCu = await _chungCuRepository.GetByIdWithDetailsAsync(id.Value);
            if (chungCu == null) return NotFound();
            return View(chungCu);
        }

        // GET: ChungCu/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: ChungCu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ChungCu chungCu, IFormFile HinhAnh)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra và lưu hình ảnh
                if (HinhAnh != null && HinhAnh.Length > 0)
                {
                    var fileName = Path.GetFileName(HinhAnh.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                    // Lưu file vào thư mục wwwroot/images
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await HinhAnh.CopyToAsync(stream);
                    }

                    // Cập nhật thông tin hình ảnh vào đối tượng ChungCu
                    chungCu.HinhAnhChungCus = new List<HinhAnhChungCu>
                    {
                        new HinhAnhChungCu { DuongDan = "/images/" + fileName }
                    };
                }

                // Lưu đối tượng ChungCu vào cơ sở dữ liệu
                await _chungCuRepository.AddAsync(chungCu);
                return RedirectToAction(nameof(Index));
            }
            return View(chungCu);
        }

        // GET: ChungCu/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var chungCu = await _chungCuRepository.GetByIdAsync(id.Value);
            if (chungCu == null) return NotFound();
            return View(chungCu);
        }

        // POST: ChungCu/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ChungCu chungCu, IFormFile? HinhAnh)
        {
            if (id != chungCu.ID) return NotFound();

            if (ModelState.IsValid)
            {
                // Kiểm tra và lưu hình ảnh mới nếu có
                if (HinhAnh != null && HinhAnh.Length > 0)
                {
                    var fileName = Path.GetFileName(HinhAnh.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await HinhAnh.CopyToAsync(stream);
                    }

                    chungCu.HinhAnhChungCus = new List<HinhAnhChungCu>
                    {
                        new HinhAnhChungCu { DuongDan = "/images/" + fileName }
                    };
                }

                await _chungCuRepository.UpdateAsync(chungCu);
                return RedirectToAction(nameof(Index));
            }
            return View(chungCu);
        }

        // GET: ChungCu/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var chungCu = await _chungCuRepository.GetByIdAsync(id.Value);
            if (chungCu == null) return NotFound();
            return View(chungCu);
        }

        // POST: ChungCu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _chungCuRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
