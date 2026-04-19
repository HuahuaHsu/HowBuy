using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ISpanShop.Models.EfModels;

namespace ISpanShop.Repositories.Communication
{
    public interface IChatRepository
    {
        Task<int> GetUnreadCountAsync(int receiverId);
        Task MarkAsReadAsync(int senderId, int receiverId);
        Task AddMessageAsync(ChatMessage message);
        Task<List<ChatMessage>> GetChatHistoryAsync(int user1Id, int user2Id);
        Task<List<dynamic>> GetChatSessionsAsync(int userId);
    }

    public class ChatRepository : IChatRepository
    {
        private readonly ISpanShopDBContext _context;

        public ChatRepository(ISpanShopDBContext context)
        {
            _context = context;
        }

        public async Task<int> GetUnreadCountAsync(int receiverId)
        {
            return await _context.ChatMessages
                .Where(m => m.ReceiverId == receiverId && m.IsRead == false)
                .CountAsync();
        }

        public async Task MarkAsReadAsync(int senderId, int receiverId)
        {
            var unreadMessages = await _context.ChatMessages
                .Where(m => m.SenderId == senderId && m.ReceiverId == receiverId && m.IsRead == false)
                .ToListAsync();

            if (unreadMessages.Any())
            {
                foreach (var msg in unreadMessages)
                {
                    msg.IsRead = true;
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddMessageAsync(ChatMessage message)
        {
            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ChatMessage>> GetChatHistoryAsync(int user1Id, int user2Id)
        {
            return await _context.ChatMessages
                .Where(m => (m.SenderId == user1Id && m.ReceiverId == user2Id) ||
                            (m.SenderId == user2Id && m.ReceiverId == user1Id))
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<List<dynamic>> GetChatSessionsAsync(int userId)
        {
            // 找出所有與該使用者有關的訊息
            var allMessages = await _context.ChatMessages
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();

            // 依對象分組，取得最後一則訊息與未讀數
            var sessions = allMessages
                .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                .Select(g => new
                {
                    OtherUserId = g.Key,
                    LastMessage = g.First().Content,
                    SentAt = g.First().SentAt,
                    UnreadCount = g.Count(m => m.ReceiverId == userId && m.IsRead == false)
                })
                .ToList<dynamic>();

            return sessions;
        }
    }
}
