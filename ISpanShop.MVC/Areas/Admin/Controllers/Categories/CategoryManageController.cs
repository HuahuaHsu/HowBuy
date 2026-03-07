using ISpanShop.Models.DTOs.Categories;
using ISpanShop.Services.Products;
using ISpanShop.Services.Categories;
using ISpanShop.Services.Inventories;
using Microsoft.AspNetCore.Mvc;
using ISpanShop.MVC.Areas.Admin.Controllers;

namespace ISpanShop.MVC.Areas.Admin.Controllers.Categories
{
    public class CategoryManageController : AdminBaseController
    {
        private readonly CategoryManageService _svc;
        public CategoryManageController(CategoryManageService svc) => _svc = svc;

        public IActionResult Index()
        {
            var tree = _svc.GetTree();
            return View(tree);
        }

        [HttpGet]
        public IActionResult GetById(int id)
        {
            var cat = _svc.GetById(id);
            if (cat == null) return NotFound();
            return Json(cat);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CategoryCreateDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest(new { success = false, message = "名稱不能空白" });
            _svc.Create(dto.Name.Trim(), dto.NameEn?.Trim(), dto.ParentId, dto.SortOrder, dto.ImageUrl);
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult Update([FromBody] CategoryUpdateDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest(new { success = false, message = "名稱不能空白" });
            _svc.Update(dto.Id, dto.Name.Trim(), dto.NameEn?.Trim(), dto.ParentId, dto.SortOrder, dto.ImageUrl);
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult Delete([FromBody] IdDto dto)
        {
            var productCount = _svc.GetProductCount(dto.Id);
            if (productCount > 0)
                return Json(new { success = false, message = $"此分類有 {productCount} 筆商品，請先移除商品再刪除" });
            var ok = _svc.Delete(dto.Id);
            if (!ok) return Json(new { success = false, message = "此分類有子分類，無法刪除" });
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult ToggleActive([FromBody] ToggleActiveDto dto)
        {
            _svc.UpdateIsActive(dto.Id, dto.IsActive);
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult UpdateSort([FromBody] UpdateSortDto dto)
        {
            _svc.UpdateSortOrder(dto.Id, dto.SortOrder);
            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult GetProductCount(int categoryId)
        {
            var count = _svc.GetProductCount(categoryId);
            return Json(new { count });
        }
    }
}
