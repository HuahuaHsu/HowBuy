using ISpanShop.Models.DTOs;
using System.Collections.Generic;

namespace ISpanShop.Services.Interfaces
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
		/// 取得所有可用的角色列表（用於下拉選單）
		/// </summary>
		/// <returns>角色 DTO 列表</returns>
		IEnumerable<AdminRoleDto> GetAllRoles();

		/// <summary>
		/// 更新管理員的角色
		/// </summary>
		/// <param name="adminId">要更新的管理員 ID</param>
		/// <param name="roleId">新的角色 ID</param>
		/// <param name="currentAdminId">當前登入的管理員 ID（用於防止自我鎖定）</param>
		/// <returns>是否更新成功</returns>
		/// <exception cref="InvalidOperationException">當管理員試圖修改自己的角色時拋出</exception>
		bool UpdateAdminRole(int adminId, int roleId, int currentAdminId);
	}
}
