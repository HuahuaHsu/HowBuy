using ISpanShop.Models.DTOs.Orders;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ISpanShop.MVC.Areas.Admin.Models.Orders
{
	public class OrderIndexVm
	{
		public PagedResultDto<OrderDto> Orders { get; set; }
		public OrderSearchDto Criteria { get; set; }

		// 提供給前端的下拉選單選項
		public List<SelectListItem> StatusOptions { get; set; } = new List<SelectListItem>();
		public List<SelectListItem> StoreOptions { get; set; } = new List<SelectListItem>();
		public List<SelectListItem> DateDimensionOptions { get; set; } = new List<SelectListItem>();
	}
}

