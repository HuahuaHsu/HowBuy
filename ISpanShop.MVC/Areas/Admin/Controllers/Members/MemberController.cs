using ISpanShop.Models.DTOs.Common;
using ISpanShop.MVC.Areas.Admin.Models.Members;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ISpanShop.MVC.Areas.Admin.Controllers.Members
{
	[Area("Admin")]
	[Route("Admin/Members")]
	public class MemberController : Controller
	{
		[HttpGet]
		public IActionResult Index(string keyword = "", int page = 1, int pageSize = 10)
		{
			// === 建立 Mock Data 供測試使用 ===
			var allMembers = new List<MemberItemVm>
			{
				new MemberItemVm
				{
					UserId = 1,
					Account = "jackchan",
					FullName = "陳大文",
					Email = "jackchan@example.com",
					PhoneNumber = "0912-345-678",
					AvatarUrl = "https://i.pravatar.cc/150?img=12",
					IsSeller = false,
					LevelName = "金牌會員",
					PointBalance = 1200,
					IsBlacklisted = false
				},
				new MemberItemVm
				{
					UserId = 2,
					Account = "mary.lee",
					FullName = "李小美",
					Email = "mary.lee@example.com",
					PhoneNumber = "0923-456-789",
					AvatarUrl = "https://i.pravatar.cc/150?img=5",
					IsSeller = true,
					LevelName = "銀牌會員",
					PointBalance = 800,
					IsBlacklisted = false
				},
				new MemberItemVm
				{
					UserId = 3,
					Account = "johnwang",
					FullName = "王小明",
					Email = "johnwang@example.com",
					PhoneNumber = "0934-567-890",
					AvatarUrl = "https://i.pravatar.cc/150?img=33",
					IsSeller = true,
					LevelName = "金牌會員",
					PointBalance = 2500,
					IsBlacklisted = false
				},
				new MemberItemVm
				{
					UserId = 4,
					Account = "linda.liu",
					FullName = "劉雅婷",
					Email = "linda.liu@example.com",
					PhoneNumber = "0945-678-901",
					AvatarUrl = "https://i.pravatar.cc/150?img=20",
					IsSeller = false,
					LevelName = "銅牌會員",
					PointBalance = 350,
					IsBlacklisted = false
				},
				new MemberItemVm
				{
					UserId = 5,
					Account = "kevin.hsu",
					FullName = "許志明",
					Email = "kevin.hsu@example.com",
					PhoneNumber = "0956-789-012",
					AvatarUrl = "https://i.pravatar.cc/150?img=51",
					IsSeller = false,
					LevelName = "銀牌會員",
					PointBalance = 600,
					IsBlacklisted = true
				},
				new MemberItemVm
				{
					UserId = 6,
					Account = "amy.chen",
					FullName = "陳美玲",
					Email = "amy.chen@example.com",
					PhoneNumber = "0967-890-123",
					AvatarUrl = "https://i.pravatar.cc/150?img=9",
					IsSeller = true,
					LevelName = "金牌會員",
					PointBalance = 3200,
					IsBlacklisted = false
				},
				new MemberItemVm
				{
					UserId = 7,
					Account = "david.chang",
					FullName = "張大衛",
					Email = "david.chang@example.com",
					PhoneNumber = "0978-901-234",
					AvatarUrl = "https://i.pravatar.cc/150?img=68",
					IsSeller = false,
					LevelName = "銅牌會員",
					PointBalance = 150,
					IsBlacklisted = false
				},
				new MemberItemVm
				{
					UserId = 8,
					Account = "sophia.wu",
					FullName = "吳雅文",
					Email = "sophia.wu@example.com",
					PhoneNumber = "0989-012-345",
					AvatarUrl = "https://i.pravatar.cc/150?img=47",
					IsSeller = true,
					LevelName = "銀牌會員",
					PointBalance = 950,
					IsBlacklisted = false
				}
			};

			// 關鍵字搜尋 (姓名、電話、帳號)
			var filteredMembers = allMembers.AsQueryable();
			if (!string.IsNullOrWhiteSpace(keyword))
			{
				filteredMembers = filteredMembers.Where(m =>
					m.Account.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
					m.FullName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
					m.PhoneNumber.Contains(keyword, StringComparison.OrdinalIgnoreCase));
			}

			// 總筆數
			var totalCount = filteredMembers.Count();

			// 分頁
			var pagedMembers = filteredMembers
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToList();

			var viewModel = new MemberIndexVm
			{
				PagedResult = new PagedResult<MemberItemVm>
				{
					Data = pagedMembers,
					TotalCount = totalCount,
					CurrentPage = page,
					PageSize = pageSize
				},
				Keyword = keyword
			};

			// 明確指定使用 Members 資料夾 (複數)
			return View("~/Areas/Admin/Views/Members/Index.cshtml", viewModel);
		}

		[HttpGet("Edit/{id}")]
		public IActionResult Edit(int id)
		{
			// === 建立 Mock Data 供測試使用 ===
			var mockMember = new MemberEditVm
			{
				UserId = id,
				Account = "jackchan",
				FullName = "陳大文",
				Email = "jackchan@example.com",
				PhoneNumber = "0912-345-678",
				AvatarUrl = "https://i.pravatar.cc/150?img=12",
				LevelName = "金牌會員",
				PointBalance = 1200,
				City = "台北市",
				Region = "中正區",
				Street = "忠孝東路一段 100 號",
				IsBlacklisted = false,
				IsSeller = true,
				CityOptions = GetCityOptions(),
				RegionOptions = GetRegionOptions("台北市")
			};

			// 實際開發時應從資料庫查詢
			// var user = await _context.Users.Include(...).FirstOrDefaultAsync(u => u.Id == id);
			// if (user == null) return NotFound();

			// 明確指定使用 Members 資料夾 (複數)
			return View("~/Areas/Admin/Views/Members/Edit.cshtml", mockMember);
		}

		[HttpPost("Edit/{id}")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, MemberEditVm model)
		{
// 移除非必要欄位的驗證
ModelState.Remove("AvatarFile"); // 圖片上傳非必填
ModelState.Remove("CityOptions"); // 導覽屬性
ModelState.Remove("RegionOptions"); // 導覽屬性


			{
				model.CityOptions = GetCityOptions();
				model.RegionOptions = GetRegionOptions(model.City);
				return View("~/Areas/Admin/Views/Members/Edit.cshtml", model);
			}

			try
			{
				// === 處理圖片上傳 ===
				if (model.AvatarFile != null && model.AvatarFile.Length > 0)
				{
					// 驗證檔案類型
					var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
					var fileExtension = Path.GetExtension(model.AvatarFile.FileName).ToLowerInvariant();
					
					if (!allowedExtensions.Contains(fileExtension))
					{
						ModelState.AddModelError("AvatarFile", "只允許上傳 jpg, jpeg, png, gif 格式的圖片");
						model.CityOptions = GetCityOptions();
						model.RegionOptions = GetRegionOptions(model.City);
						return View("~/Areas/Admin/Views/Members/Edit.cshtml", model);
					}

					// 驗證檔案大小 (限制 5MB)
					if (model.AvatarFile.Length > 5 * 1024 * 1024)
					{
						ModelState.AddModelError("AvatarFile", "圖片大小不能超過 5MB");
						model.CityOptions = GetCityOptions();
						model.RegionOptions = GetRegionOptions(model.City);
						return View("~/Areas/Admin/Views/Members/Edit.cshtml", model);
					}

					// 建立儲存目錄
					var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "avatars");
					if (!Directory.Exists(uploadsFolder))
					{
						Directory.CreateDirectory(uploadsFolder);
					}

					// 生成唯一檔名
					var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
					var filePath = Path.Combine(uploadsFolder, uniqueFileName);

					// 儲存檔案
					using (var fileStream = new FileStream(filePath, FileMode.Create))
					{
						await model.AvatarFile.CopyToAsync(fileStream);
					}

					// 更新圖片 URL
					model.AvatarUrl = $"/images/avatars/{uniqueFileName}";
				}

				// === 實際開發時應更新資料庫 ===
				// var user = await _context.Users.Include(u => u.MemberProfile).Include(u => u.Addresses)
				//     .FirstOrDefaultAsync(u => u.Id == id);
				// if (user == null) return NotFound();
				//
				// user.Email = model.Email;
				// user.IsBlacklisted = model.IsBlacklisted;
				//
				// if (user.MemberProfile != null)
				// {
				//     user.MemberProfile.FullName = model.FullName;
				//     user.MemberProfile.PhoneNumber = model.PhoneNumber;
				// }
				//
				// if (!string.IsNullOrWhiteSpace(model.AvatarUrl))
				// {
				//     // 更新會員的 AvatarUrl 欄位 (假設有此欄位)
				//     // user.AvatarUrl = model.AvatarUrl;
				// }
				//
				// var defaultAddress = user.Addresses.FirstOrDefault(a => a.IsDefault == true);
				// if (defaultAddress != null)
				// {
				//     defaultAddress.City = model.City;
				//     defaultAddress.Region = model.Region;
				//     defaultAddress.Street = model.Street;
				// }
				// else if (!string.IsNullOrWhiteSpace(model.City))
				// {
				//     user.Addresses.Add(new Address
				//     {
				//         City = model.City,
				//         Region = model.Region,
				//         Street = model.Street,
				//         IsDefault = true,
				//         RecipientName = model.FullName,
				//         RecipientPhone = model.PhoneNumber
				//     });
				// }
				//
				// await _context.SaveChangesAsync();

				TempData["Success"] = "會員資料更新成功！";
				return RedirectToAction("Index");
			}
			catch (Exception)
			{
				// 開發模式下直接拋出例外以顯示詳細錯誤
				throw;
			}
		}

		private List<SelectListItem> GetCityOptions()
		{
			return new List<SelectListItem>
			{
				new SelectListItem { Text = "請選擇縣市", Value = "" },
				new SelectListItem { Text = "台北市", Value = "台北市" },
				new SelectListItem { Text = "新北市", Value = "新北市" },
				new SelectListItem { Text = "桃園市", Value = "桃園市" },
				new SelectListItem { Text = "台中市", Value = "台中市" },
				new SelectListItem { Text = "台南市", Value = "台南市" },
				new SelectListItem { Text = "高雄市", Value = "高雄市" }
			};
		}

		private List<SelectListItem> GetRegionOptions(string city)
		{
			var regions = city switch
			{
				"台北市" => new[] { "中正區", "大同區", "中山區", "松山區", "大安區", "萬華區", "信義區", "士林區", "北投區", "內湖區", "南港區", "文山區" },
				"新北市" => new[] { "板橋區", "三重區", "中和區", "永和區", "新莊區", "新店區", "土城區", "蘆洲區", "樹林區", "汐止區" },
				"桃園市" => new[] { "桃園區", "中壢區", "平鎮區", "八德區", "楊梅區", "蘆竹區", "龜山區", "龍潭區", "大溪區" },
				"台中市" => new[] { "中區", "東區", "南區", "西區", "北區", "西屯區", "南屯區", "北屯區", "豐原區", "太平區" },
				"台南市" => new[] { "中西區", "東區", "南區", "北區", "安平區", "安南區", "永康區", "歸仁區", "新化區", "左鎮區" },
				"高雄市" => new[] { "新興區", "前金區", "苓雅區", "鹽埕區", "鼓山區", "旗津區", "前鎮區", "三民區", "左營區", "楠梓區" },
				_ => new[] { "" }
			};

			var options = new List<SelectListItem> { new SelectListItem { Text = "請選擇區域", Value = "" } };
			options.AddRange(regions.Select(r => new SelectListItem { Text = r, Value = r }));
			return options;
		}
	}
}