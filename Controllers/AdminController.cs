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

        public AdminController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public IActionResult Index()
        {
            ViewData["Title"] = "Admin Pages";

            return View();
        }

        public async Task<IActionResult> ListMyInformation(int userId)
        {
            ViewData["Title"] = "My Information";

            var response = await _httpClient.GetAsync($"{baseUri}/api/User/getUserById/{userId}");

            var json = await response.Content.ReadAsStringAsync();

            var myInformation = JsonConvert.DeserializeObject<User>(json);

            return View();
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

            var json = await response.Content.ReadAsStringAsync();

            var activities = JsonConvert.DeserializeObject<List<Activity>>(json);

            return View(activities);
        }


        public async Task<IActionResult> ApproveActivity(int id)
        {
            var response = await _httpClient.GetAsync($"{baseUri}/api/Activity/getActivityById/{id}");

            var json = await response.Content.ReadAsStringAsync();

            var activityToApprove = JsonConvert.DeserializeObject<Activity>(json);

            return View(activityToApprove);
        }


        [HttpPost]
        //putasync takes two params, so had to add null
        public async Task<IActionResult> ApproveActivity(Activity activity)
        {
            var response = await _httpClient.PutAsync($"{baseUri}/api/Activity/approveActivity/{activity.ActivityId}", null);

            //if (response.IsSuccessStatusCode)
            //{
            return RedirectToAction("ListActivitiesToApprove");

            //}
        }

        //[HttpPost]
        ////putasync takes two params, so had to add null
        //public async Task<IActionResult> ApproveActivity(int activityId)
        //{
        //    var response = await _httpClient.PutAsync($"{baseUri}/api/Activity/approveActivity/{activityId}", null);

        //    //if (response.IsSuccessStatusCode)
        //    //{
        //        return RedirectToAction("ListActivitiesToApprove");

        //    //}
        //}


        public async Task<IActionResult> ListAllActivities()
        {
            ViewData["Title"] = "All Activities";

            var response = await _httpClient.GetAsync($"{baseUri}/api/Activity/getAllActivities");

            var json = await response.Content.ReadAsStringAsync();

            var activities = JsonConvert.DeserializeObject<List<Activity>>(json);

            return View(activities);
        }

        public async Task<IActionResult> EditActivity(int id)
        {
            var response = await _httpClient.GetAsync($"{baseUri}/api/Activity/getActivityById/{id}");

            var json = await response.Content.ReadAsStringAsync();

            var activity = JsonConvert.DeserializeObject<Activity>(json);



            return View(activity);
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
                    ActivityId = activity.ActivityId, // Se till att detta sätts
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


            var json = JsonConvert.SerializeObject(activity);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{baseUri}/api/Activity/editActivity/{activity.ActivityId}", content);

            return RedirectToAction("ListAllActivities");
        }


        [HttpPost]
        public async Task<IActionResult> DeleteActivity(int id)
        {
            var response = await _httpClient.DeleteAsync($"{baseUri}/api/Activity/deleteActivity/{id}");

            return RedirectToAction("ListAllActivities");
        }

        public async Task<IActionResult> ListAllCategories()
        {
            ViewData["Title"] = "All Categories";

            var response = await _httpClient.GetAsync($"{baseUri}/api/Category/getAllCategories");

            var json = await response.Content.ReadAsStringAsync();

            var categories = JsonConvert.DeserializeObject<List<Category>>(json);

            return View(categories);
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
                return View(category);
            }

            var json = JsonConvert.SerializeObject(category);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{baseUri}/api/Category/addCategory", content);

            return RedirectToAction("ListAllCategories");
        }


        //MUST BE FIXED
        public async Task<IActionResult> EditCategory(int id)
        {
            ViewData["Title"] = "Edit Category";

            var response = await _httpClient.GetAsync($"{baseUri}/api/Category/getCategoryById/{id}");

            var json = await response.Content.ReadAsStringAsync();

            var category = JsonConvert.DeserializeObject<Category>(json);

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> EditCategory(Category category)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View(category);
            //}


            if (ModelState.IsValid)
            {
                var updatedCategory = new Category
                {
                    CategoryId = category.CategoryId,
                    CategoryName = category.CategoryName,
                    CategoryDescription = category.CategoryDescription
                };

            }

            var json = JsonConvert.SerializeObject(category);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{baseUri}/api/Category/editCategory/{category.CategoryId}", content);

            return RedirectToAction("ListAllCategories");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var response = await _httpClient.DeleteAsync($"{baseUri}/api/Category/deleteCategory?categoryId=/{id}");

            return RedirectToAction("ListAllCategories");
        }
    }
}
