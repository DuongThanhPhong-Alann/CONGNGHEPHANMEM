using Microsoft.AspNetCore.SignalR;
using QLCCCC.Services;
using System.Threading.Tasks;

namespace QLCCCC.Hubs
{
    public class ChatHub : Hub
    {
        private readonly OpenAIService _openAIService;

        public ChatHub(OpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        public async Task SendMessage(string user, string message)
        {
            // Gửi tin nhắn của người dùng đến tất cả kết nối
            await Clients.All.SendAsync("ReceiveMessage", user, message);

            // Gửi câu hỏi của người dùng tới GPT và nhận phản hồi
            var aiResponse = await _openAIService.GetAIResponse(message);

            // Gửi phản hồi từ GPT về cho tất cả kết nối
            await Clients.All.SendAsync("ReceiveMessage", "GPT", aiResponse);
        }
    }
}
