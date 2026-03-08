using ISpanShop.Models.DTOs;
using System.Collections.Generic;

namespace ISpanShop.Repositories.Interfaces
{
	/// <summary>
	/// 管理員角色資料存取介面
	/// </summary>
	public interface IAdminRoleRepository
	{
		/// <summary>
		/// 取得所有可用的角色列表
		/// </summary>
		/// <returns>角色 DTO 列表</returns>
		IEnumerable<AdminRoleDto> GetAllRoles();
	}
}
