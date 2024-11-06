using EventureMVC.Models;
using EventureMVC.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EventureMVC.Controllers
{
    public class ExploreController : Controller
    {
        private readonly HttpClient _client;
        private string baseUrl = "https://localhost:7277/";
        private string countriesData = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "resources", "countries.json");
      
        public ExploreController(HttpClient client)
        {
            _client = client;
        }

        public async Task <IActionResult> Index(
            bool? isFree = null,
            bool? is18Plus = null,
            bool? isFamilyFriendly = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string location = null)
        {
            //Make a query with all the bools and inputs
            var queryParameters = new List<string>();

            if (isFree.HasValue)
            {
                queryParameters.Add($"isFree={isFree.Value}");
            }
            if (is18Plus.HasValue)
            {
                queryParameters.Add($"is18Plus={is18Plus.Value}");
            }
            if (isFamilyFriendly.HasValue)
            {
                queryParameters.Add($"isFamilyFriendly={isFamilyFriendly.Value}");
            }
            if (startDate.HasValue)
            {
                queryParameters.Add($"startDate={startDate.Value:yyyy-MM-dd}");
            }
            if (endDate.HasValue)
            {
                queryParameters.Add($"endDate={endDate.Value:yyyy-MM-dd}");
            }
            if (!string.IsNullOrEmpty(location))
            {
                queryParameters.Add($"location={Uri.EscapeDataString(location)}"); // URI-encode the location for safety
            }

            // Puts togetehr all the inputs to a query for the api
            var queryString = string.Join("&", queryParameters);
            var requestUrl = $"{baseUrl}api/Activity/getFilteredActivities?{queryString}";
            var response = await _client.GetAsync(requestUrl);

            // Loading the locations
            var countriesWithCities = LoadCountriesWithCities();

            // error handling, will need updating
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var activities = JsonConvert.DeserializeObject<List<Activity>>(json);

                return View(new ExploreViewModel
                {
                    Activities = activities,
                    CountriesWithCities = countriesWithCities,
                    IsFree = isFree ?? false,
                    Is18Plus = is18Plus ?? false,
                    IsFamilyFriendly = isFamilyFriendly ?? false,
                    StartDate = startDate,
                    EndDate = endDate,
                    Location = location
                });
            }
            else
            {
                return View(new ExploreViewModel
                {
                    Activities = new List<Activity>(),
                    CountriesWithCities = countriesWithCities,
                    IsFree = isFree ?? false,
                    Is18Plus = is18Plus ?? false,
                    IsFamilyFriendly = isFamilyFriendly ?? false,
                    StartDate = startDate,
                    EndDate = endDate,
                    Location = location
                });
            }
        }

        // Loads the countries json so we can list them in search bar
        private Dictionary<string, List<string>> LoadCountriesWithCities()
        {
            if (!System.IO.File.Exists(countriesData))
            {
                return new Dictionary<string, List<string>>();
            }
            var json = System.IO.File.ReadAllText(countriesData);
            return JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);
        }
    }
}
