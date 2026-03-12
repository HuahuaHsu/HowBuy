using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace ISpanShop.Models.DTOs.Admins
{
	/// <summary>
	/// 管理員資料傳輸物件 - 用於列表、詳細資訊及編輯
	/// </summary>
	public class AdminDto
	{
		[Display(Name = "管理員ID")]
		public int UserId { get; set; }

		[Display(Name = "帳號")]
		public string Account { get; set; }

		[Display(Name = "電子信箱")]
		public string Email { get; set; }

		[Display(Name = "角色ID")]
		public int RoleId { get; set; }

		[Display(Name = "角色名稱")]
		public string RoleName { get; set; }

		[Display(Name = "管理員等級ID")]
		public int? AdminLevelId { get; set; }

		[Display(Name = "管理員等級名稱")]
		public string AdminLevelName { get; set; }

		[Display(Name = "是否停權")]
		public bool IsBlacklisted { get; set; }

		[Display(Name = "是否首次登入")]
		public bool IsFirstLogin { get; set; }

		[Display(Name = "建立時間")]
		public DateTime CreatedAt { get; set; }

		[Display(Name = "更新時間")]
		public DateTime? UpdatedAt { get; set; }

		[JsonIgnore]//[JsonIgnore] 屬性來隱藏敏感欄位
		public string PasswordHash { get; set; }

	}

	/// <summary>
	/// 管理員等級資料傳輸物件 - 用於 Modal 下拉選單來源
	/// </summary>
	public class AdminLevelDto
	{
		public int AdminLevelId { get; set; }
		public string LevelName { get; set; }
		public string Description { get; set; }
	}

	/// <summary>
	/// 新增管理員 DTO
	/// </summary>
	public class AdminCreateDto
	{
		[Required(ErrorMessage = "{0}為必填")]
		[Display(Name = "初始密碼")]
		public string Password { get; set; }

		[Required(ErrorMessage = "{0}為必填")]
		[Display(Name = "管理員等級")]
		public int AdminLevelId { get; set; }
	}

	/// <summary>
	/// 修改管理員密碼 DTO
	/// </summary>
	public class AdminChangePasswordDto
	{
		public int UserId { get; set; }

		[Required(ErrorMessage = "{0}為必填")]
		[DataType(DataType.Password)]
		[Display(Name = "新密碼")]
		public string NewPassword { get; set; }

		[Required(ErrorMessage = "{0}為必填")]
		[DataType(DataType.Password)]
		[Compare("NewPassword", ErrorMessage = "兩次密碼輸入不一致")]
		[Display(Name = "確認新密碼")]
		public string ConfirmPassword { get; set; }
	}

	/// <summary>
	/// 角色資料傳輸物件 - 用於下拉選單
	/// </summary>
	public class AdminPermissionDto
	{
		[Display(Name = "角色ID")]
		public int PermissionId { get; set; }

		[Display(Name = "角色名稱")]
		public string PermissionKey { get; set; }

		[Display(Name = "描述")]
		public string Description { get; set; }
	}
}
