using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using EventureMVC.Models;

namespace EventureMVC.Controllers
{
    public class ActivityController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string BaseUri = "https://localhost:7277/";

        public ActivityController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IActionResult> Index(int activityId = 3)
        {
            ViewData["Title"] = "Activity";

            // Define the request URI with activityId parameter
            var requestUri = $"{BaseUri}api/Activity/getActivityById/{activityId}";

            try
            {
                // Send request to API
                var response = await _httpClient.GetAsync(requestUri);

                // Check if the response is successful
                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, "Failed to retrieve data. Please contact the administrator.");
                    return View(null);
                }

                // Read response content as string
                var json = await response.Content.ReadAsStringAsync();

                // Deserialize JSON to a single ActivityViewModel object
                var activity = JsonConvert.DeserializeObject<ActivityViewModel>(json);

                return View(activity);
            }
            catch (HttpRequestException)
            {
                // Handle request errors
                ModelState.AddModelError(string.Empty, "Server error. Please try again later.");
                return View(null);
            }
            catch (JsonException)
            {
                // Handle deserialization errors
                ModelState.AddModelError(string.Empty, "Error processing data. Please try again later.");
                return View(null);
            }
        }


    }
}
