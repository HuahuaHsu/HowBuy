using System.Threading.Tasks;
using ISpanShop.Models.DTOs.Members;

namespace ISpanShop.Services.Members
{
    public interface IMemberLevelService
    {
        Task<MemberLevelDto> GetMemberLevelInfoAsync(int userId);
        Task UpdateMemberLevelAsync(int userId); // 用於排程或手動更新
    }
}
