using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using EventureMVC.Models;
using EventureMVC.Models.ViewModel;

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

        public async Task<IActionResult> Index(int activityId)
        {
            ViewData["Title"] = "Activity";

            // Define the request URI with activityId parameter
            var activityUri = $"{BaseUri}api/Activity/getActivityById/{activityId}";
           
            var commentsUri = $"{BaseUri}api/Comment/getAllCommentsByActivity/{activityId}";
              
            try
            {
                // Fetch Activity details
                var activityResponse = await _httpClient.GetAsync(activityUri);

                if (!activityResponse.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, "Failed to retrieve activity data.");
                    return View(null);
                }
                var activityJson = await activityResponse.Content.ReadAsStringAsync();
                var activity = JsonConvert.DeserializeObject<ActivityViewModel>(activityJson);

                // Fetch Comments related to the activity
                var commentsResponse = await _httpClient.GetAsync(commentsUri);
                if (!commentsResponse.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, "Failed to retrieve comments.");
                    return View(activity); // Return activity even if comments fail
                }
                var commentsJson = await commentsResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"API Response JSON: {commentsJson}"); // Loggar hela responsen för kontroll

                activity.Comments = JsonConvert.DeserializeObject<List<CommentViewModel>>(commentsJson);

                return View(activity);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An error occurred. Please try again later.");
                return View(null);
            }
        }


    }
}
