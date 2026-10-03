using Baitul_Kitab.BAL.Interfaces;
using Baitul_Kitab.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Baitul_Kitab.Areas.Category.Controllers
{
    [Area("Category")]
    [Authorize(Roles = "Admin")]
    public class LanguageController : Controller
    {
        private readonly ILanguages objLanguage;

        public LanguageController(ILanguages language)
        {
            objLanguage = language;
        }
        public IActionResult Language()
        {
            return View();
        }
        public async Task<IActionResult> AddLanguage(DTOLanguage param)
        {
            int? result = -1;
            try
            {
                if (param.Id != null)
                {
                    result = await objLanguage.UpdateLanguage(param);
                    return Json(new
                    {
                        result = result,
                        ErrorMessage = "Language are Updated Successfully"

                    });
                }
                else
                {
                    result = objLanguage.AddLanguage(param);
                    return Json(new
                    {
                        result = result,
                        ErrorMessage = "Language are Inserted Successfully"

                    });
                }

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    result = -1,
                    ErrorMessage = ex.ToString()

                });
            }



        }

        [HttpPost]
        public IActionResult GetLanguageList()
        {
            try
            {
                var draw = Convert.ToInt32(Request.Form["draw"]);
                var start = Convert.ToInt32(Request.Form["start"]);
                var length = Convert.ToInt32(Request.Form["length"]);
                var sortColumnIndex = Convert.ToInt32(Request.Form["order[0][column]"]);
                var sortColumnName = Request.Form[$"columns[{sortColumnIndex}][data]"];
                var sortDirection = Request.Form["order[0][dir]"];
                var searchValue = Request.Form["search[value]"];

                var response = objLanguage.GetLanguageList(draw, start, length, sortColumnName, sortDirection, searchValue);

                return Json(data: response);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        public async Task<IActionResult> DeleteLanguage(Guid? id)
        {
            try
            {
                if (id == null)
                {
                    return Json(new
                    {
                        result = false,
                        ErrorMessage = "Invalid Category Id"
                    });
                }

                // Await the async service
                bool result = await objLanguage.DeleteLanguage(id);

                if (result)
                {
                    return Json(new
                    {
                        result = true,
                        ErrorMessage = "Category Deleted Successfully"
                    });
                }
                else
                {
                    return Json(new
                    {
                        result = false,
                        ErrorMessage = "This Category is Not Available"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    result = false,
                    ErrorMessage = ex.Message
                });
            }
        }

    }
}
