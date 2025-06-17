using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLCCCC.Data;
using QLCCCC.Models;

namespace QLCCCC.Controllers
{
    public class ChuHoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChuHoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ChuHo
        public async Task<IActionResult> Index()
        {
            var chuHos = await _context.ChuHos
                .Include(ch => ch.CuDan).ThenInclude(cd => cd.NguoiDung)
                .Include(ch => ch.CanHo)
                .Include(ch => ch.ChungCu)
                .ToListAsync();

            return View(chuHos);
        }
        // GET: ChuHo/GetThongTinCuDan
        [HttpGet]
        public async Task<IActionResult> GetThongTinCuDan(int idCuDan)
        {
            var thongTin = await _context.CuDans
                .Where(c => c.ID == idCuDan)
                .Select(c => new
                {
                    id_CanHo = c.ID_CanHo,
                    id_ChungCu = c.ID_ChungCu
                })
                .FirstOrDefaultAsync();

            if (thongTin == null)
                return NotFound();

            return Json(thongTin);
        }

        // GET: ChuHo/Create
        public IActionResult Create()
        {
            ViewBag.CuDans = _context.CuDans.ToList();
            return View();
        }

        // POST: ChuHo/Create
        // POST: ChuHo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ChuHo chuHo)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra: Căn hộ đã có chủ hộ hay chưa
                var daTonTai = await _context.ChuHos.AnyAsync(c => c.ID_CanHo == chuHo.ID_CanHo);

                if (daTonTai)
                {
                    ModelState.AddModelError(string.Empty, "Căn hộ này đã có Chủ hộ. Không thể thêm mới.");
                    ViewBag.CuDans = _context.CuDans.ToList();
                    return View(chuHo);
                }

                _context.Add(chuHo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CuDans = _context.CuDans.ToList();
            return View(chuHo);
        }


        // GET: ChuHo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var chuHo = await _context.ChuHos.FindAsync(id);
            if (chuHo == null) return NotFound();

            ViewBag.CuDans = _context.CuDans.ToList();
            return View(chuHo);
        }

        // POST: ChuHo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ChuHo chuHo)
        {
            if (id != chuHo.ID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chuHo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChuHoExists(chuHo.ID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CuDans = _context.CuDans.ToList();
            return View(chuHo);
        }

        // GET: ChuHo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var chuHo = await _context.ChuHos
                .Include(ch => ch.CuDan).ThenInclude(cd => cd.NguoiDung)
                .Include(ch => ch.CanHo)
                .Include(ch => ch.ChungCu)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (chuHo == null) return NotFound();

            return View(chuHo);
        }

        // POST: ChuHo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chuHo = await _context.ChuHos.FindAsync(id);
            if (chuHo != null)
            {
                _context.ChuHos.Remove(chuHo);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ChuHoExists(int id)
        {
            return _context.ChuHos.Any(e => e.ID == id);
        }
    }
}
