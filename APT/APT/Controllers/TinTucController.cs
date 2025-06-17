using Microsoft.AspNetCore.Mvc;
using QLCCCC.Models;
using QLCCCC.Repositories;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace QLCCCC.Controllers
{
    public class TinTucController : Controller
    {
        private readonly ITinTucRepository _repository;
        private readonly IWebHostEnvironment _environment;

        public TinTucController(ITinTucRepository repository, IWebHostEnvironment environment)
        {
            _repository = repository;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            var tinTucs = await _repository.GetAllAsync();
            return View(tinTucs);
        }

        public async Task<IActionResult> Details(int id)
        {
            var tinTuc = await _repository.GetByIdAsync(id);
            if (tinTuc == null) return NotFound();
            return View(tinTuc);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TinTuc tinTuc, IFormFile hinhAnh)
        {
            if (!ModelState.IsValid)
            {
                return View(tinTuc);
            }

            // Xử lý tải lên hình ảnh nếu có
            if (hinhAnh != null && hinhAnh.Length > 0)
            {
                var fileName = Path.GetFileNameWithoutExtension(hinhAnh.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(hinhAnh.FileName);
                var filePath = Path.Combine(_environment.WebRootPath, "images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await hinhAnh.CopyToAsync(stream);
                }

                tinTuc.HinhAnh = "/images/" + fileName; // Gán đường dẫn vào thuộc tính HinhAnh
            }

            await _repository.AddAsync(tinTuc);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var tinTuc = await _repository.GetByIdAsync(id);
            if (tinTuc == null) return NotFound();
            return View(tinTuc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TinTuc tinTuc, IFormFile? hinhAnh)
        {
            if (!ModelState.IsValid)
            {
                return View(tinTuc);
            }

            // Xử lý tải lên hình ảnh mới nếu có
            if (hinhAnh != null && hinhAnh.Length > 0)
            {
                var fileName = Path.GetFileNameWithoutExtension(hinhAnh.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(hinhAnh.FileName);
                var filePath = Path.Combine(_environment.WebRootPath, "images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await hinhAnh.CopyToAsync(stream);
                }

                // Xóa hình ảnh cũ nếu có
                if (!string.IsNullOrEmpty(tinTuc.HinhAnh))
                {
                    var oldFilePath = Path.Combine(_environment.WebRootPath, tinTuc.HinhAnh.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                tinTuc.HinhAnh = "/images/" + fileName;
            }

            await _repository.UpdateAsync(tinTuc);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var tinTuc = await _repository.GetByIdAsync(id);
            if (tinTuc == null) return NotFound();
            return View(tinTuc);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tinTuc = await _repository.GetByIdAsync(id);
            if (tinTuc != null)
            {
                // Xóa hình ảnh nếu có
                if (!string.IsNullOrEmpty(tinTuc.HinhAnh))
                {
                    var filePath = Path.Combine(_environment.WebRootPath, tinTuc.HinhAnh.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                await _repository.DeleteAsync(id);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}