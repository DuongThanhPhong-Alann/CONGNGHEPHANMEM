using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using QLCCCC.Models;
using QLCCCC.Repositories.Interfaces;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using QLCCCC.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace QLCCCC.Controllers
{
    public class CanHoController : Controller
    {
        private readonly ICanHoRepository _canHoRepository;
        private readonly IChungCuRepository _chungCuRepository;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ApplicationDbContext _context;

        public CanHoController(ICanHoRepository canHoRepository, IChungCuRepository chungCuRepository,
            IWebHostEnvironment hostingEnvironment, ApplicationDbContext context)
        {
            _canHoRepository = canHoRepository;
            _chungCuRepository = chungCuRepository;
            _hostingEnvironment = hostingEnvironment;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var canHos = await _canHoRepository.GetAllWithDetailsAsync(); // Includes HinhAnhCanHos
            return View(canHos);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.ChungCus = _context.ChungCus.ToList();
            return View(new CanHo());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CanHo model, List<string> URLs, IFormFileCollection HinhAnh)
        {
            if (ModelState.IsValid)
            {
                // Handle image uploads
                if (HinhAnh != null && HinhAnh.Count > 0)
                {
                    model.HinhAnhCanHos = new List<HinhAnhCanHo>();
                    foreach (var file in HinhAnh)
                    {
                        if (file.Length > 0 && file.Length <= 5 * 1024 * 1024 && file.ContentType.StartsWith("image/"))
                        {
                            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            model.HinhAnhCanHos.Add(new HinhAnhCanHo { DuongDan = $"/images/{fileName}" });
                        }
                        else
                        {
                            ModelState.AddModelError("HinhAnh", $"File {file.FileName} is invalid or too large.");
                        }
                    }
                }

                // Assign URLs
                model.URLs = URLs != null && URLs.Any() ? URLs : new List<string>();

                _context.CanHos.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ChungCus = _context.ChungCus.ToList();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var canHo = await _canHoRepository.GetByIdWithDetailsAsync(id.Value);
            if (canHo == null) return NotFound();

            ViewBag.ChungCus = await _chungCuRepository.GetAllAsync();
            return View(canHo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CanHo model, List<string> URLs, IFormFileCollection HinhAnh)
        {
            if (id != model.ID) return NotFound();

            if (ModelState.IsValid)
            {
                var existingCanHo = await _canHoRepository.GetByIdWithDetailsAsync(id);
                if (existingCanHo == null) return NotFound();

                // Update basic properties
                existingCanHo.MaCan = model.MaCan;
                existingCanHo.ID_ChungCu = model.ID_ChungCu;
                existingCanHo.DienTich = model.DienTich;
                existingCanHo.SoPhong = model.SoPhong;
                existingCanHo.Gia = model.Gia;
                existingCanHo.TrangThai = model.TrangThai;
                existingCanHo.MoTa = model.MoTa;
                existingCanHo.URLs = URLs != null && URLs.Any() ? URLs : new List<string>();

                // Handle new images
                if (HinhAnh != null && HinhAnh.Count > 0)
                {
                    // Delete old images
                    await _canHoRepository.DeleteImagesAsync(existingCanHo.HinhAnhCanHos);
                    existingCanHo.HinhAnhCanHos = new List<HinhAnhCanHo>();

                    foreach (var file in HinhAnh)
                    {
                        if (file.Length > 0 && file.Length <= 5 * 1024 * 1024 && file.ContentType.StartsWith("image/"))
                        {
                            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            existingCanHo.HinhAnhCanHos.Add(new HinhAnhCanHo
                            {
                                DuongDan = $"/images/{fileName}",
                                ID_CanHo = existingCanHo.ID // Explicitly set foreign key
                            });
                        }
                        else
                        {
                            ModelState.AddModelError("HinhAnh", $"File {file.FileName} is invalid or too large.");
                        }
                    }
                }

                await _canHoRepository.UpdateAsync(existingCanHo);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ChungCus = await _chungCuRepository.GetAllAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var canHo = await _canHoRepository.GetByIdWithDetailsAsync(id.Value);
            if (canHo == null) return NotFound();

            return View(canHo);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var canHo = await _canHoRepository.GetByIdAsync(id.Value);
            if (canHo == null) return NotFound();

            return View(canHo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var canHo = await _canHoRepository.GetByIdAsync(id);
            if (canHo == null) return NotFound();

            // Delete images before deleting CanHo
            await _canHoRepository.DeleteImagesAsync(canHo.HinhAnhCanHos);

            await _canHoRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}