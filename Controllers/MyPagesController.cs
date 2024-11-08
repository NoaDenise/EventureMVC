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
            ViewData["Title"] = "My Pages"; // Sätter rubriken för vyn
            return View("~/Views/MyPagesUser/Index.cshtml"); // Återger vyn för My Pages
        }

        public async Task <IActionResult> MyInformation(string userId = "1d19e442-0828-4e8f-8b37-0752b62ab4a8")
        {
            var response = await _httpClient.GetAsync($"{_baseUri}/api/User/getUserById/{userId}");

            var json = await response.Content.ReadAsStringAsync();

            var myInformation = JsonConvert.DeserializeObject<MyInformationViewModel>(json);

            return View("~/Views/MyPagesUser/MyInformation.cshtml", myInformation);
        }

        public IActionResult SavedActivities()
        {
            ViewData["Title"] = "Saved Activities";
            return View(); 
        }

        public IActionResult ActivitySignUps()
        {
            ViewData["Title"] = "Activity Sign-Ups";
            return View(); 
        }

        public IActionResult EditDeleteCreatedActivities()
        {
            ViewData["Title"] = "Edit/Delete Created Activities";
            return View(); 
        }
    }
}
