using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLCCCC.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using System.Text;

namespace QLCCCC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataExportController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DataExportController>? _logger;

        public DataExportController(ApplicationDbContext context, ILogger<DataExportController>? logger = null)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> ExportData()
        {
            try
            {
                var data = new
                {
                    Users = await _context.NguoiDungs
                        .Select(u => new { u.ID, u.HoTen, u.Email, u.LoaiNguoiDung })
                        .ToListAsync(),
                    Invoices = await _context.HoaDonDichVus
                        .Include(h => h.CanHo)
                        .Select(h => new { h.ID, h.ID_CanHo, h.SoTien, h.TrangThai, CanHoMa = h.CanHo != null ? h.CanHo.MaCan : null })
                        .ToListAsync(),
                    Complaints = await _context.PhanAnhs
                        .Select(p => new
                        {
                            p.ID,
                            p.ID_NguoiDung,
                            p.NoiDung,
                            TrangThai = p.TrangThai.ToString(),
                            p.NgayGui,
                            p.PhanHoi
                        })
                        .ToListAsync(),
                    Services = await _context.DichVus
                        .Select(d => new { d.ID, d.TenDichVu, d.Gia })
                        .ToListAsync(),
                    Residents = await _context.CuDans
                        .Include(c => c.CanHo)
                        .Select(c => new { c.ID, c.ID_NguoiDung, c.ID_CanHo, CanHoMa = c.CanHo != null ? c.CanHo.MaCan : null })
                        .ToListAsync(),
                    ChungCus = await _context.ChungCus
                        .Select(c => new { c.ID, c.Ten, c.DiaChi })
                        .ToListAsync(),
                    CanHos = await _context.CanHos
                        .Select(c => new { c.ID, c.ID_ChungCu, c.MaCan, c.Gia, c.TrangThai })
                        .ToListAsync()
                };

                if (data.Users.Count == 0 && data.Invoices.Count == 0 && data.Complaints.Count == 0 &&
                    data.Services.Count == 0 && data.Residents.Count == 0 && data.ChungCus.Count == 0 && data.CanHos.Count == 0)
                {
                    _logger?.LogWarning("Cơ sở dữ liệu không có dữ liệu để xuất.");
                    return BadRequest("Cơ sở dữ liệu không có dữ liệu để xuất.");
                }

                var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                if (string.IsNullOrEmpty(json))
                {
                    _logger?.LogError("Không thể tuần tự hóa dữ liệu sang JSON.");
                    return StatusCode(500, "Lỗi khi xuất dữ liệu: Không thể tạo tệp JSON.");
                }

                Response.Headers.Add("Content-Disposition", "attachment; filename=qlcc_data.json");
                return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Lỗi khi xuất dữ liệu: {Message}", ex.Message);
                return StatusCode(500, $"Lỗi khi xuất dữ liệu: {ex.Message}");
            }
        }
    }
}