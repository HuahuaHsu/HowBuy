using ISpanShop.Models.DTOs.Admins;
using ISpanShop.Repositories.Admins;
using System.Text.RegularExpressions;
using BCrypt.Net;

namespace ISpanShop.Services.Admins;

/// <summary>
/// 管理員服務實現 - 處理管理員相關業務邏輯
/// </summary>
public class AdminService : IAdminService
{
	private readonly IAdminRepository _adminRepository;
	private readonly IAdminRoleRepository _roleRepository;

	public AdminService(IAdminRepository adminRepository, IAdminRoleRepository roleRepository)
	{
		_adminRepository = adminRepository ?? throw new ArgumentNullException(nameof(adminRepository));
		_roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
	}

	public IEnumerable<AdminDto> GetAllAdmins()
	{
		return _adminRepository.GetAllAdmins();
	}

	public IEnumerable<AdminPermissionDto> GetAllPermissions()
	{
		return _roleRepository.GetAllPermissions();
	}

	public IEnumerable<AdminLevelDto> GetSelectableAdminLevels()
	{
		return _adminRepository.GetSelectableAdminLevels();
	}

	public (bool IsSuccess, string Message, string GeneratedAccount) CreateAdmin(AdminCreateDto dto)
	{
		try
		{
			// 1. 確認 AdminLevelId 不為 1（不可直接新增超級管理員）
			if (dto.AdminLevelId == 1)
			{
				return (false, "無法直接新增超級管理員", "");
			}

			// 2. 取得流水號
			int seq = _adminRepository.GetNextAdminSequence();

			// 3. 組成帳號與 Email
			string account = $"ADM{seq:D3}";
			string email = $"{account}@ispan.com";

			// 4. 確認帳號不重複
			if (_adminRepository.IsAccountExists(account))
			{
				return (false, "帳號已存在，請稍後重試", "");
			}

			// 5. 角色固定為 Admin
			int roleId = 1;

			// 6. Hash 初始密碼
			string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

			// 7. 呼叫 Repository.CreateAdmin
			bool success = _adminRepository.CreateAdmin(account, email, passwordHash, roleId, dto.AdminLevelId);

			if (success)
			{
				// 8. 回傳 GeneratedAccount 供畫面顯示
				return (true, "管理員新增成功", account);
			}
			else
			{
				return (false, "新增管理員失敗", "");
			}
		}
		catch (Exception ex)
		{
			return (false, $"發生錯誤: {ex.Message}", "");
		}
	}

	public (bool IsSuccess, string Message) DeactivateAdmin(int userId, int currentUserId)
	{
		try
		{
			// 1. userId != currentUserId（不可停用自己）
			if (userId == currentUserId)
			{
				return (false, "無法停用自己的帳號");
			}

			// 2. 若目標是超級管理員，檢查超級管理員數量 > 1
			var targetAdmin = _adminRepository.GetAdminById(userId);
			if (targetAdmin?.AdminLevelId == 1)
			{
				int superAdminCount = _adminRepository.GetSuperAdminCount();
				if (superAdminCount <= 1)
				{
					return (false, "至少需保留一位超級管理員");
				}
			}

			// 3. 呼叫 Repository.DeactivateAdmin
			bool success = _adminRepository.DeactivateAdmin(userId);

			if (success)
			{
				return (true, "管理員已停用");
			}
			else
			{
				return (false, "停用管理員失敗");
			}
		}
		catch (Exception ex)
		{
			return (false, $"發生錯誤: {ex.Message}");
		}
	}

	public AdminDto? VerifyLogin(string account, string password)
	{
		try
		{
			// 1. 呼叫 GetAdminByAccount(account)
			var admin = _adminRepository.GetAdminByAccount(account);

			// 2. 帳號不存在 → 回傳 null
			if (admin == null)
			{
				return null;
			}

			// 3. BCrypt.Verify(password, admin.PasswordHash)
			if (!BCrypt.Net.BCrypt.Verify(password, admin.PasswordHash))
			{
				// 5. 驗證失敗 → 回傳 null
				return null;
			}

			// 4. 驗證成功 → 回傳 AdminDto（含 IsFirstLogin）
			return admin;
		}
		catch (Exception)
		{
			return null;
		}
	}

	public (bool IsSuccess, string Message) ChangePassword(AdminChangePasswordDto dto)
	{
		try
		{
			// 1. NewPassword == ConfirmPassword（由 DataAnnotation 在 Model 驗證，此為備用檢查）
			if (dto.NewPassword != dto.ConfirmPassword)
			{
				return (false, "兩次密碼輸入不一致");
			}

			// 2. 新密碼至少 8 碼且含英文與數字
			if (dto.NewPassword.Length < 8)
			{
				return (false, "新密碼至少需 8 個字元");
			}

			if (!Regex.IsMatch(dto.NewPassword, @"[a-zA-Z]"))
			{
				return (false, "新密碼必須包含英文字母");
			}

			if (!Regex.IsMatch(dto.NewPassword, @"[0-9]"))
			{
				return (false, "新密碼必須包含數字");
			}

			// 3. Hash 新密碼
			string newPasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

			// 4. 呼叫 Repository.ChangePassword()
			bool changeSuccess = _adminRepository.ChangePassword(dto.UserId, newPasswordHash);

			if (!changeSuccess)
			{
				return (false, "變更密碼失敗");
			}

			// 5. 呼叫 Repository.SetFirstLoginComplete()
			_adminRepository.SetFirstLoginComplete(dto.UserId);

			return (true, "密碼變更成功");
		}
		catch (Exception ex)
		{
			return (false, $"發生錯誤: {ex.Message}");
		}
	}

	public bool UpdateAdminRole(int adminId, int roleId, int currentAdminId)
	{
		if (adminId == currentAdminId)
		{
			throw new InvalidOperationException("無法修改您自己的角色。請聯繫其他管理員協助。");
		}
		return _adminRepository.UpdateAdminRole(adminId, roleId);
	}
}