using ISpanShop.Models.DTOs;
using ISpanShop.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace ISpanShop.Repositories
{
	/// <summary>
	/// 管理員角色資料存取實現 - 使用 ADO.NET
	/// </summary>
	public class AdminRoleRepository : IAdminRoleRepository
	{
		private readonly string _connectionString;

		public AdminRoleRepository(IConfiguration configuration)
		{
			_connectionString = configuration.GetConnectionString("DefaultConnection")
				?? throw new ArgumentNullException(nameof(configuration), "DefaultConnection 未設定");
		}

		/// <summary>
		/// 取得所有可用的角色列表
		/// </summary>
		public IEnumerable<AdminRoleDto> GetAllRoles()
		{
			var roles = new List<AdminRoleDto>();

			try
			{
				using (SqlConnection conn = new SqlConnection(_connectionString))
				{
					conn.Open();

					string query = @"
						SELECT 
							Id AS RoleId,
							RoleName,
							Description
						FROM Roles
						ORDER BY RoleName";

					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						using (SqlDataReader reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								roles.Add(new AdminRoleDto
								{
									RoleId = (int)reader["RoleId"],
									RoleName = reader["RoleName"]?.ToString(),
									Description = reader["Description"]?.ToString()
								});
							}
						}
					}
				}
			}
			catch (SqlException ex)
			{
				throw new InvalidOperationException("資料庫查詢失敗", ex);
			}

			return roles;
		}

		
	}
}
