using EventureMVC.Models;
using EventureMVC.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;

namespace EventureMVC.Controllers
{
    public class MyPagesController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUri = "https://localhost:7277";
        private readonly ILogger<AdminController> _logger;

        public MyPagesController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public IActionResult Index()
        {
            // Hämta användar-ID från sessionen
            string userId = HttpContext.Session.GetString("nameid");

            // Om användaren inte är inloggad, omdirigera till login
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "User");
            }

            ViewData["Title"] = "My Pages";
            return View("Index"); // Återger vyn för My Pages
        }

        public async Task<IActionResult> MyInformation()
        {
            var userId = HttpContext.Session.GetString("nameid");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "User");
            }

            ViewData["Title"] = "My Information";

            var response = await _httpClient.GetAsync($"{_baseUri}/api/User/getUserById/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Unable to list your information. Please, try again later.";
                return RedirectToAction("Index");
            }

            var json = await response.Content.ReadAsStringAsync();

            try
            {
                var userInfo = JsonConvert.DeserializeObject<MyInformationDTO>(json);

                var model = new MyInformationViewModel
                {
                    FirstName = userInfo.FirstName,
                    LastName = userInfo.LastName,
                    PhoneNumber = userInfo.PhoneNumber,
                    UserLocation = userInfo.UserLocation,
                    UserName = userInfo.UserName,
                    Email = userInfo.Email
                };

                return View(model);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize JSON:{JsonContent}", json);
                TempData["ErrorMessage"] = "Unable to list your information. Please, try again later.";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> EditMyInformation()
        {
            ViewData["Title"] = "Edit Information";

 
            var id = HttpContext.Session.GetString("nameid");

            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Login", "User");
            }


            var response = await _httpClient.GetAsync($"{_baseUri}/api/User/getUserById/{id}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error: {response.StatusCode}");
                TempData["ErrorMessage"] = "Unable to find user. Please, try again later.";
                return RedirectToAction("MyInformation");
            }

            var json = await response.Content.ReadAsStringAsync();

            try
            {
                var user = JsonConvert.DeserializeObject<MyInformationViewModel>(json);


                return View(user);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize JSON:{JsonContent}", json);
                TempData["ErrorMessage"] = "Unable to edit your information. Please, try again later.";
                return RedirectToAction("MyInformation");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditMyInformation(MyInformationViewModel myInformationViewModel)
        {

            if (!ModelState.IsValid)
            {
                return View(myInformationViewModel);
            }


            var id = HttpContext.Session.GetString("nameid");

            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Login", "User");
            }

            var apiUrl = $"{_baseUri}/api/User/editUser/{id}";

            var response = await _httpClient.PutAsJsonAsync(apiUrl, myInformationViewModel);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Information successfully updated!";
            }
            else
            {
                TempData["ErrorMessage"] = "Unable to update information.";
            }

            return RedirectToAction("MyInformation");

        }

        public async Task<IActionResult> EditUserPassword()
        {
            ViewData["Title"] = "Edit Password";

            var id = HttpContext.Session.GetString("nameid");

            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Login", "User");
            }


            var response = await _httpClient.GetAsync($"{_baseUri}/api/User/getUserById/{id}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error: {response.StatusCode}");
                TempData["ErrorMessage"] = "Unable to find user. Please, try again later.";
                return RedirectToAction("MyInformation");
            }

            var json = await response.Content.ReadAsStringAsync();

            try
            {
                var user = JsonConvert.DeserializeObject<UserPasswordViewModel>(json);

                return View(user);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize JSON:{JsonContent}", json);
                TempData["ErrorMessage"] = "Unable to edit password. Please, try again later.";
                return RedirectToAction("MyInformation");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditUserPassword(UserPasswordViewModel userPasswordViewModel)
        {

            if (!ModelState.IsValid)
            {
                return View(userPasswordViewModel);
            }


            var id = HttpContext.Session.GetString("nameid");

            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Login", "User");
            }

            var apiUrl = $"{_baseUri}/api/User/editAdminPassword/{id}";

            var response = await _httpClient.PutAsJsonAsync(apiUrl, userPasswordViewModel);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Password successfully updated!";
            }
            else
            {
                TempData["ErrorMessage"] = "Unable to update password.";
            }

            return RedirectToAction("MyInformation");

        }

        public async Task<IActionResult> SavedActivities()
        {
            ViewData["Title"] = "Saved Activities";

            // Hämta användar-ID från sessionen
            string userId = HttpContext.Session.GetString("nameid");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "User");
            }

            // Anropa API:et för att hämta sparade aktiviteter
            var response = await _httpClient.GetAsync($"{_baseUri}/api/User/getAllUserEvents?userId={userId}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var savedActivities = JsonConvert.DeserializeObject<List<SavedActivitiesUserDTO>>(json);
                return View("SavedActivities", savedActivities);
            }
            else
            {
                return View("SavedActivities", new List<SavedActivitiesUserDTO>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSavedActivity(int userEventId)
        {
            string userId = HttpContext.Session.GetString("nameid");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "User");
            }

            // Anropa API:et för att radera den sparade aktiviteten
            var response = await _httpClient.DeleteAsync($"{_baseUri}/api/User/deleteUserEvent?userEventId={userEventId}&userId={userId}");

            if (response.IsSuccessStatusCode)
            {
                var updatedResponse = await _httpClient.GetAsync($"{_baseUri}/api/User/getAllUserEvents?userId={userId}");

                if (updatedResponse.IsSuccessStatusCode)
                {
                    var json = await updatedResponse.Content.ReadAsStringAsync();
                    var savedActivities = JsonConvert.DeserializeObject<List<SavedActivitiesUserDTO>>(json);
                    return View("SavedActivities", savedActivities);
                }
            }
            else
            {
                ViewData["ErrorMessage"] = "Error occurred while deleting the activity.";
                return View("SavedActivities");
            }

            return RedirectToAction("SavedActivities");
        }

        public async Task<IActionResult> ActivitySignUps()
        {
            string userId = HttpContext.Session.GetString("nameid");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "User");
            }

            var response = await _httpClient.GetAsync($"{_baseUri}/api/Attendance/getUsersAttendance/{userId}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var activitySignUps = JsonConvert.DeserializeObject<List<UserAttendanceDTO>>(json);
                return View("ActivitySignUps", activitySignUps);
            }
            else
            {
                return View("ActivitySignUps", new List<UserAttendanceDTO>());
            }
        }

        // Method to delete a user's activity sign-up
        [HttpPost]
        public async Task<IActionResult> DeleteActivitySignup(int attendanceId)
        {
            string userId = HttpContext.Session.GetString("nameid");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "User");
            }

            // Anropa API för att radera användarens sign-up, nu med både attendanceId och userId
            var response = await _httpClient.DeleteAsync($"{_baseUri}/api/Attendance/deleteAttendance/{attendanceId}");


            if (response.IsSuccessStatusCode)
            {
                var updatedResponse = await _httpClient.GetAsync($"{_baseUri}/api/Attendance/getUsersAttendance/{userId}");

                if (updatedResponse.IsSuccessStatusCode)
                {
                    var json = await updatedResponse.Content.ReadAsStringAsync();
                    var activitySignUps = JsonConvert.DeserializeObject<List<UserAttendanceDTO>>(json);
                    return View("ActivitySignUps", activitySignUps); // Återvänd till vyn med uppdaterad lista
                }
            }
            else
            {
                ViewData["ErrorMessage"] = "Error occurred while deleting the activity sign-up.";
                return View("ActivitySignUps");
            }

            return RedirectToAction("ActivitySignUps");
        }

        public IActionResult DeleteCreatedActivities()
        {
            // Hämta användar-ID från sessionen
            string userId = HttpContext.Session.GetString("nameid");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "User");
            }

            ViewData["Title"] = "Delete Created Activities";
            return View();
        }
    }
}
