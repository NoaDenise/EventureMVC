using Microsoft.AspNetCore.Mvc;
using System.Text;
using EventureMVC.Models;
using System.Text.Json;
namespace EventureMVC.Controllers
{
    public class UserController : Controller
    {

        private readonly HttpClient _httpClient;
        private readonly string _baseUri = "https://localhost:7277/";

        public UserController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public IActionResult Index()
        {
            return View();
        }


        // GET: User/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: User/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginUserViewModel login)
        {

            var jsonContent = new StringContent(JsonSerializer.Serialize(login), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUri}/login", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var token = JsonDocument.Parse(responseData).RootElement.GetProperty("Token").GetString();

                // Store the token and user data in session or cookies
                HttpContext.Session.SetString("AuthToken", token);

                // Check if the user is an admin or a regular user
                var userRole = await GetUserRole(token); // Custom method for role retrieval based on token

                if (userRole == "Admin")
                {
                    return RedirectToAction("Dashboard", "Admin");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(login);
            }
        }

        //Det behövs inte
        //Hur ska jag bestämma att hämta rolle (användare att admin eller user)
        private async Task<string> GetUserRole(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"{_baseUri}api/User/getAllUsers");  // or a role-specific endpoint
            if (response.IsSuccessStatusCode)
            {
                var roleResponse = await response.Content.ReadAsStringAsync();
                return JsonDocument.Parse(roleResponse).RootElement.GetProperty("Role").GetString();
            }
            return null;
        }
        [HttpPost]
        public IActionResult GuestLogin()
        {
            // Set up a guest session without authentication, or redirect to a specific guest-only page
            HttpContext.Session.SetString("UserRole", "Guest");
            return RedirectToAction("GuestHome", "Home"); // Redirect to a guest-friendly page
        }
    }
}




