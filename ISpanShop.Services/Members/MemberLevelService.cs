using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ISpanShop.Models.DTOs.Members;
using ISpanShop.Models.EfModels;
using Microsoft.EntityFrameworkCore;

namespace ISpanShop.Services.Members
{
    public class MemberLevelService : IMemberLevelService
    {
        private readonly ISpanShopDBContext _context;

        public MemberLevelService(ISpanShopDBContext context)
        {
            _context = context;
        }

        public async Task<MemberLevelDto> GetMemberLevelInfoAsync(int userId)
        {
            var now = DateTime.Now;
            var oneYearAgo = now.AddYears(-1);

            // 1. 動態加總過去 12 個月內已完成的訂單金額
            var totalSpending = await _context.Orders
                .Where(o => o.UserId == userId && o.Status == 3 && o.CompletedAt >= oneYearAgo)
                .SumAsync(o => o.FinalAmount);

            // 2. 取得所有等級規則
            var allLevels = await _context.MembershipLevels
                .OrderBy(l => l.MinSpending)
                .ToListAsync();

            // 3. 找出目前所屬等級 (取符合 MinSpending 門檻且金額最高的那一級)
            var currentLevel = allLevels
                .Where(l => totalSpending >= l.MinSpending)
                .OrderByDescending(l => l.MinSpending)
                .FirstOrDefault() ?? allLevels.First();

            // 4. 找出下一級
            var nextLevel = allLevels
                .Where(l => l.MinSpending > currentLevel.MinSpending)
                .OrderBy(l => l.MinSpending)
                .FirstOrDefault();

            // 5. 同步回 MemberProfile (快取機制)
            var profile = await _context.MemberProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (profile != null)
            {
                profile.TotalSpending = totalSpending;
                profile.LevelId = currentLevel.Id;
                profile.UpdatedAt = now;
                await _context.SaveChangesAsync();
            }

            // 6. 封裝 DTO
            var dto = new MemberLevelDto
            {
                UserId = userId,
                CurrentLevelName = currentLevel.LevelName,
                CurrentTotalSpending = totalSpending,
                CalculationStartDate = oneYearAgo,
                CalculationEndDate = now,
                AllLevels = allLevels.Select(l => new LevelRuleDto
                {
                    LevelId = l.Id,
                    Name = l.LevelName,
                    MinSpending = l.MinSpending,
                    DiscountRate = l.DiscountRate
                }).ToList()
            };

            if (nextLevel != null)
            {
                dto.NextLevelName = nextLevel.LevelName;
                dto.NextLevelThreshold = nextLevel.MinSpending;
                // 計算進度百分比 (目前消費額 / 下一級門檻)
                dto.ProgressPercent = Math.Min(Math.Round((totalSpending / nextLevel.MinSpending) * 100, 1), 100);
            }
            else
            {
                // 已達到最高等級
                dto.NextLevelName = "最高等級";
                dto.NextLevelThreshold = currentLevel.MinSpending;
                dto.ProgressPercent = 100;
            }

            return dto;
        }

        public async Task UpdateMemberLevelAsync(int userId)
        {
            // 重用邏輯進行單純的資料同步
            await GetMemberLevelInfoAsync(userId);
        }
    }
}
