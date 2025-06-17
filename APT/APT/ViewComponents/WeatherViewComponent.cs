using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace QLCCCC.ViewComponents
{
    public class WeatherViewComponent : ViewComponent
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<IViewComponentResult> InvokeAsync(string city = "Ho Chi Minh")
        {
            string apiKey = "96e280e6c6f5ff134d49420a461773b2";
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric&lang=vi";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return View("Error");

            var json = await response.Content.ReadAsStringAsync();
            JObject data = JObject.Parse(json);

            ViewBag.City = data["name"];
            ViewBag.Temp = data["main"]["temp"];
            ViewBag.FeelsLike = data["main"]["feels_like"];
            ViewBag.Humidity = data["main"]["humidity"];
            ViewBag.Description = data["weather"][0]["description"];
            ViewBag.Icon = data["weather"][0]["icon"];

            return View("Default");
        }
    }
}
