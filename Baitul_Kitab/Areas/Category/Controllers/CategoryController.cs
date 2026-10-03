using Baitul_Kitab.BAL.Interfaces;
using Baitul_Kitab.BAL.Services;
using Baitul_Kitab.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Baitul_Kitab.Areas.Category.Controllers
{
    [Area("Category")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategories _categoryService;

        public CategoryController(ICategories categoryService)
        {
            _categoryService = categoryService;
        }
        public IActionResult Index() => View();

        public async Task<IActionResult> AddCategory(CategoryDTO param) 
        {
            int? result = -1;
            try
            {
                if(param.Id != null) 
                { 
                   result = await _categoryService.UpdateCategory(param);
                    return Json(new
                    {
                        result = result,
                        ErrorMessage = "Category are Updated Successfully"

                    });
                }
                else
                {
                    result = _categoryService.AddCategory(param);
                    return Json(new
                    {
                        result = result,
                        ErrorMessage = "Category are Inserted Successfully"

                    });
                }
                
            } catch (Exception ex)
            {
                return Json(new
                {
                    result = -1,
                    ErrorMessage = ex.ToString()

                });
            }
            
           
        
        }

        [HttpPost]
        public IActionResult GetCategoryList()
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

                var response = _categoryService.GetCategoryList(draw, start, length, sortColumnName, sortDirection, searchValue);

                return Json(data: response);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }


        public async Task<IActionResult> DeleteCategory(Guid? id)
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
                bool result = await _categoryService.DeleteCategory(id);

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
