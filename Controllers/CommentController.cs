using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using EventureMVC.Models;
using EventureMVC.Models.ViewModel;
using System.Collections.Generic;
using System.Text;


namespace EventureMVC.Controllers
{
    public class CommentController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CommentController> _logger;
        private string _baseUri = "https://localhost:7277/";

        public CommentController(IHttpClientFactory httpClientFactory, ILogger<CommentController> logger)
        {
            _httpClient = httpClientFactory.CreateClient("APIClient");

            _logger = logger;
        }
        public async Task<IActionResult> Index(int activityId)
        {
            // Log and ensure that the activityId is correct
            if (activityId == 0)
            {
                ModelState.AddModelError(string.Empty, "Invalid activity ID.");
                return View(new List<CommentViewModel>());
            }

            Console.WriteLine($"Loading comments for ActivityId: {activityId}");
            ViewData["activityId"] = activityId;

            var apiUrl = $"{_baseUri}api/Comment/getAllCommentsByActivity/{activityId}";
            var response = await _httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                return View(new List<CommentViewModel>());
            }

            var json = await response.Content.ReadAsStringAsync();
            var commentList = JsonConvert.DeserializeObject<List<CommentViewModel>>(json);
            //foreach (var comment in commentList)
            //{
            //    //Console.WriteLine($"Deserialized Comment ID: {comment.CommentId}, Text: {comment.CommentText}");
            //}
            return View(commentList);
        }

        [HttpGet]
        public IActionResult Create(int activityId)
        {
            var newComment = new CommentViewModel
            {
                ActivityId = activityId
            };
            var token = HttpContext.Request.Cookies["jwtToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "User", new { returnUrl = $"/Comment/Create?activityId={activityId}" });
            }
            else
            {
                return View(newComment);
            }         
            return View(/*new CommentViewModel*/ ); /*{ ActivityId = activityId }*/

        }
        [HttpPost]
        public async Task<IActionResult> Create(CommentViewModel newComment)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View(newComment);
            //}
            string userId = HttpContext.Session.GetString("nameid");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "User");
            }

            newComment.UserId = userId;

            var apiUrl = $"{_baseUri}api/Comment/addComment";

            var token = HttpContext.Request.Cookies["jwtToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            var response = await _httpClient.PostAsJsonAsync(apiUrl, newComment);
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response Status: {response.StatusCode}");
            Console.WriteLine($"Response Content: {responseContent}");
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Failed to add comment. Please try again.");
                return View(newComment);
            }

            return RedirectToAction("Index", new { activityId = newComment.ActivityId });
        }


        // GET: Edit - Edit an Existing Comment
        [HttpGet]
        public async Task<IActionResult> Edit(int commentId)
        {
            var apiUrl = $"{_baseUri}api/Comment/getCommentById/{commentId}";
            var response = await _httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Failed to retrieve comment for editing.");
                return RedirectToAction("Index");
            }

            var json = await response.Content.ReadAsStringAsync();
            var comment = JsonConvert.DeserializeObject<CommentViewModel>(json);

            return View(comment);
        }

        // POST: Edit - Save Updated Comment
        [HttpPost]
        public async Task<IActionResult> Edit(int commentId, CommentViewModel updatedComment)
        {
            var apiUrl = $"{_baseUri}api/Comment/editComment/{commentId}";

            var jsonContent = JsonConvert.SerializeObject(updatedComment);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(apiUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Failed to update comment.");
                return View("Edit", updatedComment);
            }
            //return RedirectToAction("Index", "Comment", new { activityId = updatedComment.ActivityId });       
            return RedirectToAction("Index", new { activityId = updatedComment.ActivityId });

        }

        [HttpPost]
        public async Task<IActionResult> Delete(int commentId, int activityId)
        {
            var apiUrl = $"{_baseUri}api/Comment/deleteComment/{commentId}";
            var response = await _httpClient.DeleteAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Comment", new { activityId = activityId }); 
            }
            else
            {
                
                ModelState.AddModelError(string.Empty, "Failed to delete comment.");
                return RedirectToAction("Index", "Comment", new { activityId = activityId });
            }
        }
    }
}




    
