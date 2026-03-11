using ISpanShop.Models.DTOs.Support;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISpanShop.Services.Support
{
	public interface ISupportTicketService
	{
		// 取得所有工單列表
		Task<List<SupportTicketDto>> GetAllAsync();

		// 管理員回覆工單並結案
		Task ReplyAndCloseTicketAsync(int id, string adminReply);

		Task<SupportTicketDto> GetTicketDetailsAsync(int id);
	}
}