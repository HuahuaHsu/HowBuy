using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ISpanShop.Services.Communication;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ISpanShop.MVC.Hubs
{
    [Authorize(AuthenticationSchemes = "FrontendJwt")]
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(IChatService chatService, ILogger<ChatHub> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }

        // 當客戶端發送訊息時呼叫此方法
        public async Task SendMessage(int receiverId, string content, byte type)
        {
            var senderIdStr = Context.UserIdentifier;
            
            _logger.LogInformation($"ChatHub: Attempting to send message from {senderIdStr} to {receiverId}");

            if (int.TryParse(senderIdStr, out int senderId))
            {
                // 1. 執行正規的發送流程 (存入資料庫)
                await _chatService.SendMessageAsync(senderId, receiverId, content, type);

                // 2. 暫時模擬邏輯：改為全站廣播 (Broadcast)
                // 這樣不論 receiverId 是誰，fuen49 和 fuen50 只要開著網頁，
                // 都能接收到對方的訊息，從而模擬雙向溝通。
                await Clients.All.SendAsync("ReceiveMessage", senderId, content, type);
                
                _logger.LogInformation($"[Simulation Mode] Message broadcasted from {senderId} to All (intended for {receiverId}).");
            }
            else
            {
                _logger.LogWarning("ChatHub: Could not identify sender ID.");
            }
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation($"ChatHub: User connected - {Context.UserIdentifier}");
            await base.OnConnectedAsync();
        }
    }
}
