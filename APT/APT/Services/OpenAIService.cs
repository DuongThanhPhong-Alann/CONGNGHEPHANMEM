using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

public class OpenAIService
{
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;

    public OpenAIService(string apiKey)
    {
        _apiKey = apiKey;
        _httpClient = new HttpClient();
    }

    public async Task<string> GetAIResponse(string message)
    {
        var requestData = new
        {
            model = "gpt-3.5-turbo", // Hoặc GPT-4 nếu bạn sử dụng model mới
            messages = new[]
            {
                new { role = "user", content = message }
            }
        };

        var jsonContent = JsonConvert.SerializeObject(requestData);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Thêm Bearer Token vào header
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);

        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(responseBody);
            return result.choices[0].message.content.ToString();
        }
        else
        {
            return $"Lỗi API: {response.StatusCode}. {await response.Content.ReadAsStringAsync()}";
        }
    }
}
