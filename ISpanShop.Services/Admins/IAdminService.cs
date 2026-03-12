using ISpanShop.Models.DTOs.Admins;
using System.Collections.Generic;

namespace ISpanShop.Services.Admins
{
	/// <summary>
	/// 管理員服務介面 - 處理管理員相關業務邏輯
	/// </summary>
	public interface IAdminService
	{
		/// <summary>
		/// 取得所有管理員資訊
		/// </summary>
		/// <returns>管理員 DTO 列表</returns>
		IEnumerable<AdminDto> GetAllAdmins();

		/// <summary>
		/// 取得所有可用的管理員權限列表（用於下拉選單）
		/// </summary>
		/// <returns>管理員權限 列表</returns>
		IEnumerable<AdminPermissionDto> GetAllPermissions();

		/// <summary>
		/// 取得所有可選擇的管理員等級（排除超級管理員）
		/// </summary>
		/// <returns>管理員等級 DTO 列表</returns>
		IEnumerable<AdminLevelDto> GetSelectableAdminLevels();

		/// <summary>
		/// 新增管理員
		/// </summary>
		/// <param name="dto">新增管理員的 DTO</param>
		/// <returns>是否成功、訊息、生成的帳號</returns>
		(bool IsSuccess, string Message, string GeneratedAccount) CreateAdmin(AdminCreateDto dto);

		/// <summary>
		/// 停用管理員
		/// </summary>
		/// <param name="userId">要停用的管理員 ID</param>
		/// <param name="currentUserId">當前登入的管理員 ID（用於防止自我停用）</param>
		/// <returns>是否成功、訊息</returns>
		(bool IsSuccess, string Message) DeactivateAdmin(int userId, int currentUserId);

		/// <summary>
		/// 驗證管理員登入
		/// </summary>
		/// <param name="account">帳號</param>
		/// <param name="password">密碼</param>
		/// <returns>驗證成功回傳 AdminDto，失敗回傳 null</returns>
		AdminDto? VerifyLogin(string account, string password);

		/// <summary>
		/// 變更管理員密碼
		/// </summary>
		/// <param name="dto">包含新密碼的 DTO</param>
		/// <returns>是否成功、訊息</returns>
		(bool IsSuccess, string Message) ChangePassword(AdminChangePasswordDto dto);

		/// <summary>
		/// 更新管理員的權限
		/// </summary>
		/// <param name="adminId">要更新的管理員 ID</param>
		/// <param name="roleId">新的權限</param>
		/// <param name="currentPermissionId">當前登入的管理員 ID（用於防止自我鎖定）</param>
		/// <returns>是否更新成功</returns>
		/// <exception cref="InvalidOperationException">當管理員試圖修改自己的角色時拋出</exception>
		bool UpdateAdminRole(int adminId, int PermissionId, int currentPermissionId);
	}
}
