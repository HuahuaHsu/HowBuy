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
        
        // 暫時記錄 fuen50 的 ID，用於模擬賣家
        private static string _simulatedSellerId = null;

        public ChatHub(IChatService chatService, ILogger<ChatHub> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }

        // 當客戶端發送訊息時呼叫此方法
        public async Task SendMessage(int receiverId, string content, byte type)
        {
            var senderIdStr = Context.UserIdentifier;
            
            if (int.TryParse(senderIdStr, out int senderId))
            {
                // --- 核心模擬邏輯：將買家訊息導向 fuen50 ---
                int finalReceiverId = receiverId;
                
                // 如果 fuen50 已經上線，且目前的接收者不是 fuen49 本人
                // 我們就將接收者強制改為 fuen50，這樣訊息就會存入 fuen50 的名下
                if (!string.IsNullOrEmpty(_simulatedSellerId) && int.TryParse(_simulatedSellerId, out int sellerId))
                {
                    // 如果我是買家 (senderId != sellerId)，就把訊息傳給 sellerId
                    if (senderId != sellerId)
                    {
                        finalReceiverId = sellerId;
                    }
                }

                // 1. 執行正規的發送流程 (存入資料庫)
                await _chatService.SendMessageAsync(senderId, finalReceiverId, content, type);

                // 2. 傳送給接收者
                await Clients.User(finalReceiverId.ToString()).SendAsync("ReceiveMessage", senderId, content, type);
                
                // 3. 也傳送給發送者本人 (讓自己的畫面出現訊息泡泡)
                await Clients.Caller.SendAsync("ReceiveMessage", senderId, content, type);

                _logger.LogInformation($"ChatHub: Message sent from {senderId} to {finalReceiverId} (Simulation Directed).");
            }
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            
            // 偵測是否為 fuen50 登入 (這需要從 User.Identity.Name 判斷)
            var userName = Context.User?.Identity?.Name;
            _logger.LogInformation($"ChatHub: User {userName} (ID: {userId}) connected.");

            // 我們在此暫時假設所有登入的使用者，如果他進入了對話列表，且我們想測試賣家
            // 只要他在右邊視窗連線，我們就記錄他的 ID 作為接收目標
            // 您可以透過 Console 看一下 ID 是多少
            
            // 暫時讓最後一個連上線的人 (非買家本人) 成為模擬賣家
            if (userId != null) 
            {
                _simulatedSellerId = userId;
            }

            await base.OnConnectedAsync();
        }
    }
}
