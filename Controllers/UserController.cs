using Microsoft.AspNetCore.Mvc;
using System.Text;
using EventureMVC.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using System.Reflection;

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

            var response = await _httpClient.PostAsJsonAsync($"{_baseUri}api/User/login", login);

            if (!response.IsSuccessStatusCode)
            {
                return View(login);
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<TokenResponse>(jsonResponse);
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token.Token);
            var claims = jwtToken.Claims.ToList();
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Extract UserId from the "nameid" claim (assuming it's present)
            var userIdClaim = claims.FirstOrDefault(c => c.Type == "nameid");
            if (userIdClaim != null)
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userIdClaim.Value));
                HttpContext.Session.SetString("nameid", userIdClaim.Value); // Storing in session
            }

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = jwtToken.ValidTo
            });

            HttpContext.Response.Cookies.Append("jwtToken", token.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = jwtToken.ValidTo
            });

            return RedirectToAction("Index", "Home");
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




