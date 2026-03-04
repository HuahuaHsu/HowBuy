using Microsoft.AspNetCore.Mvc;
using ISpanShop.Services;
using ISpanShop.Models.DTOs;

namespace ISpanShop.MVC.Controllers
{
    public class CategoryBindingController : Controller
    {
        private readonly CategorySpecService _specService;

        public CategoryBindingController(CategorySpecService specService)
        {
            _specService = specService;
        }

        // 首頁：只渲染分類列表，規格用 AJAX 載入
        public IActionResult Index()
        {
            var categories = _specService.GetAllCategories();
            return View(categories);
        }

        // AJAX：取得某分類的所有規格（含是否已綁定）
        [HttpGet]
        public IActionResult GetSpecsForCategory(int categoryId)
        {
            var allSpecs = _specService.GetAll();
            var boundIds = _specService.GetBoundAttributeIds(categoryId);
            var categoryName = _specService.GetAllCategories()
                .FirstOrDefault(c => c.Id == categoryId)?.Name ?? "";

            var result = allSpecs.Select(s => new {
                attributeId  = s.Id,
                attributeName = s.Name,
                inputType    = s.InputType,
                isRequired   = s.IsRequired,
                isBound      = boundIds.Contains(s.Id),
                options      = s.Options
            });

            return Json(new { categoryName, specs = result });
        }

        // AJAX：儲存分類規格綁定
        [HttpPost]
        public IActionResult SaveBindings([FromBody] SaveBindingsDto dto)
        {
            if (dto == null) return BadRequest(new { success = false });
            _specService.SaveCategoryBindings(dto.CategoryId, dto.AttributeIds);
            return Json(new { success = true });
        }
    }
}
