using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using EventureMVC.Models;
namespace EventureMVC.Controllers
{
    public class CommentController : Controller
    {
        private readonly HttpClient _httpClient;
        private string _baseUri = "https://localhost:7277/";

        public CommentController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IActionResult> Index(int activityId=3 )
        {
            ViewData["activityId"] = activityId;

            // Log API URI and activityId
            var apiUrl = $"{_baseUri}api/Comment/getAllCommentsByActivity/{activityId}";

            var response = await _httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"API Response Status Code: {response.StatusCode}");
                ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                return View(new List<CommentShowDTO>());
            }

            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine("API Response JSON: " + json);

            try
            {
                var commentList = JsonConvert.DeserializeObject<List<CommentShowDTO>>(json);
                Console.WriteLine("Deserialized Comment List Count: " + (commentList?.Count ?? 0));

                return View(commentList);
            }
            catch (JsonException ex)
            {
                Console.WriteLine("Deserialization Error: " + ex.Message);
                ModelState.AddModelError(string.Empty, "Error processing data. Please try again later.");
                return View(new List<CommentShowDTO>());
            }
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CommentCreateEditDTO());
        }
        [HttpPost]
        public async Task<IActionResult> Create(CommentCreateEditDTO newComment)
        {
            if (!ModelState.IsValid)
            {
                return View(newComment);
            }

            var token = HttpContext.Request.Cookies["jwtToken"];
            if (string.IsNullOrEmpty(token))
            {
                ModelState.AddModelError(string.Empty, "Authentication token is missing. Please log in.");
                return View(newComment);
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var apiUrl = $"{_baseUri}api/Comment/addComment";

            var response = await _httpClient.PostAsJsonAsync(apiUrl, newComment);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Failed to add comment. Please try again.");
                return View(newComment);
            }

            return RedirectToAction("Index", new { activityId = newComment.ActivityId });
        }
        [HttpGet]
        public async Task<IActionResult> Details(int commentId)
        {

            var apiUrl = $"{_baseUri}api/Comment/getCommentById/{commentId}";

            var response = await _httpClient.GetAsync(apiUrl);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Error retrieving comment details.");
                return RedirectToAction("Index");
            }
            var json = await response.Content.ReadAsStringAsync(); 
            var comment = JsonConvert.DeserializeObject<CommentShowDTO>(json); 

        
            return View(comment);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int commentId)
        {
            var apiUrl = $"{_baseUri}api/Comment/getCommentById/{commentId}";
            var response = await _httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Error loading comment data.");
                return RedirectToAction("Index");
            }
            var json = await response.Content.ReadAsStringAsync();
            var comment = JsonConvert.DeserializeObject<CommentShowDTO>(json);


            return View(comment);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int commentId, CommentCreateEditDTO updatedComment)
        {
            var apiUrl = $"{_baseUri}api/Comment/editComment/{commentId}";

            var response = await _httpClient.PutAsJsonAsync(apiUrl, updatedComment);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Failed to update comment.");
                return View(updatedComment);
            }

            return RedirectToAction("Index", new { activityId = updatedComment.ActivityId });
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int commentId)
        {
            var apiUrl = $"{_baseUri}api/Comment/deleteComment/{commentId}";

            var response = await _httpClient.DeleteAsync(apiUrl);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Failed to delete comment.");
            }

            return RedirectToAction("Index");
        }

    }
}
