using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using EventureMVC.Models;
using EventureMVC.Models.ViewModel;
using System.Text;

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
             

            //need this for fetching liked activity
            var userId = HttpContext.Session.GetString("nameid");

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

                if (!string.IsNullOrEmpty(userId))
                {
                    var likedActivitiesResponse = await _httpClient.GetAsync($"{BaseUri}api/User/likedActivities/{userId}");
                    if (likedActivitiesResponse.IsSuccessStatusCode)
                    {
                        var likedJson = await likedActivitiesResponse.Content.ReadAsStringAsync();
                        activity.LikedActivities = JsonConvert.DeserializeObject<List<int>>(likedJson);
                    }
                }
                else
                {
                    activity.LikedActivities = new List<int>();
                }


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

        //Toggles the like between like and unlike
        [HttpPost]
        public async Task<IActionResult> LikeActivity(int activityId)
        {
            var userId = HttpContext.Session.GetString("nameid");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "User");
            }

            var likedActivitiesResponse = await _httpClient.GetAsync($"{BaseUri}api/User/likedActivities/{userId}");
            List<int> likedActivities = new List<int>();

            if (likedActivitiesResponse.IsSuccessStatusCode)
            {
                var likedJson = await likedActivitiesResponse.Content.ReadAsStringAsync();
                likedActivities = JsonConvert.DeserializeObject<List<int>>(likedJson);
            }

            bool isLiked = likedActivities.Contains(activityId);
            var toggleLikeUrl = $"{BaseUri}api/User/toggleLike/{userId}/{activityId}/{!isLiked}";

            var response = await _httpClient.PostAsJsonAsync(toggleLikeUrl, new { UserId = userId, ActivityId = activityId });

            if (response.IsSuccessStatusCode)
            {
                likedActivitiesResponse = await _httpClient.GetAsync($"{BaseUri}api/User/likedActivities/{userId}");

                if (likedActivitiesResponse.IsSuccessStatusCode)
                {
                    var likedJson = await likedActivitiesResponse.Content.ReadAsStringAsync();
                    likedActivities = JsonConvert.DeserializeObject<List<int>>(likedJson);
                }

                // Pass liked activities list to the view model when redirecting back to the Index action
                return RedirectToAction("Index", new { likedActivities });
            }
            else
            {
                TempData["Error"] = "Failed to toggle the like status.";
                return RedirectToAction("Index");
            }
        }

        //Imported from old SecondActivity Controller
        public IActionResult AddActivity()
        {
            ViewData["Title"] = "Create Activity";

            var userId = HttpContext.Session.GetString("nameid");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "User");
            }

            var activity = new Activity
            {
                UserId = userId
            };

            return View(activity);
        }

        [HttpPost]
        public async Task<IActionResult> AddActivity(Activity activity)
        {
            if (!ModelState.IsValid)
            {
                return View(activity);
            }

            var userId = HttpContext.Session.GetString("nameid");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "User");
            }


            var json = JsonConvert.SerializeObject(activity);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{BaseUri}api/Activity/addActivity", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Explore"); // Or use RedirectToAction("Details", "Activity", new { id = activity.Id }); Kanske ska länka där man ser listan för alla man skapat?
                //eller ändra i backend så man får ut ActivityId i svaret när man har skapat en activity.
            }

            return View(activity);
        }

    }
}
