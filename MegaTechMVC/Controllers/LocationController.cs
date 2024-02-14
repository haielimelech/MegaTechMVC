using Microsoft.AspNetCore.Mvc;
using MegaTechMVC.Models;
using MegaTechMVC.Data;
using MegaTechMVC.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;


namespace MegaTechMVC.Controllers
{
    public class LocationController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public LocationController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddLocationViewModel viewModel)
        {
            try
            {
                var existingLocation = await dbContext.Locations.FirstOrDefaultAsync(l => l.Name == viewModel.Name);
                if (existingLocation != null)
                {

                    return View("Error", new ErrorViewModel { ErrorMessage = "Location already exists." });
                }

                var location = new Location
                {
                    Name = viewModel.Name
                };

                await dbContext.Locations.AddAsync(location);
                await dbContext.SaveChangesAsync();

                return RedirectToAction("List", "Location");
            }
            catch (DbUpdateException ex)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "Error adding location: Duplicate entry." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var locations = await dbContext.Locations.OrderBy(location => location.Name).ToListAsync();

            return View(locations);
        }

        [HttpGet]
        public async Task<IActionResult> ListOrderBy()
        {
            var locations = await dbContext.Locations.OrderByDescending(location => location.Name).ToListAsync();

            return View("List", locations);
        }
        public static bool IsEnglishAlphabet(string str)
        {
            //regular expressing for accepting all the keyboard apart from numbers.
            string englishPattern = @"^[a-zA-Z\s!@#$%^&*()_+{}\[\]:;<>,.?\/\\|-]*$";

            return Regex.IsMatch(str, englishPattern);
        }
        public static string MapEnglishToHebrew(string input)
        {
            Dictionary<char, char> EnglishToHebrewMapping = new Dictionary<char, char>
     {
        {'q', '/'},
        {'w', '\''},
        {'e', 'ק'},
        {'r', 'ר'},
        {'t', 'א'},
        {'y', 'ט'},
        {'u', 'ו'},
        {'i', 'ן'},
        {'o', 'ם'},
        {'p', 'פ'},
        {'a', 'ש'},
        {'s', 'ד'},
        {'d', 'ג'},
        {'f', 'כ'},
        {'g', 'ע'},
        {'h', 'י'},
        {'j', 'ח'},
        {'k', 'ל'},
        {'l', 'ך'},
        {'z', 'ז'},
        {'x', 'ס'},
        {'c', 'ב'},
        {'v', 'ה'},
        {'b', 'נ'},
        {'n', 'מ'},
        {'m', 'צ'},
        {',', 'ת'},
        {'.', 'ץ'},
        {'/', ','},
        {'\'', '.'}

    };
            char[] output = new char[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                char englishChar = input[i];
                if (EnglishToHebrewMapping.ContainsKey(englishChar))
                {
                    output[i] = EnglishToHebrewMapping[englishChar];
                }
                else
                {
                    output[i] = englishChar;
                }
            }

            return new string(output);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string searchQuery)
        {
            if (string.IsNullOrEmpty(searchQuery))
            {
                return RedirectToAction("List", "Location");
            }
            string tempSearchQuery = searchQuery;
            if (IsEnglishAlphabet(searchQuery))
            {
                tempSearchQuery = MapEnglishToHebrew(searchQuery.ToLower());
            }
            var locations = await dbContext.Locations.Where(l => l.Name.StartsWith(tempSearchQuery)).ToListAsync();

            if (locations == null || !locations.Any())
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "No Result Found." });
            }

            return View("List", locations);
        }



        [HttpGet]
        public async Task<IActionResult> Edit(string locationName)
        {

            var location = await dbContext.Locations.FindAsync(locationName);

            return View(location);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(string originalName, string newName)
        {
            var location = await dbContext.Locations.FirstOrDefaultAsync(l => l.Name == originalName);

            if (location is not null)
            {
                dbContext.Locations.Remove(location);
                await dbContext.SaveChangesAsync();

                var newLocation = new Location { Name = newName };
                await dbContext.Locations.AddAsync(newLocation);
                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("List", "Location");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string originalName)
        {
            var location = await dbContext.Locations.FirstOrDefaultAsync(l => l.Name == originalName);

            if (location is not null)
            {
                dbContext.Locations.Remove(location);
                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("List", "Location");
        }

    }
}
