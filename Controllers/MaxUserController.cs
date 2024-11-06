using EventureMVC.Models.ViewModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using EventureMVC.Models;
using System.Net.Http;

namespace EventureMVC.Controllers
{
    public class MaxUserController : Controller
    {
        private readonly HttpClient _httpClient;
        private string baseUrl = "https://localhost:7277/"; //Lägga den här i .env kanske?
        public MaxUserController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task <IActionResult> Register(RegisterViewModel model) 
        {
            if (!ModelState.IsValid)
            {
                return View(model);               
            }

            var response = await _httpClient.PostAsJsonAsync($"{baseUrl}api/User/register", model);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(model);
            
        }

        // Logout the user by clearing the cookies and then returns user to homepage
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Response.Cookies.Delete("jwtToken");

            return RedirectToAction("Index", "Home");
        }

        // Trying to create the Category connection when creating user

        [HttpGet]
        public async Task<IActionResult> AddPreferences()
        {
            var response = await _httpClient.GetAsync($"{baseUrl}api/Category/getAllCategories");

            if (!response.IsSuccessStatusCode)
            {
                return View("Error");
            }

            var categories = await response.Content.ReadFromJsonAsync<List<Category>>();
            var viewModel = new PreferencesViewModel
            {
                Categories = categories ?? new List<Category>(),
                SelectedCategoryIds = new List<int>()
            };

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> AddPreferences(PreferencesViewModel model)
        {
            // Retrieve user ID from the cookie
            if (!Request.Cookies.TryGetValue("UserId", out var userId) || string.IsNullOrEmpty(userId))
            {
                return Unauthorized(); // No user ID found, return Unauthorized
            }

            // Loop through selected categories and make a request for each one
            foreach (var categoryId in model.SelectedCategoryIds)
            {
                var response = await _httpClient.PostAsync($"{baseUrl}/api/User/{userId}/categories/{categoryId}", null);

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, "Unable to save preferences for one or more categories. Please try again.");
                    return View(model);
                }
            }

            return RedirectToAction("Index", "Home");

        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(string categoryName, string categoryDescription)
        {
            if (string.IsNullOrEmpty(categoryName))
            {
                ModelState.AddModelError(string.Empty, "Category name is required.");
                return RedirectToAction(nameof(AddPreferences)); // Stay on the same page if validation fails
            }

            var data = new
            {
                categoryName = categoryName,
                categoryDescription = categoryDescription
            };

            var response = await _httpClient.PostAsJsonAsync($"{baseUrl}api/Category/addCategory", data);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Failed to create category.");
            }

            return RedirectToAction(nameof(AddPreferences));
        }
    }
}
