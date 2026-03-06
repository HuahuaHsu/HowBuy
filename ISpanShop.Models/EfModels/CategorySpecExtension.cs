namespace ISpanShop.Models.EfModels;

/// <summary>
/// 手動擴充 CategorySpec（避免直接修改 EF Core Power Tools 自動生成的檔案）
/// </summary>
public partial class CategorySpec
{
    /// <summary>允許賣家在預設選項清單外自行輸入選項（僅適用於 select/checkbox/radio 類型）</summary>
    public bool AllowCustomInput { get; set; }
}
