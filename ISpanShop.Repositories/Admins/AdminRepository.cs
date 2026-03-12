using ISpanShop.Models.DTOs.Admins;
using ISpanShop.Repositories.Admins;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace ISpanShop.Repositories.Admins
{
	/// <summary>
	/// 管理員資料存取實現 - 使用 ADO.NET
	/// </summary>
	public class AdminRepository : IAdminRepository
	{
		private readonly string _connectionString;

		public AdminRepository(IConfiguration configuration)
		{
			_connectionString = configuration.GetConnectionString("DefaultConnection")
				?? throw new ArgumentNullException(nameof(configuration), "DefaultConnection 未設定");
		}

		// ==========================================
		// 以下是你原本就寫好的方法
		// ==========================================

		public AdminDto? GetAdminById(int adminId)
		{
			try
			{
				using (SqlConnection conn = new SqlConnection(_connectionString))
				{
					conn.Open();
					string query = @"
						SELECT 
							A.Id AS UserId,
							A.Account,
							A.Email,
							A.RoleId,
							R.RoleName,
							A.AdminLevelId,
							AL.LevelName AS AdminLevelName,
							A.IsBlacklisted,
							A.IsFirstLogin,
							A.CreatedAt,
							A.UpdatedAt
						FROM Users A
						JOIN Roles R ON A.RoleId = R.Id
						LEFT JOIN AdminLevels AL ON A.AdminLevelId = AL.Id
						WHERE A.Id = @AdminId
						  AND R.RoleName IN ('Admin', 'SuperAdmin')";

					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@AdminId", adminId);

						using (SqlDataReader reader = cmd.ExecuteReader())
						{
							if (reader.Read())
							{
								return new AdminDto
								{
									UserId = (int)reader["UserId"],
									Account = reader["Account"]?.ToString() ?? "",
									Email = reader["Email"]?.ToString() ?? "",
									RoleId = (int)reader["RoleId"],
									RoleName = reader["RoleName"]?.ToString() ?? "",
									AdminLevelId = reader["AdminLevelId"] != DBNull.Value ? (int?)reader["AdminLevelId"] : null,
									AdminLevelName = reader["AdminLevelName"]?.ToString() ?? "",
									IsBlacklisted = reader["IsBlacklisted"] != DBNull.Value && (bool)reader["IsBlacklisted"],
									IsFirstLogin = reader["IsFirstLogin"] != DBNull.Value && (bool)reader["IsFirstLogin"],
									CreatedAt = reader["CreatedAt"] != DBNull.Value ? (DateTime)reader["CreatedAt"] : DateTime.MinValue,
									UpdatedAt = reader["UpdatedAt"] != DBNull.Value ? (DateTime?)reader["UpdatedAt"] : null
								};
							}
							return null;
						}
					}
				}
			}
			catch (SqlException ex)
			{
				throw new InvalidOperationException("查詢管理員失敗", ex);
			}
		}

		/// <summary>
		/// 取得所有管理員資訊（僅限 Admin 和 SuperAdmin 角色）
		/// </summary>
		public IEnumerable<AdminDto> GetAllAdmins()
		{
			var admins = new List<AdminDto>();
			try
			{
				using (SqlConnection conn = new SqlConnection(_connectionString))
				{
					conn.Open();
					string query = @"
						SELECT 
							A.Id AS UserId,
							A.Account,
							A.Email,
							A.RoleId,
							R.RoleName,
							A.AdminLevelId,
							AL.LevelName AS AdminLevelName,
							A.IsBlacklisted,
							A.IsFirstLogin,
							A.CreatedAt,
							A.UpdatedAt
						FROM Users A
						JOIN Roles R ON A.RoleId = R.Id
						LEFT JOIN AdminLevels AL ON A.AdminLevelId = AL.Id
						WHERE R.RoleName IN ('Admin', 'SuperAdmin')
						ORDER BY A.CreatedAt DESC";

					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						using (SqlDataReader reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								admins.Add(new AdminDto
								{
									UserId = (int)reader["UserId"],
									Account = reader["Account"]?.ToString() ?? "",
									Email = reader["Email"]?.ToString() ?? "",
									RoleId = (int)reader["RoleId"],
									RoleName = reader["RoleName"]?.ToString() ?? "",
									AdminLevelId = reader["AdminLevelId"] != DBNull.Value ? (int?)reader["AdminLevelId"] : null,
									AdminLevelName = reader["AdminLevelName"]?.ToString() ?? "",
									IsBlacklisted = reader["IsBlacklisted"] != DBNull.Value && (bool)reader["IsBlacklisted"],
									IsFirstLogin = reader["IsFirstLogin"] != DBNull.Value && (bool)reader["IsFirstLogin"],
									CreatedAt = reader["CreatedAt"] != DBNull.Value ? (DateTime)reader["CreatedAt"] : DateTime.MinValue,
									UpdatedAt = reader["UpdatedAt"] != DBNull.Value ? (DateTime?)reader["UpdatedAt"] : null
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
			return admins;
		}

		/// <summary>
		/// 更新管理員的角色
		/// </summary>
		public bool UpdateAdminRole(int adminId, int roleId)
		{
			try
			{
				using (SqlConnection conn = new SqlConnection(_connectionString))
				{
					conn.Open();
					string query = @"
						UPDATE Users 
						SET RoleId = @RoleId, 
							UpdatedAt = GETDATE()
						WHERE Id = @AdminId 
							AND RoleId IN (SELECT Id FROM Roles WHERE RoleName IN ('Admin', 'SuperAdmin'))";

					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@AdminId", adminId);
						cmd.Parameters.AddWithValue("@RoleId", roleId);

						int rowsAffected = cmd.ExecuteNonQuery();
						return rowsAffected > 0;
					}
				}
			}
			catch (SqlException ex)
			{
				throw new InvalidOperationException("更新管理員角色失敗", ex);
			}
		}

		// ==========================================
		// 以下是剛才幫你補齊的 8 個規格方法
		// ==========================================

		/// <summary>
		/// 取得可選擇的管理員層級（排除超級管理員）
		/// </summary>
		public IEnumerable<AdminLevelDto> GetSelectableAdminLevels()
		{
			var levels = new List<AdminLevelDto>();
			try
			{
				using (SqlConnection conn = new SqlConnection(_connectionString))
				{
					conn.Open();
					string query = @"
						SELECT Id AS AdminLevelId, LevelName, Description
						FROM AdminLevels
						WHERE Id != 1
						ORDER BY Id";

					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						using (SqlDataReader reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								levels.Add(new AdminLevelDto
								{
									AdminLevelId = (int)reader["AdminLevelId"],
									LevelName = reader["LevelName"]?.ToString() ?? "",
									Description = reader["Description"]?.ToString() ?? ""
								});
							}
						}
					}
				}
			}
			catch (SqlException ex)
			{
				throw new InvalidOperationException("查詢管理員層級失敗", ex);
			}
			return levels;
		}

		/// <summary>
		/// 透過帳號取得管理員
		/// </summary>
		public AdminDto? GetAdminByAccount(string account)
		{
			try
			{
				using (SqlConnection conn = new SqlConnection(_connectionString))
				{
					conn.Open();
					string query = @"
						SELECT U.Id AS UserId,
							   U.Account,
							   U.Email,
							   U.Password AS PasswordHash,
							   U.RoleId,
							   R.RoleName,
							   U.AdminLevelId,
							   AL.LevelName AS AdminLevelName,
							   U.IsBlacklisted,
							   U.IsFirstLogin
						FROM Users U
						JOIN Roles R ON U.RoleId = R.Id
						LEFT JOIN AdminLevels AL ON U.AdminLevelId = AL.Id
						WHERE U.Account = @Account
						  AND U.IsBlacklisted = 0";

					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Account", account);
						using (SqlDataReader reader = cmd.ExecuteReader())
						{
							if (reader.Read())
							{
								return new AdminDto
								{
									UserId = (int)reader["UserId"],
									Account = reader["Account"]?.ToString() ?? "",
									Email = reader["Email"]?.ToString() ?? "",
									PasswordHash = reader["PasswordHash"]?.ToString() ?? "",
									RoleId = (int)reader["RoleId"],
									RoleName = reader["RoleName"]?.ToString() ?? "",
									AdminLevelId = reader["AdminLevelId"] != DBNull.Value ? (int?)reader["AdminLevelId"] : null,
									AdminLevelName = reader["AdminLevelName"]?.ToString() ?? "",
									IsBlacklisted = reader["IsBlacklisted"] != DBNull.Value && (bool)reader["IsBlacklisted"],
									IsFirstLogin = reader["IsFirstLogin"] != DBNull.Value && (bool)reader["IsFirstLogin"]
								};
							}
							return null;
						}
					}
				}
			}
			catch (SqlException ex)
			{
				throw new InvalidOperationException("透過帳號查詢管理員失敗", ex);
			}
		}

		/// <summary>
		/// 取得下一個管理員序列號
		/// </summary>
		public int GetNextAdminSequence()
		{
			try
			{
				using (SqlConnection conn = new SqlConnection(_connectionString))
				{
					conn.Open();
					string query = "SELECT ISNULL(MAX(Id), 0) + 1 FROM Users";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						return (int)cmd.ExecuteScalar();
					}
				}
			}
			catch (SqlException ex)
			{
				throw new InvalidOperationException("取得序列號失敗", ex);
			}
		}

		/// <summary>
		/// 新增管理員
		/// </summary>
		public bool CreateAdmin(string account, string email, string passwordHash, int roleId, int adminLevelId)
		{
			try
			{
				using (SqlConnection conn = new SqlConnection(_connectionString))
				{
					conn.Open();
					string query = @"
						INSERT INTO Users
							(RoleId, Account, Password, Email,
							 AdminLevelId, IsConfirmed,
							 IsBlacklisted, IsFirstLogin, CreatedAt)
						VALUES
							(@RoleId, @Account, @PasswordHash, @Email,
							 @AdminLevelId, 1,
							 0, 1, GETDATE())";

					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@RoleId", roleId);
						cmd.Parameters.AddWithValue("@Account", account);
						cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
						cmd.Parameters.AddWithValue("@Email", email);
						cmd.Parameters.AddWithValue("@AdminLevelId", adminLevelId);

						int rowsAffected = cmd.ExecuteNonQuery();
						return rowsAffected > 0;
					}
				}
			}
			catch (SqlException ex)
			{
				throw new InvalidOperationException("新增管理員失敗", ex);
			}
		}

		/// <summary>
		/// 停用管理員 (軟刪除)
		/// </summary>
		public bool DeactivateAdmin(int userId)
		{
			try
			{
				using (SqlConnection conn = new SqlConnection(_connectionString))
				{
					conn.Open();
					string query = "UPDATE Users SET IsBlacklisted = 1, UpdatedAt = GETDATE() WHERE Id = @UserId";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@UserId", userId);
						return cmd.ExecuteNonQuery() > 0;
					}
				}
			}
			catch (SqlException ex)
			{
				throw new InvalidOperationException("停用管理員失敗", ex);
			}
		}

		/// <summary>
		/// 檢查帳號是否存在
		/// </summary>
		public bool IsAccountExists(string account)
		{
			try
			{
				using (SqlConnection conn = new SqlConnection(_connectionString))
				{
					conn.Open();
					string query = "SELECT COUNT(1) FROM Users WHERE Account = @Account";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@Account", account);
						int count = (int)cmd.ExecuteScalar();
						return count > 0;
					}
				}
			}
			catch (SqlException ex)
			{
				throw new InvalidOperationException("檢查帳號失敗", ex);
			}
		}

		/// <summary>
		/// 變更密碼
		/// </summary>
		public bool ChangePassword(int userId, string newPasswordHash)
		{
			try
			{
				using (SqlConnection conn = new SqlConnection(_connectionString))
				{
					conn.Open();
					string query = "UPDATE Users SET Password = @PasswordHash, UpdatedAt = GETDATE() WHERE Id = @UserId";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@UserId", userId);
						cmd.Parameters.AddWithValue("@PasswordHash", newPasswordHash);
						return cmd.ExecuteNonQuery() > 0;
					}
				}
			}
			catch (SqlException ex)
			{
				throw new InvalidOperationException("變更密碼失敗", ex);
			}
		}

		/// <summary>
		/// 設定為完成首次登入
		/// </summary>
		public bool SetFirstLoginComplete(int userId)
		{
			try
			{
				using (SqlConnection conn = new SqlConnection(_connectionString))
				{
					conn.Open();
					string query = "UPDATE Users SET IsFirstLogin = 0, UpdatedAt = GETDATE() WHERE Id = @UserId";
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.Parameters.AddWithValue("@UserId", userId);
						return cmd.ExecuteNonQuery() > 0;
					}
				}
			}
			catch (SqlException ex)
			{
				throw new InvalidOperationException("更新首次登入狀態失敗", ex);
			}
		}

		/// <summary>
		/// 取得超級管理員的數量
		/// </summary>
		public int GetSuperAdminCount()
		{
			try
			{
				using (SqlConnection conn = new SqlConnection(_connectionString))
				{
					conn.Open();
					string query = @"
						SELECT COUNT(1) FROM Users U
						JOIN AdminLevels AL ON U.AdminLevelId = AL.Id
						WHERE AL.Id = 1 AND U.IsBlacklisted = 0";

					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						return (int)cmd.ExecuteScalar();
					}
				}
			}
			catch (SqlException ex)
			{
				throw new InvalidOperationException("查詢超級管理員數量失敗", ex);
			}
		}
	}
}