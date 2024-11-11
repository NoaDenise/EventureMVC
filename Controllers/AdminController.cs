using EventureMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;

namespace EventureMVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly HttpClient _httpClient;
        private string baseUri = "https://localhost:7277";
        private readonly ILogger<AdminController> _logger;

        public AdminController(HttpClient httpClient, ILogger<AdminController> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        public IActionResult Index()
        {
            ViewData["Title"] = "Admin Pages";

            return View();
        }

        public async Task<IActionResult> ListMyInformation(string userId)
        {

            //USE THIS??? <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<----------------
            //if (!User.IsInRole("Admin"))
            //{
            //    return Forbid("You are not authorized to perform this action.");
            //}

            ViewData["Title"] = "My Information";

            var response = await _httpClient.GetAsync($"{baseUri}/api/User/getUserById/{userId}");

            if (!response.IsSuccessStatusCode)
            {

                //SHOULD WE SEND TO ERROR PAGE OR JUST REDIRECT TO FUNCTIONING SITE?
                //Console.WriteLine($"Error: {response.StatusCode}");
                //return View("Error");

                TempData["ErrorMessage"] = "Unable to list your information. Please, try again later.";
                return RedirectToAction("Index");


            }

            var json = await response.Content.ReadAsStringAsync();

            try
            {
                var myInformation = JsonConvert.DeserializeObject<User>(json);
                return View();

            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize JSON:{JsonContent}", json);
                TempData["ErrorMessage"] = "Unable to list activities to approve. Please, try again later.";
                return RedirectToAction("Index");
            }

        }


        public IActionResult EditMyInformation()
        {
            ViewData["Title"] = "Edit Information";

            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> EditMyInformation()
        //{


        //    return RedirectToAction("AdminInformation");
        //}

        public async Task<IActionResult> ListActivitiesToApprove()
        {
            ViewData["Title"] = "Activites Awaiting Approval";

            //in Swagger and here we have to fill in true in order to list NOT approved activities
            var response = await _httpClient.GetAsync($"{baseUri}/api/Activity/awaitingApproval?isApproved=true");

            if (!response.IsSuccessStatusCode)
            {
                //Console.WriteLine($"Error: {response.StatusCode}");
                //return View("Error");

                TempData["ErrorMessage"] = "Unable to list activities to approve. Please, try again later.";
                return RedirectToAction("Index");
            }

            var json = await response.Content.ReadAsStringAsync();

            try
            {
                var activities = JsonConvert.DeserializeObject<List<Activity>>(json);
                return View(activities);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize JSON:{JsonContent}", json);
                TempData["ErrorMessage"] = "Unable to list activities to approve. Please, try again later.";
                return RedirectToAction("Index");
            }
        }


        public async Task<IActionResult> ApproveActivity(int id)
        {

            var response = await _httpClient.GetAsync($"{baseUri}/api/Activity/getActivityById/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Could not find activity. Please, try again later.";
                return RedirectToAction("ListActivitiesToApprove");
            }

            var json = await response.Content.ReadAsStringAsync();



            try
            {
                var activityToApprove = JsonConvert.DeserializeObject<Activity>(json);

                return View(activityToApprove);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize JSON:{JsonContent}", json);
                TempData["ErrorMessage"] = "Unable to list activities to approve. Please, try again later.";
                return RedirectToAction("Index");
            }

        }


        [HttpPost]
        //putasync takes two params, so had to add null
        public async Task<IActionResult> ApproveActivity(Activity activity)
        {

            try
            {
                var response = await _httpClient.PutAsync($"{baseUri}/api/Activity/approveActivity/{activity.ActivityId}", null);

                return RedirectToAction("ListActivitiesToApprove");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unable to approve activity. Please, try again later.";
                return RedirectToAction("ListActivitiesToApprove");
            }

        }


        public async Task<IActionResult> ListAllActivities()
        {
            ViewData["Title"] = "All Activities";

            var response = await _httpClient.GetAsync($"{baseUri}/api/Activity/getAllActivities");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Unable to list all activities. Please, try again later.";
                return RedirectToAction("Index");
            }

            var json = await response.Content.ReadAsStringAsync();

            try
            {
                var activities = JsonConvert.DeserializeObject<List<Activity>>(json);

                return View(activities);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize JSON:{JsonContent}", json);
                TempData["ErrorMessage"] = "Unable to list all activities. Please, try again later.";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> EditActivity(int id)
        {
            //if (id <= 0)
            //{
            //    return BadRequest("Invalid Activity ID.");//TEST, bugged? for int params
            //}


            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data received.");
            }

            var response = await _httpClient.GetAsync($"{baseUri}/api/Activity/getActivityById/{id}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error: {response.StatusCode}");
                TempData["ErrorMessage"] = "Unable to edit activity. Please, try again later.";
                return RedirectToAction("ListAllActivities");
            }

            var json = await response.Content.ReadAsStringAsync();

            try
            {
                var activity = JsonConvert.DeserializeObject<Activity>(json);

                return View(activity);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize JSON:{JsonContent}", json);
                TempData["ErrorMessage"] = "Unable to list activities to approve. Please, try again later.";
                return RedirectToAction("ListAllActivities");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditActivity(Activity activity)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View(editActivityVM);
            //}

            if (ModelState.IsValid)
            {
                // Skapa ActivityCreateEditDTO och fyll i fälten???
                var activityToEdit = new Activity
                {
                    ActivityId = activity.ActivityId, // always set this field
                    ActivityName = activity.ActivityName,
                    ActivityDescription = activity.ActivityDescription,
                    DateOfActivity = activity.DateOfActivity,
                    ActivityLocation = activity.ActivityLocation,
                    ImageUrl = activity.ImageUrl,
                    WebsiteUrl = activity.WebsiteUrl,
                    ContactInfo = activity.ContactInfo,
                    IsFree = activity.IsFree,
                    Is18Plus = activity.Is18Plus,
                    IsFamilyFriendly = activity.IsFamilyFriendly
                };
            }

            try
            {
                var json = JsonConvert.SerializeObject(activity);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{baseUri}/api/Activity/editActivity/{activity.ActivityId}", content);

                return RedirectToAction("ListAllActivities");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unable to edit activity. Please, try again later.";
                return RedirectToAction("ListAllActivities");
            }
        }


        [HttpPost]
        public async Task<IActionResult> DeleteActivity(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data received.");
            }

            var response = await _httpClient.DeleteAsync($"{baseUri}/api/Activity/deleteActivity/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Unable to list your information. Please, try again later.";
                return RedirectToAction("ListAllActivities");
            }

            return RedirectToAction("ListAllActivities");
        }

        public async Task<IActionResult> ListAllCategories()
        {
            ViewData["Title"] = "All Categories";

            var response = await _httpClient.GetAsync($"{baseUri}/api/Category/getAllCategories");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Unable to list your information. Please, try again later.";
                return RedirectToAction("Index");
            }

            var json = await response.Content.ReadAsStringAsync();

            try
            {
                var categories = JsonConvert.DeserializeObject<List<Category>>(json);

                return View(categories);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize JSON:{JsonContent}", json);
                TempData["ErrorMessage"] = "Unable to list categories. Please, try again later.";
                return RedirectToAction("Index");
            }

        }

        public IActionResult CreateCategory()
        {
            ViewData["Title"] = "Create New Category";

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data received.");
            }

            try
            {
                var json = JsonConvert.SerializeObject(category);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{baseUri}/api/Category/addCategory", content);

                return RedirectToAction("ListAllCategories");
            }

            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unable to create category. Please, try again later.";
                return RedirectToAction("ListAllCategories");
            }
        }


        //BUGGED
        public async Task<IActionResult> EditCategory(int id)
        {
            ViewData["Title"] = "Edit Category";

            var response = await _httpClient.GetAsync($"{baseUri}/api/Category/getCategoryById/{id}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error: {response.StatusCode}");
                return RedirectToAction("ListAllCategories");
            }

            var json = await response.Content.ReadAsStringAsync();


            try
            {
                var category = JsonConvert.DeserializeObject<Category>(json);

                return View(category);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize JSON:{JsonContent}", json);
                TempData["ErrorMessage"] = "Unable to find category. Please, try again later.";
                return RedirectToAction("ListAllCategories");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditCategory(Category category)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View(category);
            //}

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data received.");
            }


            if (ModelState.IsValid)
            {
                var updatedCategory = new Category
                {
                    CategoryId = category.CategoryId,
                    CategoryName = category.CategoryName,
                    CategoryDescription = category.CategoryDescription
                };

            }
            try
            {
                var json = JsonConvert.SerializeObject(category);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{baseUri}/api/Category/editCategory/{category.CategoryId}", content);

                return RedirectToAction("ListAllCategories");
            }

            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unable to edit category. Please, try again later.";
                return RedirectToAction("ListAllCategories");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data received.");
            }

            var response = await _httpClient.DeleteAsync($"{baseUri}/api/Category/deleteCategory/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Unable to delete category. Please, try again later.";
                return RedirectToAction("ListAllCategories");
            }

            return RedirectToAction("ListAllCategories");
        }
    }
}
