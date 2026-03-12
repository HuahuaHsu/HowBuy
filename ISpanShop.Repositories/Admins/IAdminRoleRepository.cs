using ISpanShop.Models.DTOs.Admins;
using System.Collections.Generic;

namespace ISpanShop.Repositories.Admins
{
	/// <summary>
	/// 管理員角色資料存取介面
	/// </summary>
	public interface IAdminRoleRepository
	{
		/// <summary>
		/// 取得所有可用的管理員權限列表
		/// </summary>
		/// <returns>管理員權限 DTO 列表</returns>
		IEnumerable<AdminPermissionDto> GetAllPermissions();
	}
}
