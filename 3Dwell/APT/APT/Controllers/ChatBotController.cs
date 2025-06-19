using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Security.Claims;
using Google.Cloud.Dialogflow.V2;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.EntityFrameworkCore;
using QLCCCC.Data;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using Grpc.Auth;
using Grpc.Core;
using System.Linq;
using QLCCCC.Models;
using System;
using System.Collections.Concurrent;

namespace QLCCCC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatBotController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly SessionsClient? _sessionsClient;
        private readonly string? _projectId;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<ChatBotController> _logger;
        private readonly bool _bypassDialogflow;
        private static readonly ConcurrentDictionary<string, string> _sessionChungCuCache = new ConcurrentDictionary<string, string>();
        private static string[] _lastOptions = Array.Empty<string>(); // Lưu trữ danh sách tùy chọn cuối cùng

        public ChatBotController(IConfiguration configuration, ApplicationDbContext dbContext, ILogger<ChatBotController> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _projectId = _configuration["Dialogflow:ProjectId"];
            _bypassDialogflow = configuration.GetValue<bool>("Dialogflow:Bypass", false);

            var jsonCredentialsPath = _configuration["Dialogflow:JsonCredentialsPath"]
                ?? throw new ArgumentNullException("Dialogflow:JsonCredentialsPath");

            jsonCredentialsPath = Path.Combine(Directory.GetCurrentDirectory(), jsonCredentialsPath);
            try
            {
                using (var stream = new FileStream(jsonCredentialsPath, FileMode.Open, FileAccess.Read))
                {
                    _logger.LogInformation("Tệp chứng thực Dialogflow được tìm thấy tại: {Path}", jsonCredentialsPath);
                }
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogError(ex, "Không tìm thấy tệp chứng thực Dialogflow tại: {Path}", jsonCredentialsPath);
                throw;
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "Lỗi khi truy cập tệp chứng thực Dialogflow tại: {Path}", jsonCredentialsPath);
                throw;
            }

            if (!_bypassDialogflow)
            {
                var credentials = Google.Apis.Auth.OAuth2.GoogleCredential.FromFile(jsonCredentialsPath);
                var channelCredentials = credentials.ToChannelCredentials();
                var builder = new SessionsClientBuilder
                {
                    ChannelCredentials = channelCredentials,
                    Endpoint = "dialogflow.googleapis.com"
                };
                _sessionsClient = builder.Build();
            }
            else
            {
                _logger.LogWarning("Đang chạy ở chế độ bypass Dialogflow.");
                _sessionsClient = null;
            }
        }

        [HttpPost]
        public async Task<IActionResult> ProcessMessage([FromBody] ChatbotRequest? request)
        {
            if (request == null || string.IsNullOrEmpty(request.Message))
            {
                _logger.LogWarning("Yêu cầu không hợp lệ: Message là null hoặc rỗng.");
                // Trả về danh sách tùy chọn cuối cùng nếu có
                if (_lastOptions.Any())
                {
                    return Ok(new { reply = "Vui lòng chọn một tùy chọn:", options = _lastOptions });
                }
                return BadRequest(new { reply = "Yêu cầu không hợp lệ. Vui lòng chọn một tùy chọn hoặc nhập câu hỏi.", options = Array.Empty<string>() });
            }

            var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "Khách";
            var userEmail = User.Identity?.Name ?? string.Empty;
            string responseText = "Xin lỗi, tôi không hiểu yêu cầu của bạn. Vui lòng chọn một tùy chọn hoặc nhập ví dụ: 'Danh sách chung cư', 'Tin tức mới nhất'.";
            var responseOptions = Array.Empty<string>();

            // Log all claims for debugging
            var claims = User.Claims.Select(c => $"{c.Type}: {c.Value}").ToList();
            _logger.LogInformation("Claims người dùng: {Claims}", string.Join("; ", claims));
            _logger.LogInformation("Vai trò người dùng được gán: {UserRole}, Email: {UserEmail}", userRole, userEmail);

            if (string.IsNullOrEmpty(_projectId))
            {
                _logger.LogError("Dialogflow ProjectId chưa được cấu hình.");
                return StatusCode(500, new { reply = "Lỗi hệ thống: Thiếu ProjectId Dialogflow.", options = Array.Empty<string>() });
            }

            var sessionId = Guid.NewGuid().ToString();
            var session = new SessionName(_projectId, sessionId);
            var queryInput = new QueryInput
            {
                Text = new TextInput
                {
                    Text = request.Message.ToLower().Trim(),
                    LanguageCode = "vi"
                }
            };

            try
            {
                string intentResponse;

                if (_bypassDialogflow)
                {
                    _logger.LogInformation("Bypass Dialogflow: Xử lý câu hỏi '{Message}'", request.Message);
                    intentResponse = MapQueryToIntent(request.Message.ToLower().Trim(), sessionId);
                }
                else
                {
                    if (_sessionsClient == null)
                    {
                        _logger.LogError("SessionsClient là null khi không bypass Dialogflow.");
                        return StatusCode(500, new { reply = "Lỗi hệ thống: Dialogflow không được khởi tạo.", options = Array.Empty<string>() });
                    }

                    _logger.LogInformation("Gửi yêu cầu đến Dialogflow: Session={Session}, Message={Message}", session, request.Message);
                    var dialogflowResponse = await _sessionsClient.DetectIntentAsync(session, queryInput);
                    if (dialogflowResponse?.QueryResult == null)
                    {
                        _logger.LogWarning("Phản hồi Dialogflow hoặc QueryResult là null cho câu hỏi: {Message}", request.Message);
                        return Ok(new { reply = "Không thể xử lý yêu cầu do lỗi Dialogflow. Vui lòng thử lại.", options = Array.Empty<string>() });
                    }

                    intentResponse = dialogflowResponse.QueryResult.FulfillmentText?.ToLower().Trim() ?? string.Empty;
                    _logger.LogInformation("Nhận phản hồi từ Dialogflow: Intent={Intent}", intentResponse);
                }

                if (string.IsNullOrEmpty(intentResponse))
                {
                    _logger.LogWarning("Dialogflow trả về ý định rỗng cho câu hỏi: {Message}", request.Message);
                    responseOptions = GetRoleBasedOptions(userRole);
                    _lastOptions = responseOptions;
                    return Ok(new { reply = "Không nhận diện được yêu cầu. Vui lòng chọn một tùy chọn hoặc nhập ví dụ: 'Danh sách chung cư', 'Tin tức mới nhất'.", options = responseOptions });
                }

                _logger.LogInformation("Vai trò người dùng: {Role}, Email: {Email}, Câu hỏi: {Message}, Ý định: {Intent}", userRole, userEmail, request.Message, intentResponse);

                if (intentResponse == "intent: welcome")
                {
                    responseText = "Chào mừng bạn đến với APT Chatbot! Vui lòng chọn một tùy chọn để bắt đầu:";
                    responseOptions = GetRoleBasedOptions(userRole);
                    if (userRole == "Cư dân")
                    {
                        responseText += " Bạn cũng có thể tìm hiểu về Hóa đơn hoặc Phản ánh.";
                    }
                    else if (userRole == "Ban quản lý")
                    {
                        responseText += " Bạn có quyền truy cập tất cả thông tin, bao gồm tổng quan cư dân, hóa đơn, và phản ánh.";
                    }
                }
                else if (userRole == "Khách")
                {
                    if (intentResponse.Contains("hỏi menu") || intentResponse.Contains("hỏi danh sách") ||
                        intentResponse.Contains("xem thông tin chung cư") || intentResponse.Contains("hỏi tổng số căn hộ chung cư") ||
                        intentResponse.Contains("hỏi danh sách căn hộ chung cư") || intentResponse.Contains("xem thông tin căn hộ") ||
                        intentResponse.Contains("hỏi tổng số dịch vụ") || intentResponse.Contains("hỏi danh sách dịch vụ") ||
                        intentResponse.Contains("xem thông tin dịch vụ") || intentResponse.Contains("hỏi tin tức mới nhất") ||
                        intentResponse.Contains("hỏi tổng số chung cư"))
                    {
                        var (text, options) = await ProcessGeneralIntent(intentResponse, request.Message.ToLower().Trim(), userEmail, sessionId);
                        responseText = text;
                        responseOptions = options;
                    }
                    else
                    {
                        responseText = "Quý khách chỉ có thể truy vấn thông tin về Chung cư, Căn hộ, Dịch vụ, hoặc Tin tức. Vui lòng chọn một tùy chọn hoặc nhập ví dụ: 'Danh sách chung cư', 'Tin tức mới nhất'.";
                        responseOptions = GetRoleBasedOptions(userRole);
                    }
                }
                else if (userRole == "Cư dân")
                {
                    if (intentResponse.Contains("hỏi hóa đơn"))
                    {
                        responseText = await ProcessResidentIntent(intentResponse, request.Message.ToLower().Trim(), userEmail, userRole);
                        responseOptions = GetRoleBasedOptions(userRole);
                    }
                    else if (intentResponse.Contains("hỏi phản ánh"))
                    {
                        responseText = await ProcessResidentIntent(intentResponse, request.Message.ToLower().Trim(), userEmail, userRole);
                        responseOptions = GetRoleBasedOptions(userRole);
                    }
                    else if (intentResponse.Contains("hỏi menu") || intentResponse.Contains("hỏi danh sách") ||
                             intentResponse.Contains("xem thông tin chung cư") || intentResponse.Contains("hỏi tổng số căn hộ chung cư") ||
                             intentResponse.Contains("hỏi danh sách căn hộ chung cư") || intentResponse.Contains("xem thông tin căn hộ") ||
                             intentResponse.Contains("hỏi tổng số dịch vụ") || intentResponse.Contains("hỏi danh sách dịch vụ") ||
                             intentResponse.Contains("xem thông tin dịch vụ") || intentResponse.Contains("hỏi tin tức mới nhất") ||
                             intentResponse.Contains("hỏi tổng số chung cư"))
                    {
                        var (text, options) = await ProcessGeneralIntent(intentResponse, request.Message.ToLower().Trim(), userEmail, sessionId);
                        responseText = text;
                        responseOptions = options;
                    }
                    else
                    {
                        responseText = "Quý khách chỉ có thể truy vấn thông tin về Chung cư, Căn hộ, Dịch vụ, Tin tức, Hóa đơn, hoặc Phản ánh. Vui lòng chọn một tùy chọn hoặc nhập ví dụ: 'Hóa đơn của tôi', 'Danh sách chung cư'.";
                        responseOptions = GetRoleBasedOptions(userRole);
                    }
                }
                else if (userRole == "Ban quản lý")
                {
                    if (intentResponse.Contains("hỏi hóa đơn"))
                    {
                        var (text, options) = await ProcessInvoiceIntent(request.Message.ToLower().Trim(), userEmail, userRole);
                        responseText = text;
                        responseOptions = options;
                    }
                    else if (intentResponse.Contains("hỏi phản ánh") || intentResponse.Contains("hỏi toàn bộ cư dân") ||
                             intentResponse.Contains("hỏi toàn bộ người dùng"))
                    {
                        responseText = await ProcessResidentIntent(intentResponse, request.Message.ToLower().Trim(), userEmail, userRole);
                        responseOptions = GetRoleBasedOptions(userRole);
                    }
                    else if (intentResponse.Contains("hỏi menu") || intentResponse.Contains("hỏi danh sách") ||
                             intentResponse.Contains("xem thông tin chung cư") || intentResponse.Contains("hỏi tổng số căn hộ chung cư") ||
                             intentResponse.Contains("hỏi danh sách căn hộ chung cư") || intentResponse.Contains("xem thông tin căn hộ") ||
                             intentResponse.Contains("hỏi tổng số dịch vụ") || intentResponse.Contains("hỏi danh sách dịch vụ") ||
                             intentResponse.Contains("xem thông tin dịch vụ") || intentResponse.Contains("hỏi tin tức mới nhất") ||
                             intentResponse.Contains("hỏi tổng số chung cư"))
                    {
                        var (text, options) = await ProcessGeneralIntent(intentResponse, request.Message.ToLower().Trim(), userEmail, sessionId);
                        responseText = text;
                        responseOptions = options;
                    }
                    else
                    {
                        responseText = "Xin lỗi, tôi không hiểu yêu cầu của bạn. Vui lòng chọn một tùy chọn hoặc nhập ví dụ: 'Danh sách dịch vụ'.";
                        responseOptions = GetRoleBasedOptions(userRole);
                    }
                }
                else if (intentResponse == "intent: fallback")
                {
                    responseText = "Xin lỗi, tôi không hiểu yêu cầu của bạn. Vui lòng chọn một tùy chọn hoặc nhập ví dụ: 'Danh sách chung cư', 'Tin tức mới nhất'.";
                    responseOptions = GetRoleBasedOptions(userRole);
                    if (userRole == "Cư dân")
                    {
                        responseText += " Bạn cũng có thể hỏi về Hóa đơn hoặc Phản ánh.";
                    }
                    else if (userRole == "Ban quản lý")
                    {
                        responseText += " Bạn có quyền truy cập tất cả thông tin, bao gồm tổng quan cư dân, hóa đơn, và phản ánh.";
                    }
                }

                // Lưu danh sách tùy chọn cuối cùng
                _lastOptions = responseOptions;

                // Sử dụng indexer để thêm tiêu đề
                Response.Headers["X-Chatbot-Intent"] = intentResponse;

                return Ok(new { reply = responseText, options = responseOptions });
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "Lỗi khi gọi Dialogflow: Message={Message}, StatusCode={StatusCode}, Detail={Detail}", request.Message, ex.StatusCode, ex.Status.Detail);
                return StatusCode(500, new { reply = "Lỗi hệ thống: Không thể kết nối với Dialogflow. Vui lòng thử lại.", options = Array.Empty<string>() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định khi xử lý câu hỏi: Message={Message}", request.Message);
                return StatusCode(500, new { reply = "Lỗi hệ thống: Đã có lỗi xảy ra. Vui lòng thử lại.", options = Array.Empty<string>() });
            }
        }

        private string[] GetRoleBasedOptions(string userRole)
        {
            if (userRole == "Cư dân")
            {
                return new string[] { "Chung cư", "Căn hộ", "Dịch vụ", "Tin tức", "Hóa đơn", "Phản ánh" }.Distinct().ToArray();
            }
            else if (userRole == "Ban quản lý")
            {
                // Thay "Tổng số cư dân" bằng "Toàn bộ cư dân" và thêm "Toàn bộ người dùng"
                return new string[] { "Chung cư", "Căn hộ", "Dịch vụ", "Tin tức", "Hóa đơn", "Phản ánh", "Toàn bộ cư dân", "Toàn bộ người dùng" }.Distinct().ToArray();
            }
            return new string[] { "Chung cư", "Căn hộ", "Dịch vụ", "Tin tức" }.Distinct().ToArray();
        }

        private string MapQueryToIntent(string queryText, string sessionId)
        {
            queryText = queryText.ToLower().Trim();
            if (queryText == "chung cư")
                return "intent: hỏi menu chung cư";
            if (queryText == "căn hộ")
                return "intent: hỏi menu căn hộ";
            if (queryText == "dịch vụ")
                return "intent: hỏi menu dịch vụ";
            if (queryText == "tin tức" || queryText == "tin tức mới nhất" || queryText == "tin tức mới nhất?")
                return "intent: hỏi tin tức mới nhất";
            if (queryText == "tổng số chung cư" || queryText == "tổng số chung cư?")
                return "intent: hỏi tổng số chung cư";
            if (queryText == "danh sách chung cư")
                return "intent: hỏi danh sách chung cư";
            if (queryText.StartsWith("xem chung cư"))
            {
                var chungCuName = queryText.Replace("xem chung cư", "").Trim();
                _sessionChungCuCache[sessionId] = chungCuName;
                return $"intent: xem thông tin chung cư {chungCuName}";
            }
            if (queryText.StartsWith("tổng số căn hộ của chung cư"))
            {
                var chungCuName = queryText.Replace("tổng số căn hộ của chung cư", "").Replace("?", "").Trim();
                if (chungCuName == "này" && _sessionChungCuCache.TryGetValue(sessionId, out var cachedName))
                    chungCuName = cachedName;
                _sessionChungCuCache[sessionId] = chungCuName;
                return $"intent: hỏi tổng số căn hộ chung cư {chungCuName}";
            }
            if (queryText.StartsWith("danh sách căn hộ của chung cư") || queryText == "danh sách căn hộ của chung cư này?")
            {
                var chungCuName = queryText.Replace("danh sách căn hộ của chung cư", "").Replace("?", "").Trim();
                if (chungCuName == "này" && _sessionChungCuCache.TryGetValue(sessionId, out var cachedName))
                    chungCuName = cachedName;
                _sessionChungCuCache[sessionId] = chungCuName;
                return $"intent: hỏi danh sách căn hộ chung cư {chungCuName}";
            }
            if (queryText.StartsWith("chung cư "))
            {
                var chungCuName = queryText.Replace("chung cư ", "").Trim();
                _sessionChungCuCache[sessionId] = chungCuName;
                return $"intent: hỏi danh sách căn hộ chung cư {chungCuName}";
            }
            if (queryText.StartsWith("xem căn hộ"))
            {
                var match = Regex.Match(queryText, @"xem căn hộ (\S+) thuộc chung cư (.+)");
                if (match.Success)
                {
                    var maCan = match.Groups[1].Value;
                    var chungCuName = match.Groups[2].Value.Trim();
                    return $"intent: xem thông tin căn hộ {maCan} thuộc chung cư {chungCuName}";
                }
                return "intent: xem thông tin căn hộ";
            }
            if (queryText == "hóa đơn đã thanh toán")
                return "intent: hỏi hóa đơn đã thanh toán";
            if (queryText == "hóa đơn chưa thanh toán")
                return "intent: hỏi hóa đơn chưa thanh toán";
            if (queryText == "hóa đơn" || queryText == "hóa đơn của tôi" || queryText == "xem hóa đơn")
                return "intent: hỏi hóa đơn";
            if (queryText == "phản ánh" || queryText == "phản ánh của tôi" || queryText == "xem phản ánh")
                return "intent: hỏi phản ánh";
            if (queryText == "toàn bộ cư dân")
                return "intent: hỏi toàn bộ cư dân";
            if (queryText == "toàn bộ người dùng")
                return "intent: hỏi toàn bộ người dùng";
            if (queryText == "xin chào" || queryText == "hi" || queryText == "hello" || queryText == "chào")
                return "intent: welcome";
            if (queryText == "tổng số dịch vụ" || queryText == "tổng số dịch vụ hiện có?")
                return "intent: hỏi tổng số dịch vụ";
            if (queryText == "danh sách dịch vụ")
                return "intent: hỏi danh sách dịch vụ";
            if (queryText.StartsWith("xem dịch vụ"))
                return $"intent: xem thông tin dịch vụ {queryText.Replace("xem dịch vụ", "").Trim()}";
            if (Regex.IsMatch(queryText, @"^\d+$") || queryText.Contains("đồng") || queryText.Contains("nhắn"))
                return "intent: fallback";
            return "intent: fallback";
        }

        private async Task<(string text, string[] options)> ProcessGeneralIntent(string intent, string queryText, string userEmail, string sessionId)
        {
            try
            {
                if (intent.Contains("hỏi menu chung cư"))
                {
                    return ("Vui lòng chọn thông tin bạn muốn tìm hiểu về chung cư:", new string[] { "Tổng số chung cư", "Danh sách chung cư" });
                }
                else if (intent.Contains("hỏi tổng số chung cư"))
                {
                    var count = await _dbContext.ChungCus.CountAsync();
                    return ($"Hiện có {count} chung cư trong hệ thống. Bạn muốn quay lại menu chung cư?", new string[] { "Chung cư" });
                }
                else if (intent.Contains("hỏi danh sách chung cư"))
                {
                    var chungCus = await _dbContext.ChungCus.Select(c => c.Ten).ToListAsync();
                    if (chungCus.Any())
                    {
                        return ($"Danh sách các chung cư hiện có: {string.Join(", ", chungCus)}. Vui lòng chọn một chung cư để xem chi tiết:", chungCus.Select(c => $"Xem chung cư {c}").ToArray());
                    }
                    return ("Hiện tại không có chung cư nào trong hệ thống.", new string[] { "Chung cư" });
                }
                else if (intent.Contains("xem thông tin chung cư"))
                {
                    var chungCuName = queryText.Replace("xem chung cư", "").Trim();
                    var chungCu = await _dbContext.ChungCus.FirstOrDefaultAsync(c => c.Ten.ToLower().Contains(chungCuName));
                    if (chungCu != null)
                    {
                        _sessionChungCuCache[sessionId] = chungCu.Ten;
                        return ($"Thông tin chung cư {chungCu.Ten}: Địa chỉ: {chungCu.DiaChi}. Vui lòng chọn:", new string[] { $"Tổng số căn hộ của chung cư {chungCu.Ten}", $"Danh sách căn hộ của chung cư {chungCu.Ten}" });
                    }
                    return ("Không tìm thấy thông tin chung cư. Vui lòng chọn một chung cư khác:", new string[] { "Danh sách chung cư" });
                }
                else if (intent.Contains("hỏi menu căn hộ"))
                {
                    var chungCus = await _dbContext.ChungCus.Select(c => c.Ten).ToListAsync();
                    _logger.LogInformation("Danh sách chung cư tìm thấy: {ChungCus}", string.Join(", ", chungCus));
                    if (chungCus.Any())
                    {
                        return ($"Vui lòng chọn một chung cư để xem thông tin căn hộ:", chungCus.Select(c => $"Chung cư {c}").ToArray());
                    }
                    return ("Hiện tại không có chung cư nào để xem căn hộ.", new string[] { "Chung cư" });
                }
                else if (intent.Contains("hỏi tổng số căn hộ chung cư"))
                {
                    var chungCuName = queryText.Replace("tổng số căn hộ của chung cư", "").Replace("?", "").Trim();
                    if (chungCuName == "này" && _sessionChungCuCache.TryGetValue(sessionId, out var cachedName))
                        chungCuName = cachedName;
                    var chungCu = await _dbContext.ChungCus.FirstOrDefaultAsync(c => c.Ten.ToLower().Contains(chungCuName));
                    if (chungCu != null)
                    {
                        var count = await _dbContext.CanHos.CountAsync(c => c.ID_ChungCu == chungCu.ID);
                        return ($"Chung cư {chungCu.Ten} hiện có {count} căn hộ. Bạn muốn xem danh sách căn hộ?", new string[] { $"Danh sách căn hộ của chung cư {chungCu.Ten}" });
                    }
                    return ("Không tìm thấy thông tin chung cư. Vui lòng chọn một chung cư khác:", new string[] { "Danh sách chung cư" });
                }
                else if (intent.Contains("hỏi danh sách căn hộ chung cư"))
                {
                    var chungCuName = queryText.Replace("danh sách căn hộ của chung cư", "").Replace("?", "").Replace("chung cư ", "").Trim();
                    if (chungCuName == "này" && _sessionChungCuCache.TryGetValue(sessionId, out var cachedName))
                        chungCuName = cachedName;
                    var chungCu = await _dbContext.ChungCus.FirstOrDefaultAsync(c => c.Ten.ToLower().Contains(chungCuName));
                    if (chungCu != null)
                    {
                        var canHos = await _dbContext.CanHos
                            .Where(c => c.ID_ChungCu == chungCu.ID)
                            .Select(c => c.MaCan)
                            .ToListAsync();
                        if (canHos.Any())
                        {
                            return ($"Danh sách căn hộ của chung cư {chungCu.Ten}: {string.Join(", ", canHos)}. Vui lòng chọn một căn hộ để xem chi tiết:", canHos.Select(c => $"Xem căn hộ {c} thuộc chung cư {chungCu.Ten}").ToArray());
                        }
                        return ($"Chung cư {chungCu.Ten} hiện chưa có căn hộ nào.", new string[] { "Danh sách chung cư" });
                    }
                    return ("Không tìm thấy thông tin chung cư. Vui lòng chọn một chung cư khác:", new string[] { "Danh sách chung cư" });
                }
                else if (intent.Contains("xem thông tin căn hộ"))
                {
                    var match = Regex.Match(queryText, @"xem căn hộ (\S+) thuộc chung cư (.+)");
                    if (match.Success)
                    {
                        var maCan = match.Groups[1].Value;
                        var chungCuName = match.Groups[2].Value.Trim();
                        var canHo = await _dbContext.CanHos
                            .Include(c => c.ChungCu)
                            .FirstOrDefaultAsync(c => c.MaCan.ToLower() == maCan.ToLower() && c.ChungCu != null && c.ChungCu.Ten.ToLower().Contains(chungCuName));
                        if (canHo != null && canHo.ChungCu != null)
                        {
                            return ($"Thông tin căn hộ {canHo.MaCan}, chung cư {canHo.ChungCu.Ten}: Giá: {canHo.Gia:N2} đồng, Trạng thái: {canHo.TrangThai}. Bạn muốn quay lại danh sách căn hộ?", new string[] { $"Danh sách căn hộ của chung cư {canHo.ChungCu.Ten}" });
                        }
                    }
                    return ("Không tìm thấy thông tin căn hộ. Vui lòng nhập đúng định dạng: 'Xem căn hộ [Mã căn] thuộc chung cư [Tên]' hoặc chọn một căn hộ từ danh sách.", new string[] { "Căn hộ" });
                }
                else if (intent.Contains("hỏi menu dịch vụ"))
                {
                    return ("Vui lòng chọn thông tin bạn muốn tìm hiểu về dịch vụ:", new string[] { "Tổng số dịch vụ", "Danh sách dịch vụ" });
                }
                else if (intent.Contains("hỏi tổng số dịch vụ"))
                {
                    var count = await _dbContext.DichVus.CountAsync();
                    return ($"Hiện có {count} dịch vụ trong hệ thống. Bạn muốn xem danh sách dịch vụ?", new string[] { "Danh sách dịch vụ" });
                }
                else if (intent.Contains("hỏi danh sách dịch vụ"))
                {
                    var services = await _dbContext.DichVus.Select(d => d.TenDichVu).ToListAsync();
                    if (services.Any())
                    {
                        return ($"Danh sách các dịch vụ hiện có: {string.Join(", ", services)}. Vui lòng chọn một dịch vụ để xem chi tiết:", services.Select(s => $"Xem dịch vụ {s}").ToArray());
                    }
                    return ("Hiện tại không có dịch vụ nào.", new string[] { "Dịch vụ" });
                }
                else if (intent.Contains("xem thông tin dịch vụ"))
                {
                    var dichVuName = queryText.Replace("xem dịch vụ", "").Trim();
                    var dichVu = await _dbContext.DichVus.FirstOrDefaultAsync(d => d.TenDichVu.ToLower().Contains(dichVuName));
                    if (dichVu != null)
                    {
                        return ($"Thông tin dịch vụ {dichVu.TenDichVu}: Giá: {dichVu.Gia:N2} đồng, Mô tả: {dichVu.MoTa ?? "Không có"}. Bạn muốn quay lại danh sách dịch vụ?", new string[] { "Danh sách dịch vụ" });
                    }
                    return ("Không tìm thấy thông tin dịch vụ. Vui lòng chọn một dịch vụ khác:", new string[] { "Danh sách dịch vụ" });
                }
                else if (intent.Contains("hỏi tin tức mới nhất"))
                {
                    var tinTuc = await _dbContext.TinTucs.OrderByDescending(t => t.NgayDang).FirstOrDefaultAsync();
                    if (tinTuc != null)
                    {
                        return ($"Tin tức mới nhất: {tinTuc.TieuDe}, đăng ngày {tinTuc.NgayDang:dd/MM/yyyy}. Bạn muốn quay lại menu tin tức?", new string[] { "Tin tức" });
                    }
                    return ("Hiện tại chưa có tin tức mới.", new string[] { "Tin tức" });
                }

                _logger.LogWarning("Ý định không được xử lý: {Intent}, Query: {QueryText}", intent, queryText);
                return ("Xin lỗi, tôi không hiểu yêu cầu của bạn. Vui lòng chọn một tùy chọn hoặc nhập ví dụ: 'Danh sách chung cư', 'Tin tức mới nhất'.", new string[] { "Chung cư", "Căn hộ", "Dịch vụ", "Tin tức" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý ý định chung: Intent={Intent}, QueryText={QueryText}", intent, queryText);
                return ("Lỗi khi xử lý yêu cầu. Vui lòng thử lại.", new string[] { "Chung cư", "Căn hộ", "Dịch vụ", "Tin tức" });
            }
        }

        private async Task<(string text, string[] options)> ProcessInvoiceIntent(string queryText, string userEmail, string userRole)
        {
            try
            {
                if (string.IsNullOrEmpty(userEmail))
                {
                    _logger.LogError("Identifier người dùng là null hoặc rỗng khi xử lý ý định hóa đơn: QueryText={QueryText}", queryText);
                    return ("Lỗi: Không tìm thấy thông tin đăng nhập. Vui lòng đăng xuất và đăng nhập lại.", Array.Empty<string>());
                }

                // Log the exact identifier being queried
                _logger.LogInformation("Tìm kiếm người dùng với identifier: '{Identifier}' (Raw: '{RawIdentifier}')", userEmail, userEmail);

                // Try to find email from claims
                string? effectiveEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email || c.Type == "email")?.Value;
                if (string.IsNullOrEmpty(effectiveEmail))
                {
                    _logger.LogWarning("Không tìm thấy email trong claims, sử dụng identifier gốc: '{Identifier}'", userEmail);
                    effectiveEmail = userEmail;
                }

                // Log database state
                var userCount = await _dbContext.NguoiDungs.CountAsync();
                var userEmails = await _dbContext.NguoiDungs.Select(u => u.Email).ToListAsync();
                _logger.LogInformation("Tổng số người dùng trong NguoiDungs: {Count}. Emails: {Emails}", userCount, string.Join(", ", userEmails));

                var user = await _dbContext.NguoiDungs
                    .FirstOrDefaultAsync(u => u.Email != null && u.Email.ToLower().Trim() == effectiveEmail.ToLower().Trim());
                if (user == null)
                {
                    _logger.LogWarning("Không tìm thấy người dùng với email: '{Email}'", effectiveEmail);
                    return ($"Lỗi: Không tìm thấy thông tin tài khoản với email '{effectiveEmail}' trong hệ thống. Vui lòng liên hệ quản trị viên để kiểm tra.", Array.Empty<string>());
                }

                var emailInQuery = ExtractEmailFromQuery(queryText);
                if (emailInQuery != null && emailInQuery.ToLower().Trim() != effectiveEmail.ToLower().Trim())
                {
                    _logger.LogWarning("Email trong truy vấn ({EmailInQuery}) không khớp với email người dùng ({UserEmail})", emailInQuery, effectiveEmail);
                    return ("Bạn chỉ có thể truy vấn thông tin về tài khoản của mình.", Array.Empty<string>());
                }

                if (queryText.Contains("hóa đơn đã thanh toán"))
                {
                    var paidInvoices = await _dbContext.HoaDonDichVus
                        .Where(h => h.TrangThai == "Đã thanh toán")
                        .Select(h => new { h.ID, h.ID_CanHo, h.SoTien, h.TrangThai, h.NgayLap })
                        .ToListAsync();
                    if (paidInvoices.Any())
                    {
                        return ($"Tổng cộng có {paidInvoices.Count} hóa đơn đã thanh toán: {string.Join("; ", paidInvoices.Select(i => $"ID: {i.ID}, Căn hộ ID: {i.ID_CanHo}, Số tiền: {i.SoTien:N2} đồng, Ngày lập: {i.NgayLap:dd/MM/yyyy}"))}.", _lastOptions);
                    }
                    return ("Hiện tại không có hóa đơn nào đã thanh toán.", _lastOptions);
                }
                else if (queryText.Contains("hóa đơn chưa thanh toán"))
                {
                    var unpaidInvoices = await _dbContext.HoaDonDichVus
                        .Where(h => h.TrangThai == "Chưa thanh toán")
                        .Select(h => new { h.ID, h.ID_CanHo, h.SoTien, h.TrangThai, h.NgayLap })
                        .ToListAsync();
                    if (unpaidInvoices.Any())
                    {
                        return ($"Tổng cộng có {unpaidInvoices.Count} hóa đơn chưa thanh toán: {string.Join("; ", unpaidInvoices.Select(i => $"ID: {i.ID}, Căn hộ ID: {i.ID_CanHo}, Số tiền: {i.SoTien:N2} đồng, Ngày lập: {i.NgayLap:dd/MM/yyyy}"))}.", _lastOptions);
                    }
                    return ("Hiện tại không có hóa đơn nào chưa thanh toán.", _lastOptions);
                }
                else
                {
                    // Trả về chỉ hai mục con khi ý định là "hỏi hóa đơn"
                    return ("Vui lòng chọn loại hóa đơn bạn muốn xem:", new string[] { "Hóa đơn đã thanh toán", "Hóa đơn chưa thanh toán" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý ý định hóa đơn: QueryText={QueryText}, UserEmail={UserEmail}", queryText, userEmail);
                return ("Lỗi khi xử lý yêu cầu. Vui lòng thử lại hoặc liên hệ quản trị viên.", Array.Empty<string>());
            }
        }

        private async Task<string> ProcessResidentIntent(string intent, string queryText, string userEmail, string userRole)
        {
            try
            {
                if (string.IsNullOrEmpty(userEmail))
                {
                    _logger.LogError("Identifier người dùng là null hoặc rỗng khi xử lý ý định: {Intent}, QueryText={QueryText}", intent, queryText);
                    return "Lỗi: Không tìm thấy thông tin đăng nhập. Vui lòng đăng xuất và đăng nhập lại.";
                }

                // Log the exact identifier being queried
                _logger.LogInformation("Tìm kiếm người dùng với identifier: '{Identifier}' (Raw: '{RawIdentifier}')", userEmail, userEmail);

                // Try to find email from claims
                string? effectiveEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email || c.Type == "email")?.Value;
                if (string.IsNullOrEmpty(effectiveEmail))
                {
                    _logger.LogWarning("Không tìm thấy email trong claims, sử dụng identifier gốc: '{Identifier}'", userEmail);
                    effectiveEmail = userEmail;
                }

                // Log database state
                var userCount = await _dbContext.NguoiDungs.CountAsync();
                var userEmails = await _dbContext.NguoiDungs.Select(u => u.Email).ToListAsync();
                _logger.LogInformation("Tổng số người dùng trong NguoiDungs: {Count}. Emails: {Emails}", userCount, string.Join(", ", userEmails));

                var user = await _dbContext.NguoiDungs
                    .FirstOrDefaultAsync(u => u.Email != null && u.Email.ToLower().Trim() == effectiveEmail.ToLower().Trim());
                if (user == null)
                {
                    _logger.LogWarning("Không tìm thấy người dùng với email: '{Email}'", effectiveEmail);
                    return $"Lỗi: Không tìm thấy thông tin tài khoản với email '{effectiveEmail}' trong hệ thống. Vui lòng liên hệ quản trị viên để kiểm tra.";
                }

                var emailInQuery = ExtractEmailFromQuery(queryText);
                if (emailInQuery != null && emailInQuery.ToLower().Trim() != effectiveEmail.ToLower().Trim())
                {
                    _logger.LogWarning("Email trong truy vấn ({EmailInQuery}) không khớp với email người dùng ({UserEmail})", emailInQuery, effectiveEmail);
                    return "Bạn chỉ có thể truy vấn thông tin về tài khoản của mình.";
                }

                if (intent.Contains("hỏi hóa đơn"))
                {
                    if (userRole == "Cư dân")
                    {
                        _logger.LogInformation("Xử lý truy vấn hóa đơn cho người dùng ID: {UserId}, Email: {Email}", user.ID, userEmail);
                        // Tìm căn hộ của cư dân
                        var canHo = await _dbContext.CuDans
                            .Where(c => c.ID_NguoiDung == user.ID)
                            .Select(c => c.ID_CanHo)
                            .FirstOrDefaultAsync();

                        if (canHo == 0)
                        {
                            return "Bạn hiện không có căn hộ nào được liên kết. Vui lòng liên hệ quản trị viên.";
                        }

                        // Lấy danh sách hóa đơn của căn hộ
                        var invoices = await _dbContext.HoaDonDichVus
                            .Where(h => h.ID_CanHo == canHo)
                            .Select(h => new { h.ID, h.ID_CanHo, h.SoTien, h.TrangThai, h.NgayLap })
                            .ToListAsync();

                        if (invoices.Any())
                        {
                            if (queryText.Contains("đã thanh toán"))
                            {
                                var paidInvoices = invoices.Where(h => h.TrangThai == "Đã thanh toán").ToList();
                                if (paidInvoices.Any())
                                {
                                    return $"Bạn có {paidInvoices.Count} hóa đơn đã thanh toán: {string.Join("; ", paidInvoices.Select(i => $"ID: {i.ID}, Căn hộ ID: {i.ID_CanHo}, Số tiền: {i.SoTien:N2} đồng, Ngày lập: {i.NgayLap:dd/MM/yyyy}"))}.";
                                }
                                return "Bạn không có hóa đơn nào đã thanh toán.";
                            }
                            else if (queryText.Contains("chưa thanh toán"))
                            {
                                var unpaidInvoices = invoices.Where(h => h.TrangThai == "Chưa thanh toán").ToList();
                                if (unpaidInvoices.Any())
                                {
                                    return $"Bạn có {unpaidInvoices.Count} hóa đơn chưa thanh toán: {string.Join("; ", unpaidInvoices.Select(i => $"ID: {i.ID}, Căn hộ ID: {i.ID_CanHo}, Số tiền: {i.SoTien:N2} đồng, Ngày lập: {i.NgayLap:dd/MM/yyyy}"))}.";
                                }
                                return "Bạn không có hóa đơn nào chưa thanh toán.";
                            }
                            return $"Bạn có {invoices.Count} hóa đơn: {string.Join("; ", invoices.Select(i => $"ID: {i.ID}, Căn hộ ID: {i.ID_CanHo}, Số tiền: {i.SoTien:N2} đồng, Trạng thái: {i.TrangThai}, Ngày lập: {i.NgayLap:dd/MM/yyyy}"))}.";
                        }
                        return "Bạn chưa có hóa đơn nào.";
                    }
                    else if (userRole == "Ban quản lý")
                    {
                        // Logic này sẽ được xử lý trong ProcessInvoiceIntent
                        return "Vui lòng chọn loại hóa đơn bạn muốn xem: Hóa đơn đã thanh toán, Hóa đơn chưa thanh toán.";
                    }
                }
                else if (intent.Contains("hỏi phản ánh"))
                {
                    if (userRole == "Cư dân")
                    {
                        _logger.LogInformation("Xử lý truy vấn phản ánh cho người dùng ID: {UserId}, Email: {Email}", user.ID, userEmail);
                        var complaints = await _dbContext.PhanAnhs
                            .Where(c => c.ID_NguoiDung == user.ID)
                            .Select(c => new { c.ID, c.NoiDung, c.TrangThai, c.NgayGui })
                            .ToListAsync();

                        // Log complaint details
                        _logger.LogInformation("Số lượng phản ánh tìm thấy: {Count}. Phản ánh: {Complaints}", complaints.Count, string.Join("; ", complaints.Select(c => $"ID: {c.ID}, Nội dung: {c.NoiDung}, Trạng thái: {c.TrangThai}, Ngày gửi: {c.NgayGui}")));

                        if (complaints.Any())
                        {
                            if (queryText.Contains("chưa xử lý") || intent.Contains("phản ánh chưa xử lý"))
                            {
                                var pendingComplaints = complaints.Where(c => c.TrangThai == TrangThaiPhanAnh.ChuaXuLy).ToList();
                                if (pendingComplaints.Any())
                                {
                                    return $"Bạn có {pendingComplaints.Count} phản ánh chưa xử lý: {string.Join("; ", pendingComplaints.Select(c => $"ID: {c.ID}, Nội dung: {c.NoiDung}, Ngày gửi: {c.NgayGui:dd/MM/yyyy}"))}.";
                                }
                                return "Bạn không có phản ánh chưa xử lý.";
                            }
                            return $"Bạn có {complaints.Count} phản ánh: {string.Join("; ", complaints.Select(c => $"ID: {c.ID}, Nội dung: {c.NoiDung}, Trạng thái: {c.TrangThai}, Ngày gửi: {c.NgayGui:dd/MM/yyyy}"))}.";
                        }
                        return "Bạn chưa có phản ánh nào.";
                    }
                    else if (userRole == "Ban quản lý")
                    {
                        var complaints = await _dbContext.PhanAnhs
                            .Select(c => new { c.ID, c.ID_NguoiDung, c.NoiDung, c.TrangThai, c.NgayGui })
                            .ToListAsync();

                        if (complaints.Any())
                        {
                            return $"Tổng cộng có {complaints.Count} phản ánh: {string.Join("; ", complaints.Select(c => $"ID: {c.ID}, Người dùng ID: {c.ID_NguoiDung}, Nội dung: {c.NoiDung}, Trạng thái: {c.TrangThai}, Ngày gửi: {c.NgayGui:dd/MM/yyyy}"))}.";
                        }
                        return "Hiện tại không có phản ánh nào trong hệ thống.";
                    }
                }
                else if (intent.Contains("hỏi toàn bộ cư dân"))
                {
                    if (userRole != "Ban quản lý")
                    {
                        return "Chỉ Ban quản lý mới có quyền truy vấn thông tin toàn bộ cư dân.";
                    }

                    var residents = await _dbContext.CuDans
                        .Select(r => new { r.ID, r.ID_NguoiDung, r.ID_CanHo })
                        .ToListAsync();

                    if (residents.Any())
                    {
                        return $"Tổng cộng có {residents.Count} cư dân: {string.Join("; ", residents.Select(r => $"ID: {r.ID}, Người dùng ID: {r.ID_NguoiDung}, Căn hộ ID: {r.ID_CanHo}"))}.";
                    }
                    return "Hiện tại không có cư dân nào trong hệ thống.";
                }
                else if (intent.Contains("hỏi toàn bộ người dùng"))
                {
                    if (userRole != "Ban quản lý")
                    {
                        return "Chỉ Ban quản lý mới có quyền truy vấn thông tin toàn bộ người dùng.";
                    }

                    var users = await _dbContext.NguoiDungs
                        .Select(u => new { u.ID, u.HoTen, u.Email })
                        .ToListAsync();

                    if (users.Any())
                    {
                        return $"Tổng cộng có {users.Count} người dùng: {string.Join("; ", users.Select(u => $"ID: {u.ID}, Họ tên: {u.HoTen}, Email: {u.Email}"))}.";
                    }
                    return "Hiện tại không có người dùng nào trong hệ thống.";
                }

                _logger.LogWarning("Ý định cư dân không được xử lý: {Intent}, Query: {QueryText}", intent, queryText);
                return "Xin lỗi, tôi không hiểu yêu cầu của bạn. Vui lòng chọn một tùy chọn hoặc nhập ví dụ: 'Hóa đơn của tôi', 'Phản ánh của tôi'.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý ý định cư dân: Intent={Intent}, QueryText={QueryText}, UserEmail={UserEmail}", intent, queryText, userEmail);
                return "Lỗi khi xử lý yêu cầu. Vui lòng thử lại hoặc liên hệ quản trị viên.";
            }
        }

        private string? ExtractEmailFromQuery(string queryText)
        {
            var words = queryText.Split(' ');
            foreach (var word in words)
            {
                if (word.Contains("@") && word.Contains("."))
                {
                    return word.Trim();
                }
            }
            return null;
        }
    }

    public class ChatbotRequest
    {
        public required string Message { get; set; }
    }
}