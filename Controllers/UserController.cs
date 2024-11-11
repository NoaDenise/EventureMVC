using Microsoft.AspNetCore.Mvc;
using EventureMVC.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;
using EventureMVC.Models.ViewModel;
using System.IdentityModel.Tokens.Jwt;

namespace EventureMVC.Controllers
{
    public class UserController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUri = "https://localhost:7277/";

        public UserController(HttpClient httpClient)
        {
            _httpClient = httpClient; // Injecting the HTTP client dependency
        }

        // GET: User/Login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Models.ViewModel.LoginUserViewModel login)
        {

            // Send login credentials to the API in JSON format
            var jsonContent = new StringContent(JsonSerializer.Serialize(login), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUri}api/User/login", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                // Read the response from the API
                var responseData = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseData); // Debug output of the response
                var token = JsonDocument.Parse(responseData).RootElement.GetProperty("token").GetString(); // Extract the token from the response

                if (string.IsNullOrEmpty(token))
                {
                    // If the token is empty or null, login failed
                    ModelState.AddModelError("", "Login failed. Token is missing.");
                    return View(login);
                }

                // Parse the token to extract claims (including 'nameid')
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;

                if (!string.IsNullOrEmpty(userId))
                {
                    // Store userId in session
                    HttpContext.Session.SetString("nameid", userId);
                }
                else
                {
                    ModelState.AddModelError("", "Failed to retrieve User ID from token.");
                    return View(login);
                }

                // Set the token cookie with appropriate expiration based on Remember Me
                HttpContext.Response.Cookies.Append("jwtToken", token, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    HttpOnly = true, // Prevent client-side access to the cookie (security measure)
                    Secure = true, // Only for HTTPS
                    SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict, // Restrict cross-site cookie usage
                    Expires = login.RememberMe ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddHours(1) // If Remember Me is checked, cookie lasts 7 days, otherwise 1 hour
                });


                // Retrieve the user's role after successful login
                var userRole = await GetUserRole(token);

                if (userRole == "admin")

                {    // If the user is an admin, redirect to the admin page
                    return RedirectToAction("index", "Admin");
                }
                else if (userRole == "user")
                {
                    // If the user is a regular user, redirect to the Explore page
                    return RedirectToAction("index", "Explore");
                }
            }
            else
            {
                // If the login attempt fails, add an error to the ModelState
                ModelState.AddModelError("", "Invalid login attempt.");
            }
            // If login fails, return the login view with the model to display errors
            return View(login);
        }



        // Method to get the user's role using the token
        private async Task<string> GetUserRole(string token)
        {
            // Send a request to the API to get the user's role, passing the JWT token in the Authorization header
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{_baseUri}api/User/getRole");
            requestMessage.Headers.Add("Authorization", "Bearer " + token);

            var response = await _httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                // Parse the response to get the role of the user
                var responseData = await response.Content.ReadAsStringAsync();
                var userRole = JsonDocument.Parse(responseData).RootElement.GetProperty("role").GetString();
                return userRole;  // Return the role (admin or user)
            }

            return string.Empty;
        }

        // Method to check if the user is an admin (not currently used in the login flow, but could be useful)
        private async Task<bool> IsAdmin(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"{_baseUri}api/User/role?token={{token}}\"");

            if (response.IsSuccessStatusCode)
            {
                // If the response is successful, parse the role data
                var roleData = await response.Content.ReadAsStringAsync();
                var role = JsonDocument.Parse(roleData).RootElement.GetProperty("Role").GetString();
                return role == "Admin"; // Kontrollera om användaren har rollen "Admin"
            }
            // If the request fails, return false
            return false;
        }


        // POST: User/GuestLogin
        [HttpPost]
        public IActionResult GuestLogin()
        {            
            return RedirectToAction("Index", "Explore"); // Redirect to a guest-friendly page
        }


        //GET: User/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            // Delete the JWT token from cookies when the user logs out
            HttpContext.Response.Cookies.Delete("jwtToken");

            //Delete the session
            HttpContext.Session.Clear();

            // Redirect the user back to the login page
            return RedirectToAction("Login", "User");
        }

    }
}




