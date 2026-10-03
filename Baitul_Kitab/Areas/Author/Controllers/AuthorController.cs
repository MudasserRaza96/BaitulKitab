using Baitul_Kitab.BAL.Interfaces;
using Baitul_Kitab.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Baitul_Kitab.Areas.Author.Controllers
{
    [Area("Author")]
    [Authorize(Roles = "Admin")]
    public class AuthorController : Controller
    {
        private readonly IAuthors _authorsService;
        public AuthorController(IAuthors authorsService)
        {
            _authorsService = authorsService;
        }


        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetNationalities(string term)
        {
            var nationalities = new[]
            {
                "Afghan", "Albanian", "Algerian", "American", "Andorran", "Angolan", "Argentine", "Armenian", "Australian", "Austrian", "Azerbaijani",
                "Bahamian", "Bahraini", "Bangladeshi", "Barbadian", "Belarusian", "Belgian", "Belizean", "Beninese", "Bhutanese", "Bolivian", "Bosnian", 
                "Brazilian", "British", "Bruneian", "Bulgarian", "Burkinabe", "Burmese", "Burundian", "Cambodian", "Cameroonian", "Canadian", "Cape Verdean",
                "Central African", "Chadian", "Chilean", "Chinese", "Colombian", "Comoran", "Congolese", "Costa Rican", "Croatian", "Cuban", "Cypriot", "Czech",
                "Danish", "Djiboutian", "Dominican", "Dutch", "East Timorese", "Ecuadorean", "Egyptian", "Emirati", "Equatorial Guinean", "Eritrean", "Estonian",
                "Ethiopian", "Fijian", "Finnish", "French", "Gabonese", "Gambian", "Georgian", "German", "Ghanaian", "Greek", "Grenadian", "Guatemalan", "Guinea-Bissauan",
                "Guinean", "Guyanese", "Haitian", "Herzegovinian", "Honduran", "Hungarian", "I-Kiribati", "Icelander", "Indian", "Indonesian", "Iranian", "Iraqi", "Irish",
                "Israeli", "Italian", "Ivorian", "Jamaican", "Japanese", "Jordanian", "Kazakhstani", "Kenyan", "Kittian and Nevisian", "Kuwaiti", "Kyrgyz", "Laotian", "Latvian",
                "Lebanese", "Liberian", "Libyan", "Liechtensteiner", "Lithuanian", "Luxembourger", "Macedonian", "Malagasy", "Malawian", "Malaysian", "Maldivan", "Malian",
                "Maltese", "Marshallese", "Mauritanian", "Mauritian", "Mexican", "Micronesian", "Moldovan", "Monacan", "Mongolian", "Moroccan", "Mosotho", "Motswana",
                "Mozambican", "Namibian", "Nauruan", "Nepalese", "New Zealander", "Nicaraguan", "Nigerian", "Nigerien", "North Korean", "Northern Irish", "Norwegian",
                "Omani", "Pakistani", "Palauan", "Panamanian", "Papua New Guinean", "Paraguayan", "Peruvian", "Polish", "Portuguese", "Qatari", "Romanian", "Russian",
                "Rwandan", "Saint Lucian", "Salvadoran", "Samoan", "San Marinese", "Sao Tomean", "Saudi", "Scottish", "Senegalese", "Serbian", "Seychellois", "Sierra Leonean",
                "Singaporean", "Slovakian", "Slovenian", "Solomon Islander", "Somali", "South African", "South Korean", "Spanish", "Sri Lankan", "Sudanese", "Surinamer", 
                "Swazi", "Swedish", "Swiss", "Syrian", "Taiwanese", "Tajik", "Tanzanian", "Thai", "Togolese", "Tongan", "Trinidadian or Tobagonian", "Tunisian", "Turkish", 
                "Tuvaluan", "Ugandan", "Ukrainian", "Uruguayan", "Uzbekistani", "Venezuelan", "Vietnamese", "Welsh", "Yemenite", "Zambian", "Zimbabwean"
            };
            if (!string.IsNullOrWhiteSpace(term))
            {
                nationalities = nationalities.Where(n => n.ToLower().Contains(term.ToLower())).ToArray();
            }
            return Json(nationalities);
        }


        [HttpPost]
        public async Task<IActionResult> Add(DTOAuthor model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                return Json(new { result = 0, errorMessage = "Name is required" });
            var all = await _authorsService.GetAllAsync();
            if (all.Any(a => a.Name?.Trim().ToLower() == model.Name.Trim().ToLower()))
                return Json(new { result = -5, errorMessage = "Name Already Exists" });
            var success = await _authorsService.AddAsync(model);
            return Json(new { result = success ? 1 : 0, errorMessage = success ? "Author added successfully" : "Failed to add author" });
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var authors = await _authorsService.GetAllAsync();
            return Json(authors);
        }

        [HttpPost]
        public async Task<IActionResult> Update(DTOAuthor model)
        {
            if (model.Id == null || string.IsNullOrWhiteSpace(model.Name))
                return Json(new { result = 0, errorMessage = "Invalid data" });
            var success = await _authorsService.UpdateAsync(model);
            return Json(new { result = success ? 1 : 0, errorMessage = success ? "Author updated successfully" : "Failed to update author" });
        }


        [HttpPost]
        public async Task<IActionResult> Delete(System.Guid id)
        {
            var success = await _authorsService.DeleteAsync(id);
            return Json(new { result = success, errorMessage = success ? "Author deleted successfully" : "Failed to delete author" });
        }
    }
}
