using ISpanShop.Models.DTOs.Members;
using ISpanShop.Models.EfModels;
using ISpanShop.Repositories.Members;
using System.Collections.Generic;
using System.Linq;
using System;

namespace ISpanShop.Services.Members
{
	/// <summary>
	/// 會員服務實現 - 處理會員相關業務邏輯
	/// </summary>
	public class MemberService : IMemberService
	{
		private readonly IMemberRepository _repo;

		public MemberService(IMemberRepository repo)
		{
			_repo = repo ?? throw new ArgumentNullException(nameof(repo));
		}

		public IEnumerable<MemberDto> Search(string keyword, string status)
		{
			var users = _repo.Search(keyword, status);

			return users.Select(u => MapToDto(u));
		}

		public MemberDto GetMemberById(int id)
		{
			var user = _repo.GetById(id);
			if (user == null) return null;

			return MapToDto(user);
		}

		public void UpdateMemberStatus(MemberDto dto)
		{
			var userInDb = _repo.GetById(dto.Id);
			if (userInDb == null) throw new Exception("找不到該會員");

			userInDb.IsBlacklisted = dto.IsBlacklisted;
			_repo.Update(userInDb);
		}

		public void UpdateMemberProfile(MemberDto dto)
		{
			var userInDb = _repo.GetById(dto.Id);
			if (userInDb == null) throw new Exception("找不到該會員");

			// 更新 Users 表中的基本資訊
			userInDb.Email = dto.Email;
			userInDb.IsBlacklisted = dto.IsBlacklisted;

			// 更新 MemberProfiles 表
			if (userInDb.MemberProfile != null)
			{
				userInDb.MemberProfile.FullName = dto.FullName;
				userInDb.MemberProfile.PhoneNumber = dto.PhoneNumber;
			}

			// 更新 Addresses 表中的預設地址
			var defaultAddress = userInDb.Addresses.FirstOrDefault(a => a.IsDefault == true)
								  ?? userInDb.Addresses.FirstOrDefault();
			if (defaultAddress != null)
			{
				defaultAddress.City = dto.City;
				defaultAddress.Region = dto.Region;
				defaultAddress.Street = dto.Address; // 對應 Street 欄位
			}

			_repo.Update(userInDb);
		}

		private MemberDto MapToDto(User u)
		{
			var profile = u.MemberProfile;
			var address = u.Addresses.FirstOrDefault(a => a.IsDefault == true)
						  ?? u.Addresses.FirstOrDefault();

			return new MemberDto
			{
				Id = u.Id,
				Account = u.Account,
				Email = u.Email,
				IsBlacklisted = u.IsBlacklisted ?? false,
				//IsSeller = u.IsSeller ?? false,
				RoleName = u.Role?.RoleName,

				FullName = profile?.FullName ?? "未設定",
				PhoneNumber = profile?.PhoneNumber ?? "未設定",
				PointBalance = profile?.PointBalance ?? 0,

				// 這裡改成單數 LevelName
				LevelName = profile?.Level?.LevelName ?? "一般會員",

				// 如果有預設頭像 URL 生成邏輯
				//AvatarUrl = $"https://ui-avatars.com/api/?name={profile?.FullName ?? u.Account}&background=random&color=fff",

				City = address?.City ?? "",
				Region = address?.Region ?? "",
				Address = address?.Street ?? ""
			};
		}
	}
}
