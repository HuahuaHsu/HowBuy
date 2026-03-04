using ISpanShop.Models.DTOs;
using ISpanShop.Models.ViewModels;
using ISpanShop.MVC.Models;
using ISpanShop.Services;
using Microsoft.AspNetCore.Mvc;

namespace ISpanShop.MVC.Controllers
{
	public class CategorySpecsController : Controller
	{
		private readonly CategorySpecService _categorySpecService;

		public CategorySpecsController(CategorySpecService categorySpecService)
		{
			_categorySpecService = categorySpecService;
		}

		public IActionResult Index()
		{
			var specs = _categorySpecService.GetAll();
			ViewBag.Categories = _categorySpecService.GetAllCategories();
			return View(specs);
		}

		/// <summary>
		/// AJAX：取得某分類已綁定的規格 ID 清單
		/// </summary>
		[HttpGet]
		public IActionResult GetBoundSpecIds(int categoryId)
		{
			var ids = _categorySpecService.GetBoundAttributeIds(categoryId);
			return Json(ids);
		}

		/// <summary>
		/// AJAX：儲存某分類的規格綁定
		/// </summary>
		[HttpPost]
		public IActionResult SaveCategorySpecs([FromBody] SaveBindingsDto dto)
		{
			if (dto == null) return BadRequest(new { success = false });
			_categorySpecService.SaveCategoryBindings(dto.CategoryId, dto.AttributeIds);
			return Json(new { success = true });
		}

		/// <summary>
		/// AJAX：取得矩陣頁所需的全部資料（規格、分類、全部綁定）
		/// </summary>
		[HttpGet]
		public IActionResult GetMatrixData()
		{
			var allCats   = _categorySpecService.GetAllCategories().ToList();
			var parents   = allCats.Where(c => c.ParentId == null)
				.Select(c => new { id = c.Id, name = c.Name });
			var children  = allCats.Where(c => c.ParentId != null)
				.Select(c => new { id = c.Id, name = c.Name, parentId = c.ParentId });

			var specs     = _categorySpecService.GetAll()
				.Select(s => new {
					id        = s.Id,
					name      = s.Name,
					inputType = s.InputType,
					isRequired = s.IsRequired,
					options   = s.Options
				});

			var bindings  = _categorySpecService.GetAllBindings()
				.Select(b => new { categoryId = b.CategoryId, attributeId = b.AttributeId });

			return Json(new {
				specs,
				categories       = children,
				parentCategories = parents,
				bindings
			});
		}

		/// <summary>
		/// AJAX：切換單一規格綁定（即時儲存，不需重建全表）
		/// </summary>
		[HttpPost]
		public IActionResult ToggleSpec([FromBody] ToggleSpecDto dto)
		{
			if (dto == null) return BadRequest(new { success = false });
			_categorySpecService.ToggleSpec(dto.CategoryId, dto.AttributeId, dto.IsBound);
			return Json(new { success = true });
		}

		public IActionResult Create()
		{
			return View(new CategorySpecCreateVm());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create(CategorySpecCreateVm vm)
		{
			if (!ModelState.IsValid) return View(vm);

			// Controller 負責把 Vm 解包成基本參數傳給 Service
			_categorySpecService.Create(
				vm.Name,
				vm.InputType,
				vm.IsRequired,
				vm.SortOrder,
				vm.Options
			);

			TempData["SuccessMessage"] = "分類規格新增成功！";
			return RedirectToAction(nameof(Index));
		}

		public IActionResult Edit(int id)
		{
			var dto = _categorySpecService.GetById(id);
			if (dto == null) return NotFound();

			// Controller 負責把 DTO 轉換成 Vm 給 View 用
			var vm = new CategorySpecEditVm
			{
				Id = dto.Id,
				Name = dto.Name,
				InputType = dto.InputType,
				IsRequired = dto.IsRequired,
				SortOrder = dto.SortOrder,
				Options = dto.Options
			};

			return View(vm);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(CategorySpecEditVm vm)
		{
			if (!ModelState.IsValid) return View(vm);

			// Controller 負責把 Vm 解包成基本參數傳給 Service
			_categorySpecService.Update(
				vm.Id,
				vm.Name,
				vm.InputType,
				vm.IsRequired,
				vm.SortOrder,
				vm.Options
			);

			TempData["SuccessMessage"] = "分類規格更新成功！";
			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Delete(int id)
		{
			_categorySpecService.Delete(id);
			TempData["SuccessMessage"] = "分類規格已刪除！";
			return RedirectToAction(nameof(Index));
		}

		// ── 分類綁定 ───────────────────────────────────

		/// <summary>
		/// GET：顯示某分類的規格綁定管理頁
		/// </summary>
		public IActionResult ManageBindings(int id) // id = categoryId
		{
			var dtos = _categorySpecService.GetSpecsForCategory(id);

			var vm = new CategorySpecBindingVm
			{
				CategoryId   = id,
				CategoryName = _categorySpecService.GetCategoryName(id),
				Specs = dtos.Select(dto => new SpecCheckItem
				{
					AttributeId   = dto.AttributeId,
					AttributeName = dto.AttributeName,
					InputType     = dto.InputType,
					IsRequired    = dto.IsRequired,
					SortOrder     = dto.SortOrder,
					OptionPreview = dto.OptionPreview,
					IsBound       = dto.IsBound
				}).ToList()
			};

			return View(vm);
		}

		/// <summary>
		/// POST：儲存某分類的規格綁定
		/// </summary>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult ManageBindings(CategorySpecBindingVm vm)
		{
			var boundIds = vm.Specs
				.Where(s => s.IsBound)
				.Select(s => s.AttributeId)
				.ToList();

			_categorySpecService.UpdateSpecBindingsForCategory(vm.CategoryId, boundIds);

			TempData["SuccessMessage"] = $"「{vm.CategoryName}」的規格綁定已更新！";
			return RedirectToAction(nameof(Index));
		}
	}
}