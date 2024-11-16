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
            string userId = HttpContext.Session.GetString("nameid");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "User");
            }

            var response = await _httpClient.GetAsync($"{_baseUri}/api/User/getUserById/{userId}");
            var json = await response.Content.ReadAsStringAsync();
            var myInformation = JsonConvert.DeserializeObject<MyInformationViewModel>(json);

            return View(myInformation);
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
