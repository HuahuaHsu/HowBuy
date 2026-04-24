using System;
using System.Collections.Generic;

namespace ISpanShop.Models.DTOs.Members
{
    public class MemberLevelDto
    {
        public int UserId { get; set; }
        public string CurrentLevelName { get; set; }
        public decimal CurrentTotalSpending { get; set; }
        public decimal NextLevelThreshold { get; set; }
        public string NextLevelName { get; set; }
        public decimal ProgressPercent { get; set; }
        public DateTime CalculationStartDate { get; set; }
        public DateTime CalculationEndDate { get; set; }
        public List<LevelRuleDto> AllLevels { get; set; } = new List<LevelRuleDto>();
    }

    public class LevelRuleDto
    {
        public int LevelId { get; set; }
        public string Name { get; set; }
        public decimal MinSpending { get; set; }
        public decimal DiscountRate { get; set; }
    }
}
