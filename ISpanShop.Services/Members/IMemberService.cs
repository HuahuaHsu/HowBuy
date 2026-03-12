using ISpanShop.Models.DTOs.Members;
using System.Collections.Generic;

namespace ISpanShop.Services.Members
{
	/// <summary>
	/// 會員服務介面 - 處理會員相關業務邏輯
	/// </summary>
	public interface IMemberService
	{
		/// <summary>
		/// 搜尋會員（支援關鍵字、狀態篩選）
		/// </summary>
		/// <param name="keyword">關鍵字（帳號、姓名、Email、電話）</param>
		/// <param name="status">狀態（normal, blocked）</param>
		/// <returns>會員 DTO 列表</returns>
		IEnumerable<MemberDto> Search(string keyword, string status);

		/// <summary>
		/// 根據 ID 取得會員詳細資料
		/// </summary>
		/// <param name="id">會員 ID</param>
		/// <returns>會員 DTO</returns>
		MemberDto GetMemberById(int id);

		/// <summary>
		/// 更新會員狀態（如黑名單）
		/// </summary>
		/// <param name="dto">會員 DTO</param>
		void UpdateMemberStatus(MemberDto dto);

		/// <summary>
		/// 更新會員詳細資料
		/// </summary>
		/// <param name="dto">會員 DTO</param>
		void UpdateMemberProfile(MemberDto dto);
	}
}
