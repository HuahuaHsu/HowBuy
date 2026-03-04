# ISpanShop 專案目前程式碼狀態

### 檔案: .\ISpanShop.Models\DTOs\CategoryDto.cs
`${ext}
namespace ISpanShop.Models.DTOs
{
    /// <summary>
    /// 分類 DTO - 用於前端下拉選單
    /// </summary>
    public class CategoryDto
    {
        /// <summary>
        /// 分類 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 分類名稱
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 父分類 ID（null 表示為主分類）
        /// </summary>
        public int? ParentId { get; set; }
    }
}

``n
### 檔案: .\ISpanShop.Models\DTOs\CategorySpecDto.cs
`${ext}
using System.Collections.Generic;

namespace ISpanShop.Models.DTOs
{
    /// <summary>
    /// 分類規格 DTO - 用於資料傳輸與顯示
    /// </summary>
    public class CategorySpecDto
    {
        /// <summary>
        /// 規格 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 規格名稱 (例如：顏色、尺寸、容量)
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 輸入方式 (text=文字輸入, select=下拉選單, checkbox=多選框, radio=單選框)
        /// </summary>
        public string InputType { get; set; } = string.Empty;

        /// <summary>
        /// 是否為必填項目
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// 排序順序
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// 選項名稱列表 (僅當 InputType 為 select/checkbox/radio 時有值)
        /// </summary>
        public List<string> Options { get; set; } = new List<string>();
    }
}

``n
### 檔案: .\ISpanShop.Models\DTOs\PagedResult.cs
`${ext}
using System;
using System.Collections.Generic;

namespace ISpanShop.Models.DTOs
{
    /// <summary>
    /// 泛型分頁結果物件
    /// </summary>
    /// <typeparam name="T">資料型別</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// 當頁資料
        /// </summary>
        public List<T> Data { get; set; } = new List<T>();

        /// <summary>
        /// 總頁數
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// 目前頁碼
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// 總筆數
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 每頁筆數
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 建立分頁結果
        /// </summary>
        public static PagedResult<T> Create(List<T> data, int totalCount, int pageNumber, int pageSize)
        {
            return new PagedResult<T>
            {
                Data = data,
                TotalCount = totalCount,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }
    }
}

``n
### 檔案: .\ISpanShop.Models\DTOs\ProductBatchUpdateStatusDto.cs
`${ext}
using System.Collections.Generic;

namespace ISpanShop.Models.DTOs
{
    /// <summary>
    /// 批次更新商品狀態 DTO
    /// </summary>
    public class ProductBatchUpdateStatusDto
    {
        /// <summary>
        /// 要更新的商品 ID 集合
        /// </summary>
        public List<int> ProductIds { get; set; } = new List<int>();

        /// <summary>
        /// 目標狀態：1 為上架，0 為下架
        /// </summary>
        public byte TargetStatus { get; set; }
    }
}

``n
### 檔案: .\ISpanShop.Models\DTOs\ProductCreateDto.cs
`${ext}
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ISpanShop.Models.DTOs
{
    /// <summary>
    /// 新增商品用 DTO - 接收完整的商品建立資料
    /// </summary>
    public class ProductCreateDto
    {
        /// <summary>
        /// 店鋪 ID
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// 分類 ID
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// 品牌 ID
        /// </summary>
        public int BrandId { get; set; }

        /// <summary>
        /// 商品名稱 - 必填
        /// </summary>
        [Required(ErrorMessage = "商品名稱為必填項")]
        public required string Name { get; set; }

        /// <summary>
        /// 商品描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 影片 URL
        /// </summary>
        public string? VideoUrl { get; set; }

        /// <summary>
        /// 規格定義 JSON
        /// </summary>
        public required string SpecDefinitionJson { get; set; }

        /// <summary>
        /// 規格變體集合 - 用於接收多筆規格
        /// </summary>
        public List<ProductVariantCreateDto> Variants { get; set; } = new List<ProductVariantCreateDto>();

        /// <summary>
        /// 上傳的商品圖片集合 - 用於接收多張圖片
        /// </summary>
        public List<IFormFile> UploadImages { get; set; } = new List<IFormFile>();

        /// <summary>
        /// 主圖索引 - 紀錄在圖片集合中哪一張是主圖
        /// </summary>
        public int MainImageIndex { get; set; }
    }
}

``n
### 檔案: .\ISpanShop.Models\DTOs\ProductDetailDto.cs
`${ext}
using System.Collections.Generic;

namespace ISpanShop.Models.DTOs
{
    /// <summary>
    /// 商品詳情 DTO - 用於商品詳情頁展示（包含完整的圖片與規格）
    /// </summary>
    public class ProductDetailDto
    {
        /// <summary>
        /// 商品 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 商品名稱
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// 商店名稱
        /// </summary>
        public string? StoreName { get; set; }

        /// <summary>
        /// 分類名稱
        /// </summary>
        public string? CategoryName { get; set; }

        /// <summary>
        /// 品牌名稱
        /// </summary>
        public string? BrandName { get; set; }

        /// <summary>
        /// 商品描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 商品狀態 (1=已上架, 2=待審核, 0=下架)
        /// </summary>
        public byte? Status { get; set; }

        /// <summary>
        /// 商品圖片 URL 列表
        /// </summary>
        public List<string> Images { get; set; } = new();

        /// <summary>
        /// 商品規格變體列表
        /// </summary>
        public List<ProductVariantDetailDto> Variants { get; set; } = new();
    }
}

``n
### 檔案: .\ISpanShop.Models\DTOs\ProductListDto.cs
`${ext}
using System;

namespace ISpanShop.Models.DTOs
{
    /// <summary>
    /// 商品列表 DTO - Service 層回傳的資料轉移物件
    /// </summary>
    public class ProductListDto
    {
        /// <summary>
        /// 商品 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 商店名稱
        /// </summary>
        public string? StoreName { get; set; }

        /// <summary>
        /// 分類名稱
        /// </summary>
        public required string CategoryName { get; set; }

        /// <summary>
        /// 品牌名稱
        /// </summary>
        public string? BrandName { get; set; }

        /// <summary>
        /// 商品名稱
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// 最低價格
        /// </summary>
        public decimal? MinPrice { get; set; }

        /// <summary>
        /// 最高價格
        /// </summary>
        public decimal? MaxPrice { get; set; }

        /// <summary>
        /// 商品狀態 (1=已上架, 2=待審核, 0=下架)
        /// </summary>
        public byte? Status { get; set; }

        /// <summary>
        /// 主圖 URL
        /// </summary>
        public required string MainImageUrl { get; set; }

        /// <summary>
        /// 建檔日期
        /// </summary>
        public DateTime? CreatedAt { get; set; }
    }
}

``n
### 檔案: .\ISpanShop.Models\DTOs\ProductReviewDto.cs
`${ext}
using System;

namespace ISpanShop.Models.DTOs
{
    /// <summary>
    /// 商品審核 DTO - Service 層回傳的資料轉移物件
    /// </summary>
    public class ProductReviewDto
    {
        /// <summary>
        /// 商品 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 店鋪 ID
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// 分類名稱
        /// </summary>
        public required string CategoryName { get; set; }

        /// <summary>
        /// 品牌名稱
        /// </summary>
        public string? BrandName { get; set; }

        /// <summary>
        /// 商店名稱
        /// </summary>
        public string? StoreName { get; set; }

        /// <summary>
        /// 商品名稱
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// 商品描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 商品狀態 (0=下架, 1=上架, 2=待審核, 3=審核退回)
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// 退回原因（Status==3 時填入）
        /// </summary>
        public string? RejectReason { get; set; }

        /// <summary>
        /// 最後更新時間（退回時間）
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// 主圖 URL
        /// </summary>
        public string? MainImageUrl { get; set; }
    }
}

``n
### 檔案: .\ISpanShop.Models\DTOs\ProductSearchCriteria.cs
`${ext}
using System;

namespace ISpanShop.Models.DTOs
{
    /// <summary>
    /// 商品搜尋條件 - 用於分頁與多維度篩選
    /// </summary>
    public class ProductSearchCriteria
    {
        /// <summary>
        /// 主分類 ID（若有值，撈該主分類下所有子分類的商品）
        /// </summary>
        public int? ParentCategoryId { get; set; }

        /// <summary>
        /// 子分類 ID（若有值，優先以此篩選）
        /// </summary>
        public int? CategoryId { get; set; }

        /// <summary>
        /// 關鍵字搜尋（搜尋商品名稱或描述）
        /// </summary>
        public string? Keyword { get; set; }

        /// <summary>
        /// 品牌 ID
        /// </summary>
        public int? BrandId { get; set; }

        /// <summary>
        /// 商家 ID
        /// </summary>
        public int? StoreId { get; set; }

        /// <summary>
        /// 商品狀態（例如 1:已上架, 0:未上架/下架）
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 建檔日期起（含當天）
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 建檔日期迄（含當天整天）
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 頁碼（從 1 開始）
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// 每頁筆數
        /// </summary>
        public int PageSize { get; set; } = 10;
    }
}

``n
### 檔案: .\ISpanShop.Models\DTOs\ProductVariantCreateDto.cs
`${ext}
namespace ISpanShop.Models.DTOs
{
    /// <summary>
    /// 商品規格變體新增用 DTO - 用於接收規格變體資訊
    /// </summary>
    public class ProductVariantCreateDto
    {
        /// <summary>
        /// SKU 代碼
        /// </summary>
        public string? SkuCode { get; set; }

        /// <summary>
        /// 規格名稱
        /// </summary>
        public required string VariantName { get; set; }

        /// <summary>
        /// 規格值 JSON - 儲存規格屬性和對應值
        /// </summary>
        public required string SpecValueJson { get; set; }

        /// <summary>
        /// 價格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 庫存數量
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        /// 安全庫存 - 低於此數量時需要補貨
        /// </summary>
        public int SafetyStock { get; set; }
    }
}

``n
### 檔案: .\ISpanShop.Models\DTOs\ProductVariantDetailDto.cs
`${ext}
namespace ISpanShop.Models.DTOs
{
    /// <summary>
    /// 商品規格詳情 DTO - 用於商品詳情頁展示
    /// </summary>
    public class ProductVariantDetailDto
    {
        /// <summary>
        /// SKU 代碼
        /// </summary>
        public required string SkuCode { get; set; }

        /// <summary>
        /// 規格名稱
        /// </summary>
        public required string VariantName { get; set; }

        /// <summary>
        /// 價格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 庫存量
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        /// 規格值 JSON (如 {"color":"黑","size":"M"})
        /// </summary>
        public string? SpecValueJson { get; set; }
    }
}

``n
### 檔案: .\ISpanShop.Models\EfModels\Address.cs
`${ext}
// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ISpanShop.Models.EfModels;

public partial class Address
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string RecipientName { get; set; }

    public string RecipientPhone { get; set; }

    public string City { get; set; }

    public string Region { get; set; }

    public string Street { get; set; }

    public bool? IsDefault { get; set; }

    public virtual User User { get; set; }
}
``n
### 檔案: .\ISpanShop.Models\EfModels\Attribute.cs
`${ext}
// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ISpanShop.Models.EfModels;

public partial class Attribute
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string InputType { get; set; }

    public bool IsRequired { get; set; }

    public int SortOrder { get; set; }

    public virtual ICollection<AttributeOption> AttributeOptions { get; set; } = new List<AttributeOption>();

    public virtual ICollection<CategoryAttribute> CategoryAttributes { get; set; } = new List<CategoryAttribute>();
}
``n
### 檔案: .\ISpanShop.Models\EfModels\AttributeOption.cs
`${ext}
// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ISpanShop.Models.EfModels;

public partial class AttributeOption
{
    public int Id { get; set; }

    public int AttributeId { get; set; }

    public string OptionName { get; set; }

    public int SortOrder { get; set; }

    public virtual Attribute Attribute { get; set; }
}
``n
### 檔案: .\ISpanShop.Models\EfModels\BlacklistRecord.cs
`${ext}
// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ISpanShop.Models.EfModels;

public partial class BlacklistRecord
{
    public int Id { get; set; }

    public int BlockedUserId { get; set; }

    public int AdminUserId { get; set; }

    public string Reason { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UnblockAt { get; set; }

    public virtual User AdminUser { get; set; }

    public virtual User BlockedUser { get; set; }
}
``n
### 檔案: .\ISpanShop.Models\EfModels\Brand.cs
`${ext}
// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ISpanShop.Models.EfModels;

public partial class Brand
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string LogoUrl { get; set; }

    public int? Sort { get; set; }

    public bool? IsVisible { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
``n
### 檔案: .\ISpanShop.Models\EfModels\Cart.cs
`${ext}
// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ISpanShop.Models.EfModels;

public partial class Cart
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual User User { get; set; }
}
``n
### 檔案: .\ISpanShop.Models\EfModels\CartItem.cs
`${ext}
// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ISpanShop.Models.EfModels;

public partial class CartItem
{
    public int Id { get; set; }

    public int CartId { get; set; }

    public int StoreId { get; set; }

    public int ProductId { get; set; }

    public int VariantId { get; set; }

    public int Quantity { get; set; }

    public decimal? UnitPrice { get; set; }

    public virtual Cart Cart { get; set; }

    public virtual Product Product { get; set; }

    public virtual Store Store { get; set; }

    public virtual ProductVariant Variant { get; set; }
}
``n
### 檔案: .\ISpanShop.Models\EfModels\Category.cs
`${ext}
// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ISpanShop.Models.EfModels;

public partial class Category
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int? ParentId { get; set; }

    public string IconUrl { get; set; }

    public int? Sort { get; set; }

    public bool? IsVisible { get; set; }

    public virtual ICollection<CategoryAttribute> CategoryAttributes { get; set; } = new List<CategoryAttribute>();

    public virtual ICollection<Category> InverseParent { get; set; } = new List<Category>();

    public virtual Category Parent { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
``n
### 檔案: .\ISpanShop.Models\EfModels\Category.Partial.cs
`${ext}
using System.ComponentModel.DataAnnotations.Schema;

namespace ISpanShop.EfModels
{
    /// <summary>
    /// Category 實體的擴充部分 - 不修改資料庫
    /// </summary>
    public partial class Category
    {
        /// <summary>
        /// 預設規格 JSON - 不對應到資料庫
        /// </summary>
        [NotMapped]
        public string DefaultSpecJson { get; set; }
    }
}

``n
### 檔案: .\ISpanShop.Models\EfModels\CategoryAttribute.cs
`${ext}
// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ISpanShop.Models.EfModels;

public partial class CategoryAttribute
{
    public int CategoryId { get; set; }

    public int AttributeId { get; set; }

    public bool IsFilterable { get; set; }

    public virtual Attribute Attribute { get; set; }

    public virtual Category Category { get; set; }
}
``n
### 檔案: .\ISpanShop.Models\EfModels\ChatMessage.cs
`${ext}
// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ISpanShop.Models.EfModels;

public partial class ChatMessage
{
    public long Id { get; set; }

    public Guid SessionId { get; set; }

    public int SenderId { get; set; }

    public int ReceiverId { get; set; }

    public string Content { get; set; }

    public byte Type { get; set; }

    public bool? IsRead { get; set; }

    public DateTime? SentAt { get; set; }

    public virtual User Receiver { get; set; }

    public virtual User Sender { get; set; }
}
``n
### 檔案: .\ISpanShop.Models\EfModels\ISpanShopDBContext.cs
`${ext}
// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ISpanShop.Models.EfModels;

public partial class ISpanShopDBContext : DbContext
{
    public ISpanShopDBContext(DbContextOptions<ISpanShopDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Attribute> Attributes { get; set; }

    public virtual DbSet<AttributeOption> AttributeOptions { get; set; }

    public virtual DbSet<BlacklistRecord> BlacklistRecords { get; set; }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<CategoryAttribute> CategoryAttributes { get; set; }

    public virtual DbSet<ChatMessage> ChatMessages { get; set; }

    public virtual DbSet<LoginHistory> LoginHistories { get; set; }

    public virtual DbSet<MemberProfile> MemberProfiles { get; set; }

    public virtual DbSet<MembershipLevel> MembershipLevels { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<OrderReview> OrderReviews { get; set; }

    public virtual DbSet<PaymentLog> PaymentLogs { get; set; }

    public virtual DbSet<PointHistory> PointHistories { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductImage> ProductImages { get; set; }

    public virtual DbSet<ProductVariant> ProductVariants { get; set; }

    public virtual DbSet<ReviewImage> ReviewImages { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SensitiveWord> SensitiveWords { get; set; }

    public virtual DbSet<Store> Stores { get; set; }

    public virtual DbSet<SupportTicket> SupportTickets { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Addresse__3214EC07B7F22715");

            entity.Property(e => e.City).HasMaxLength(20);
            entity.Property(e => e.IsDefault).HasDefaultValue(false);
            entity.Property(e => e.RecipientName).HasMaxLength(50);
            entity.Property(e => e.RecipientPhone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Region).HasMaxLength(20);
            entity.Property(e => e.Street).HasMaxLength(200);

            entity.HasOne(d => d.User).WithMany(p => p.Addresses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Addresses_Users");
        });

        modelBuilder.Entity<Attribute>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Attribut__3214EC07757FC215");

            entity.Property(e => e.InputType)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("text");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);
        });

        modelBuilder.Entity<AttributeOption>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Attribut__3214EC07AD9FB113");

            entity.Property(e => e.OptionName)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(d => d.Attribute).WithMany(p => p.AttributeOptions)
                .HasForeignKey(d => d.AttributeId)
                .HasConstraintName("FK_AttributeOptions_Attributes");
        });

        modelBuilder.Entity<BlacklistRecord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Blacklis__3214EC07299EA7B3");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Reason).HasMaxLength(200);
            entity.Property(e => e.UnblockAt).HasColumnType("datetime");

            entity.HasOne(d => d.AdminUser).WithMany(p => p.BlacklistRecordAdminUsers)
                .HasForeignKey(d => d.AdminUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BlacklistRecords_AdminUser");

            entity.HasOne(d => d.BlockedUser).WithMany(p => p.BlacklistRecordBlockedUsers)
                .HasForeignKey(d => d.BlockedUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BlacklistRecords_BlockedUser");
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Brands__3214EC0714593EBF");

            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.IsVisible).HasDefaultValue(true);
            entity.Property(e => e.LogoUrl).HasMaxLength(500);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Sort).HasDefaultValue(0);
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Carts__3214EC0704FD1DFC");

            entity.HasIndex(e => e.UserId, "UQ__Carts__1788CC4D65D51340").IsUnique();

            entity.HasOne(d => d.User).WithOne(p => p.Cart)
                .HasForeignKey<Cart>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Carts_Users");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CartItem__3214EC07E996E59A");

            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.CartId)
                .HasConstraintName("FK_CartItems_Carts");

            entity.HasOne(d => d.Product).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CartItems_Products");

            entity.HasOne(d => d.Store).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.StoreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CartItems_Stores");

            entity.HasOne(d => d.Variant).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.VariantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CartItems_ProductVariants");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC07802F57AF");

            entity.Property(e => e.IconUrl).HasMaxLength(500);
            entity.Property(e => e.IsVisible).HasDefaultValue(true);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Sort).HasDefaultValue(0);

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK_Categories_Parent");
        });

        modelBuilder.Entity<CategoryAttribute>(entity =>
        {
            entity.HasKey(e => new { e.CategoryId, e.AttributeId }).HasName("PK__Category__A511A89582BA8358");

            entity.HasOne(d => d.Attribute).WithMany(p => p.CategoryAttributes)
                .HasForeignKey(d => d.AttributeId)
                .HasConstraintName("FK_CategoryAttributes_Attributes");

            entity.HasOne(d => d.Category).WithMany(p => p.CategoryAttributes)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_CategoryAttributes_Categories");
        });

        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChatMess__3214EC07F09550F3");

            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.SentAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Receiver).WithMany(p => p.ChatMessageReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChatMessages_Receiver");

            entity.HasOne(d => d.Sender).WithMany(p => p.ChatMessageSenders)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChatMessages_Sender");
        });

        modelBuilder.Entity<LoginHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LoginHis__3214EC0742469E23");

            entity.ToTable("LoginHistory");

            entity.Property(e => e.Ipaddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("IPAddress");
            entity.Property(e => e.LoginTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.LoginHistories)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LoginHistory_Users");
        });

        modelBuilder.Entity<MemberProfile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MemberPr__3214EC07E5784F2E");

            entity.HasIndex(e => e.UserId, "UQ__MemberPr__1788CC4D10D292CC").IsUnique();

            entity.Property(e => e.EmailNotification).HasDefaultValue(true);
            entity.Property(e => e.FullName).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PointBalance).HasDefaultValue(0);
            entity.Property(e => e.TotalSpending)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Level).WithMany(p => p.MemberProfiles)
                .HasForeignKey(d => d.LevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MemberProfiles_MembershipLevels");

            entity.HasOne(d => d.User).WithOne(p => p.MemberProfile)
                .HasForeignKey<MemberProfile>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MemberProfiles_Users");
        });

        modelBuilder.Entity<MembershipLevel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Membersh__3214EC07B8C71CA2");

            entity.Property(e => e.DiscountRate).HasColumnType("decimal(3, 2)");
            entity.Property(e => e.LevelName)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.MinSpending).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Orders__3214EC0748A4F83C");

            entity.HasIndex(e => e.OrderNumber, "UQ__Orders__CAC5E7438D50C8CB").IsUnique();

            entity.Property(e => e.CompletedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DiscountAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.FinalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.OrderNumber)
                .IsRequired()
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.PointDiscount).HasDefaultValue(0);
            entity.Property(e => e.RecipientAddress).HasMaxLength(300);
            entity.Property(e => e.RecipientName).HasMaxLength(50);
            entity.Property(e => e.RecipientPhone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ShippingFee)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Status).HasDefaultValue((byte)0);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Store).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StoreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Stores");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Users");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderDet__3214EC074B317D81");

            entity.Property(e => e.CoverImage).HasMaxLength(500);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductName).HasMaxLength(200);
            entity.Property(e => e.SkuCode).HasMaxLength(100);
            entity.Property(e => e.VariantName).HasMaxLength(100);

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetails_Orders");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetails_Products");

            entity.HasOne(d => d.Variant).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.VariantId)
                .HasConstraintName("FK_OrderDetails_ProductVariants");
        });

        modelBuilder.Entity<OrderReview>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderRev__3214EC07B14E9513");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsHidden).HasDefaultValue(false);

            entity.HasOne(d => d.Order).WithMany(p => p.OrderReviews)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderReviews_Orders");

            entity.HasOne(d => d.User).WithMany(p => p.OrderReviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderReviews_Users");
        });

        modelBuilder.Entity<PaymentLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PaymentL__3214EC0728844063");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MerchantTradeNo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentType)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.RtnMsg).HasMaxLength(200);
            entity.Property(e => e.TradeAmt).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TradeNo)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Order).WithMany(p => p.PaymentLogs)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PaymentLogs_Orders");
        });

        modelBuilder.Entity<PointHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PointHis__3214EC07D753B5DC");

            entity.ToTable("PointHistory");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.OrderNumber)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.PointHistories)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PointHistory_Users");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Products__3214EC077C4F655E");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MaxPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MinPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.Status).HasDefaultValue((byte)1);
            entity.Property(e => e.TotalSales).HasDefaultValue(0);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.VideoUrl).HasMaxLength(500);
            entity.Property(e => e.ViewCount).HasDefaultValue(0);

            entity.HasOne(d => d.Brand).WithMany(p => p.Products)
                .HasForeignKey(d => d.BrandId)
                .HasConstraintName("FK_Products_Brands");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Categories");

            entity.HasOne(d => d.Store).WithMany(p => p.Products)
                .HasForeignKey(d => d.StoreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Stores");
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductI__3214EC077A2F05B7");

            entity.Property(e => e.ImageUrl)
                .IsRequired()
                .HasMaxLength(500);
            entity.Property(e => e.IsMain).HasDefaultValue(false);
            entity.Property(e => e.SortOrder).HasDefaultValue(0);

            entity.HasOne(d => d.Product).WithMany(p => p.ProductImages)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductImages_Products");

            entity.HasOne(d => d.Variant).WithMany(p => p.ProductImages)
                .HasForeignKey(d => d.VariantId)
                .HasConstraintName("FK_ProductImages_ProductVariants");
        });

        modelBuilder.Entity<ProductVariant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductV__3214EC0796154C6E");

            entity.HasIndex(e => e.SkuCode, "UQ__ProductV__3B243948CFC2E65F").IsUnique();

            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SafetyStock).HasDefaultValue(0);
            entity.Property(e => e.SkuCode).HasMaxLength(100);
            entity.Property(e => e.Stock).HasDefaultValue(0);
            entity.Property(e => e.VariantName).HasMaxLength(100);

            entity.HasOne(d => d.Product).WithMany(p => p.ProductVariants)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductVariants_Products");
        });

        modelBuilder.Entity<ReviewImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ReviewIm__3214EC07224D5719");

            entity.Property(e => e.ImageUrl)
                .IsRequired()
                .HasMaxLength(500);

            entity.HasOne(d => d.Review).WithMany(p => p.ReviewImages)
                .HasForeignKey(d => d.ReviewId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReviewImages_OrderReviews");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC072DE957BA");

            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.RoleName)
                .IsRequired()
                .HasMaxLength(20);
        });

        modelBuilder.Entity<SensitiveWord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Sensitiv__3214EC07E071A7A3");

            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.CreatedTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Word)
                .IsRequired()
                .HasMaxLength(100);
        });

        modelBuilder.Entity<Store>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Stores__3214EC075783EBBC");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsVerified).HasDefaultValue(false);
            entity.Property(e => e.StoreName)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(d => d.User).WithMany(p => p.Stores)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Stores_Users");
        });

        modelBuilder.Entity<SupportTicket>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SupportT__3214EC07BC378523");

            entity.Property(e => e.AttachmentUrl).HasMaxLength(500);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ResolvedAt).HasColumnType("datetime");
            entity.Property(e => e.Status).HasDefaultValue((byte)0);
            entity.Property(e => e.Subject).HasMaxLength(100);

            entity.HasOne(d => d.Order).WithMany(p => p.SupportTickets)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_SupportTickets_Orders");

            entity.HasOne(d => d.User).WithMany(p => p.SupportTickets)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SupportTickets_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC0773EC45A5");

            entity.HasIndex(e => e.Account, "UQ__Users__B0C3AC46E8C9C7D3").IsUnique();

            entity.Property(e => e.Account)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ConfirmCode)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.IsBlacklisted).HasDefaultValue(false);
            entity.Property(e => e.IsConfirmed).HasDefaultValue(false);
            entity.Property(e => e.IsSeller).HasDefaultValue(false);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Provider)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ProviderId)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
``n
### 檔案: .\ISpanShop.Models\EfModels\LoginHistory.cs
`${ext}
// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ISpanShop.Models.EfModels;

public partial class LoginHistory
{
    public long Id { get; set; }

    public int UserId { get; set; }

    public DateTime? LoginTime { get; set; }

    public string Ipaddress { get; set; }

    public bool? IsSuccessful { get; set; }

    public virtual User User { get; set; }
}
``n
### 檔案: .\ISpanShop.Models\EfModels\MemberProfile.cs
`${ext}
// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ISpanShop.Models.EfModels;

public partial class MemberProfile
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int LevelId { get; set; }

    public string FullName { get; set; }

    public byte? Gender { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string PhoneNumber { get; set; }

    public decimal? TotalSpending { get; set; }

    public int? PointBalance { get; set; }

    public bool? EmailNotification { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual MembershipLevel Level { get; set; }

    public virtual User User { get; set; }
}
``n
### 檔案: .\ISpanShop.Models\EfModels\MembershipLevel.cs
`${ext}
// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ISpanShop.Models.EfModels;

public partial class MembershipLevel
{
    public int Id { get; set; }

    public string LevelName { get; set; }

    public decimal MinSpending { get; set; }

    public decimal DiscountRate { get; set; }

    public virtual ICollection<MemberProfile> MemberProfiles { get; set; } = new List<MemberProfile>();
}
``n
### 檔案: .\ISpanShop.Models\EfModels\Order.cs
`${ext}
// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ISpanShop.Models.EfModels;

public partial class Order
{
    public long Id { get; set; }

    public string OrderNumber { get; set; }

    public int UserId { get; set; }

    public int StoreId { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal? ShippingFee { get; set; }

    public int? PointDiscount { get; set; }

    public decimal? DiscountAmount { get; set; }

    public decimal FinalAmount { get; set; }

    public byte? Status { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string RecipientName { get; set; }

    public string RecipientPhone { get; set; }

    public string RecipientAddress { get; set; }

    public string Note { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<OrderReview> OrderReviews { get; set; } = new List<OrderReview>();

    public virtual ICollection<PaymentLog> PaymentLogs { get; set; } = new List<PaymentLog>();

    public virtual Store Store { get; set; }

    public virtual ICollection<SupportTicket> SupportTickets { get; set; } = new List<SupportTicket>();

    public virtual User User { get; set; }
}
``n
### 檔案: .\ISpanShop.Models\EfModels\OrderDetail.cs
`${ext}
// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ISpanShop.Models.EfModels;

public partial class OrderDetail
{
    public long Id { get; set; }

    public long OrderId { get; set; }

    public int ProductId { get; set; }

    public int? VariantId { get; set; }

    public string ProductName { get; set; }

    public string VariantName { get; set; }

    public string SkuCode { get; set; }

    public string CoverImage { get; set; }

    public decimal? Price { get; set; }

    public int Quantity { get; set; }

    public virtual Order Order { get; set; }

    public virtual Product Product { get; set; }

    public virtual ProductVariant Variant { get; set; }
}
``n
### 檔案: .\ISpanShop.Models\EfModels\OrderReview.cs
`${ext}
// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ISpanShop.Models.EfModels;

public partial class OrderReview
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public long OrderId { get; set; }

    public byte Rating { get; set; }

    public string Comment { get; set; }

    public string SellerReply { get; set; }

    public bool? IsHidden { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Order Order { get; set; }

    public virtual ICollection<ReviewImage> ReviewImages { get; set; } = new List<ReviewImage>();

    public virtual User User { get; set; }
}
``n
### 檔案: .\ISpanShop.Models\EfModels\PaymentLog.cs
`${ext}
// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ISpanShop.Models.EfModels;

public partial class PaymentLog
{
    public long Id { get; set; }

    public long OrderId { get; set; }

    public string MerchantTradeNo { get; set; }

    public string TradeNo { get; set; }

    public int? RtnCode { get; set; }

    public string RtnMsg { get; set; }

    public decimal? TradeAmt { get; set; }

    public string PaymentType { get; set; }

    public DateTime? PaymentDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Order Order { get; set; }
}
``n
### 檔案: .\ISpanShop.Models\EfModels\PointHistory.cs
`${ext}
// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ISpanShop.Models.EfModels;

public partial class PointHistory
{
    public long Id { get; set; }

    public int UserId { get; set; }

    public string OrderNumber { get; set; }

    public int ChangeAmount { get; set; }

    public int BalanceAfter { get; set; }

    public string Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User User { get; set; }
}
``n
### 檔案: .\ISpanShop.Models\EfModels\Product.cs
`${ext}
// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ISpanShop.Models.EfModels;

public partial class Product
{
    public int Id { get; set; }

    public int StoreId { get; set; }

    public int CategoryId { get; set; }

    public int? BrandId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string VideoUrl { get; set; }

    public string SpecDefinitionJson { get; set; }

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    public int? TotalSales { get; set; }

    public int? ViewCount { get; set; }

    public byte? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string RejectReason { get; set; }

    public virtual Brand Brand { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual Category Category { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();

    public virtual Store Store { get; set; }
}
``n
### 檔案: .\ISpanShop.Models\EfModels\ProductImage.cs
`${ext}
// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ISpanShop.Models.EfModels;

public partial class ProductImage
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int? VariantId { get; set; }

    public string ImageUrl { get; set; }

    public bool? IsMain { get; set; }

    public int? SortOrder { get; set; }

    public virtual Product Product { get; set; }

    public virtual ProductVariant Variant { get; set; }
}
``n
### 檔案: .\ISpanShop.Models\EfModels\ProductVariant.cs
`${ext}
// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ISpanShop.Models.EfModels;

public partial class ProductVariant
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string SkuCode { get; set; }

    public string VariantName { get; set; }

    public string SpecValueJson { get; set; }

    public decimal Price { get; set; }

    public int? Stock { get; set; }

    public int? SafetyStock { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Product Product { get; set; }

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
}
``n
### 檔案: .\ISpanShop.Models\EfModels\ReviewImage.cs
`${ext}
// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ISpanShop.Models.EfModels;

public partial class ReviewImage
{
    public int Id { get; set; }

    public int ReviewId { get; set; }

    public string ImageUrl { get; set; }

    public virtual OrderReview Review { get; set; }
}
``n
### 檔案: .\ISpanShop.Models\EfModels\Role.cs
`${ext}
// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ISpanShop.Models.EfModels;

public partial class Role
{
    public int Id { get; set; }

    public string RoleName { get; set; }

    public string Description { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
``n
### 檔案: .\ISpanShop.Models\EfModels\SensitiveWord.cs
`${ext}
// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ISpanShop.Models.EfModels;

public partial class SensitiveWord
{
    public int Id { get; set; }

    public string Word { get; set; }

    public string Category { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedTime { get; set; }
}
``n
### 檔案: .\ISpanShop.Models\EfModels\Store.cs
`${ext}
// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ISpanShop.Models.EfModels;

public partial class Store
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string StoreName { get; set; }

    public string Description { get; set; }

    public bool? IsVerified { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual User User { get; set; }
}
``n
### 檔案: .\ISpanShop.Models\EfModels\SupportTicket.cs
`${ext}
// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ISpanShop.Models.EfModels;

public partial class SupportTicket
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public long? OrderId { get; set; }

    public byte Category { get; set; }

    public string Subject { get; set; }

    public string Description { get; set; }

    public string AttachmentUrl { get; set; }

    public byte? Status { get; set; }

    public string AdminReply { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public virtual Order Order { get; set; }

    public virtual User User { get; set; }
}
``n
### 檔案: .\ISpanShop.Models\EfModels\User.cs
`${ext}
// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ISpanShop.Models.EfModels;

public partial class User
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public string Account { get; set; }

    public string Password { get; set; }

    public string Email { get; set; }

    public string Provider { get; set; }

    public string ProviderId { get; set; }

    public bool? IsConfirmed { get; set; }

    public string ConfirmCode { get; set; }

    public bool? IsBlacklisted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsSeller { get; set; }

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual ICollection<BlacklistRecord> BlacklistRecordAdminUsers { get; set; } = new List<BlacklistRecord>();

    public virtual ICollection<BlacklistRecord> BlacklistRecordBlockedUsers { get; set; } = new List<BlacklistRecord>();

    public virtual Cart Cart { get; set; }

    public virtual ICollection<ChatMessage> ChatMessageReceivers { get; set; } = new List<ChatMessage>();

    public virtual ICollection<ChatMessage> ChatMessageSenders { get; set; } = new List<ChatMessage>();

    public virtual ICollection<LoginHistory> LoginHistories { get; set; } = new List<LoginHistory>();

    public virtual MemberProfile MemberProfile { get; set; }

    public virtual ICollection<OrderReview> OrderReviews { get; set; } = new List<OrderReview>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<PointHistory> PointHistories { get; set; } = new List<PointHistory>();

    public virtual Role Role { get; set; }

    public virtual ICollection<Store> Stores { get; set; } = new List<Store>();

    public virtual ICollection<SupportTicket> SupportTickets { get; set; } = new List<SupportTicket>();
}
``n
### 檔案: .\ISpanShop.Models\DataSeeder.cs
`${ext}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ISpanShop.Models.EfModels; // 確保對應到你的 EF Models 命名空間

namespace ISpanShop.Models
{
	/// <summary>
	/// 電商資料播種程式 - 從公開 API (DummyJSON) 串接真實商品資料
	/// </summary>
	public class DataSeeder
	{
		private static readonly Random _random = new Random();
		private const string DUMMYJSON_URL = "https://dummyjson.com/products?limit=194"; // 已經幫你擴充到 194 筆
		private const decimal USD_TO_TWD = 30;  // 匯率：美金轉台幣

		/// <summary>
		/// 父子多層式分類對應表 (階層分類)
		/// Dictionary<API原始分類, (主分類名稱, 子分類名稱)>
		/// </summary>
		private static readonly Dictionary<string, (string ParentName, string ChildName)> CategoryHierarchyMap = new()
		{
            // === 美妝與保養 ===
            { "beauty", ("美妝與保養", "彩妝與修容") },
			{ "fragrances", ("美妝與保養", "香水與香氛") },
			{ "skin-care", ("美妝與保養", "臉部與身體保養") },
            
            // === 居家生活 ===
            { "furniture", ("居家與生活", "大型家具") },
			{ "home-decoration", ("居家與生活", "居家裝飾與收納") },
			{ "kitchen-accessories", ("居家與生活", "廚房餐具與用品") },
			{ "groceries", ("美食與生鮮", "生鮮食材與飲品") },
            
            // === 3C與電子 ===
            { "laptops", ("3C與電子", "筆記型電腦") },
			{ "smartphones", ("3C與電子", "智慧型手機") },
			{ "tablets", ("3C與電子", "平板電腦") },
			{ "mobile-accessories", ("3C與電子", "手機與平板周邊") },
            
            // === 時尚男裝 ===
            { "mens-shirts", ("男裝與配件", "男款上衣與襯衫") },
			{ "mens-shoes", ("男裝與配件", "男士鞋款") },
			{ "mens-watches", ("男裝與配件", "男士腕錶") },
            
            // === 時尚女裝 ===
            { "tops", ("女裝與配件", "女款上衣與洋裝") },
			{ "womens-dresses", ("女裝與配件", "派對與晚禮服") },
			{ "womens-bags", ("女裝與配件", "精品包包") },
			{ "womens-shoes", ("女裝與配件", "女款鞋類") },
			{ "womens-watches", ("女裝與配件", "女士腕錶") },
			{ "womens-jewellery", ("女裝與配件", "珠寶與飾品") },
            
            // === 戶外與配件 ===
            { "sunglasses", ("運動與休閒", "太陽眼鏡與配件") },
			{ "sports-accessories", ("運動與休閒", "運動裝備與球類") },
            
            // === 汽機車 ===
            { "motorcycle", ("汽機車百貨", "機車與周邊配件") },
			{ "vehicle", ("汽機車百貨", "汽車與周邊配件") }
		};

		/// <summary>
		/// 品牌名稱翻譯表
		/// 幫你把常見大廠翻譯成台灣習慣的說法
		/// </summary>
		private static readonly Dictionary<string, string> BrandTranslationMap = new(StringComparer.OrdinalIgnoreCase)
		{
            // 3C 與科技
            { "Apple", "蘋果 (Apple)" },
			{ "Samsung", "三星 (Samsung)" },
			{ "Huawei", "華為 (Huawei)" },
			{ "Lenovo", "聯想 (Lenovo)" },
			{ "Dell", "戴爾 (Dell)" },
			{ "Asus", "華碩 (ASUS)" },
			{ "Oppo", "OPPO" },
			{ "Realme", "realme" },
			{ "Vivo", "vivo" },
			{ "Amazon", "亞馬遜 (Amazon)" },
			{ "Beats", "Beats by Dr. Dre" },
			{ "Gigabyte", "技嘉 (AORUS)" },
            
            // 精品與香水
            { "Chanel", "香奈兒 (Chanel)" },
			{ "Dior", "迪奧 (Dior)" },
			{ "Dolce & Gabbana", "Dolce & Gabbana (D&G)" },
			{ "Gucci", "古馳 (Gucci)" },
			{ "Prada", "普拉達 (Prada)" },
			{ "Calvin Klein", "Calvin Klein (CK)" },
			{ "Rolex", "勞力士 (Rolex)" },
			{ "Longines", "浪琴 (Longines)" },
			{ "IWC", "萬國錶 (IWC)" },
            
            // 運動與服飾
            { "Nike", "耐吉 (Nike)" },
			{ "Puma", "彪馬 (PUMA)" },
			{ "Off White", "Off-White" },
            
            // 汽機車
            { "Kawasaki", "川崎 (Kawasaki)" },
			{ "Chrysler", "克萊斯勒 (Chrysler)" },
			{ "Dodge", "道奇 (Dodge)" },
            
            // 日用品與美妝
            { "Olay", "歐蕾 (Olay)" },
			{ "Vaseline", "凡士林 (Vaseline)" },
			{ "Attitude", "Attitude 天然護理" },
			{ "Essence", "Essence 艾森絲" }
		};

		
		private static readonly Dictionary<string, (string Title, string Description)> ProductTranslationMap = new()
		{
			// === 💄 美妝與護膚 (Cosmetics & Skincare) ===
    {"Essence Mascara Lash Princess", ("精華纖長睫毛膏", "超人氣濃密纖長睫毛膏，持久防水配方讓您輕鬆打造無死角的迷人電眼。")},
	{"Eyeshadow Palette with Mirror", ("自帶補妝鏡多色眼影盤", "百搭實用色系一次擁有！粉質細膩服貼，內附實用化妝鏡，隨時隨地保持完美妝容。")},
	{"Powder Canister", ("無瑕控油定妝蜜粉", "輕盈透氣的細緻粉末，長效控油不暗沉，為您打造宛如天生的微霧面平滑底妝。")},
	{"Red Lipstick", ("經典正紅誘惑唇膏", "絲絨奶霜質地，一抹極致顯色，為雙唇注入飽滿色彩，瞬間提升全場氣場。")},
	{"Red Nail Polish", ("勃艮第紅指甲油", "沙龍級高光澤亮面，快乾持久不掉色，讓指尖散發迷人的成熟女人味。")},
    
    // === 🌸 香水 (Fragrances) ===
    {"Calvin Klein CK One", ("CK One 中性淡香水", "經典不敗的清新綠茶柑橘調，男女皆宜，散發隨性又充滿活力的獨特魅力。")},
	{"Chanel Coco Noir Eau De", ("香奈兒黑色可可淡香水", "揉合葡萄柚、玫瑰與溫潤檀香，神祕且優雅，是夜晚約會或晚宴的完美選擇。")},
	{"Dior J'adore", ("Dior 真我宣言淡香水", "奢華揉合依蘭依蘭、玫瑰與頂級茉莉，完美詮釋女性的極致優雅與自信。")},
	{"Dolce Shine Eau de", ("D&G 閃耀女性淡香水", "充滿陽光氣息的芒果與茉莉花果香調，帶來猶如漫步在義大利海岸的愉悅心情。")},
	{"Gucci Bloom Eau de", ("Gucci 花悅女性淡香水", "以晚香玉與茉莉交織出宛如盛開花園的香氣，展現現代女性的浪漫與柔美。")},
    
    // === 🛋️ 家具與居家裝飾 (Furniture & Home Decor) ===
    {"Annibale Colombo Bed", ("義大利經典雙人床架", "頂級材質結合匠心工藝，為您的臥室注入奢華質感，提供最極致的睡眠體驗。")},
	{"Annibale Colombo Sofa", ("義大利頂級真皮沙發", "精緻設計與頂級皮革的完美相遇，坐感舒適，是客廳品味升級的絕佳焦點。")},
	{"Bedside Table African Cherry", ("非洲櫻桃木床頭櫃", "保留珍貴木材天然紋理，兼具實用收納與優雅設計，提升整體空間溫馨感。")},
	{"Knoll Saarinen Executive Conference Chair", ("高階主管人體工學椅", "世紀中期現代主義經典之作！完美支撐背部曲線，辦公與會議室的質感首選。")},
	{"Wooden Bathroom Sink With Mirror", ("實木衛浴浴櫃附化妝鏡", "防潮實木打造，獨特質感設計搭配專屬浴鏡，讓衛浴空間煥然一新。")},
	{"Decoration Swing", ("北歐風手工編織吊籃", "精緻編織細節，為居家空間增添一抹童話般的優雅與波西米亞度假風情。")},
	{"Family Tree Photo Frame", ("家族樹造型相框", "以質感樹木造型展示多張珍貴照片，讓溫馨的家庭回憶成為最美的居家裝飾。")},
	{"House Showpiece Plant", ("擬真居家裝飾盆栽", "無需澆水照料即可擁有滿滿綠意，為室內空間注入自然生機與現代設計感。")},
	{"Plant Pot", ("極簡風現代質感花盆", "俐落流暢的線條設計，完美襯托各式植栽，無論放室內外都能提升空間品味。")},
	{"Table Lamp", ("護眼金屬質感檯燈", "兼具照明與裝飾功能，柔和光源營造溫馨氛圍，為您的閱讀或工作時光加分。")},

    // === 🥩 生鮮與食品 (Groceries) ===
    {"Apple", ("產地直送鮮脆蘋果", "每日清晨新鮮採摘，果肉鮮甜多汁，富含豐富維他命C，健康活力的最佳來源。")},
	{"Beef Steak", ("極黑和牛霜降牛排", "完美大理石油花分布，肉質軟嫩多汁，讓您在家也能輕鬆煎出五星級美味。")},
	{"Cat Food", ("無穀頂級鮮肉貓糧", "專為挑嘴貓主子設計，高含肉量且無添加穀物，全面滿足愛貓的營養需求。")},
	{"Chicken Meat", ("去骨本土仿土雞腿肉", "嚴選放山雞，肉質Q彈有嚼勁，適合煮湯、煎烤，是家庭料理的百搭食材。")},
	{"Cooking Oil", ("冷壓初榨頂級橄欖油", "100% 第一道冷壓榨取，保留最豐富的營養精華，適合涼拌與低溫健康烹調。")},
	{"Cucumber", ("溫室無毒脆甜小黃瓜", "網室精心栽培無農藥殘留，口感清脆爽口，生菜沙拉或涼拌菜的最佳主角。")},
	{"Dog Food", ("全齡犬深海魚狗糧", "富含必需營養素與 Omega-3，保護關節與毛髮，讓愛犬每天充滿健康活力。")},
	{"Eggs", ("友善平飼放牧紅殼蛋", "來自快樂母雞的健康好蛋，蛋黃飽滿濃郁無抗生素，適合各式烘焙與料理。")},
	{"Fish Steak", ("挪威空運厚切鮭魚片", "來自純淨海域的頂級漁獲，油脂豐富入口即化，簡單乾煎即可品嚐極致海味。")},
	{"Green Bell Pepper", ("產銷履歷翠綠青椒", "色澤鮮綠且果肉厚實，富含膳食纖維，為您的熱炒料理增添豐富色彩與口感。")},
	{"Green Chili Pepper", ("特級辛香青辣椒", "辣度適中並帶有獨特清香，適合爆香或製作特製醃料，瞬間提升料理層次。")},
	{"Honey Jar", ("台灣龍眼純天然蜂蜜", "100% 純天然無添加，蜜香濃郁色澤琥珀，無論沖泡飲品或烘焙甜點皆宜。")},
	{"Ice Cream", ("法式香草莢冰淇淋", "採用頂級天然香草莢製作，口感綿密順滑，帶給您夏日最奢華的甜品享受。")},
	{"Juice", ("100% 鮮榨綜合果汁", "滿滿維他命與天然果香，無添加人工糖分，是您早晨解渴與補充活力的首選。")},
	{"Kiwi", ("紐西蘭特級奇異果", "營養密度極高，酸甜多汁，無論直接享用或加入優格沙拉，都是健康好滋味。")},
	{"Lemon", ("屏東無籽綠檸檬", "酸度十足且香氣濃郁，適合入菜、烘焙或製作清涼解渴的蜂蜜檸檬飲品。")},
	{"Milk", ("在地小農鮮乳", "每日清晨產地直送，保留最純粹的濃郁奶香與豐富鈣質，全家人健康的好夥伴。")},
	{"Mulberry", ("有機手採甜黑桑葚", "果實飽滿多汁，富含花青素與抗氧化物，當作零食或熬煮果醬都非常適合。")},
	{"Nescafe Coffee", ("雀巢金牌醇厚咖啡", "嚴選咖啡豆烘焙而成，香氣濃郁口感滑順，為您帶來早晨最美好的醒腦時光。")},
	{"Potatoes", ("產銷履歷優質馬鈴薯", "口感綿密鬆軟，無論是烤馬鈴薯、煮濃湯或做成馬鈴薯泥，都是百搭神級食材。")},
	{"Protein Powder", ("高蛋白乳清沖泡飲", "提供優質且好吸收的蛋白質，是健身族群肌肉修復與日常營養補充的最佳選擇。")},
	{"Red Onions", ("紫皮鮮甜紅洋蔥", "香氣濃郁且辛辣度低，富含豐富微量元素，是提升燉肉與沙拉風味的秘密武器。")},
	{"Rice", ("花東特級香米", "米粒飽滿透亮，煮熟後散發淡淡芋頭香氣，口感Q彈，是各式台菜的完美基底。")},
	{"Soft Drinks", ("經典氣泡汽水綜合組", "多種暢快口味一次滿足，氣泡強烈清涼解渴，是派對聚餐不可或缺的歡樂飲品。")},
	{"Strawberry", ("大湖直送鮮採草莓", "果實碩大紅潤，酸甜多汁香氣撲鼻，無論單吃或製作甜點都能帶來滿滿幸福感。")},
	{"Tissue Paper Box", ("純水柔韌抽取式衛生紙", "親膚觸感且不易破，不含螢光劑，為您與家人提供最安心柔軟的日常清潔。")},
	{"Water", ("深海純淨微礦泉水", "經過多重過濾與殺菌，蘊含微量礦物質，口感甘甜順口，隨時補充流失水分。")},

    // === 🍳 廚房用品 (Kitchenware) ===
    {"Bamboo Spatula", ("天然竹製不沾鍋鍋鏟", "環保天然竹材製成，耐高溫且不傷鍋面，手感溫潤，是翻炒料理的得力助手。")},
	{"Black Aluminium Cup", ("霧黑鋁合金冷水杯", "極簡輕量化設計，導冷極快，讓您在炎炎夏日享受最透心涼的飲品體驗。")},
	{"Black Whisk", ("人體工學矽膠打蛋器", "防滑握把搭配耐熱矽膠，輕鬆打發蛋液與奶油，不刮傷打蛋盆，清洗超方便。")},
	{"Boxed Blender", ("多功能隨行果汁機", "強勁馬力輕鬆擊碎冰塊，附贈隨行杯蓋，讓您隨時隨地享受新鮮現打的健康蔬果汁。")},
	{"Carbon Steel Wok", ("無塗層碳鋼中華炒鍋", "傳熱迅速均勻，經開鍋保養後可形成天然不沾層，完美鎖住食材的爆炒鍋氣。")},
	{"Chopping Board", ("抗菌防滑實木砧板", "採用高密度原木壓製，不易留刀痕與發霉，提供最安全衛生的食材處理環境。")},
	{"Citrus Squeezer Yellow", ("手動檸檬鮮果壓汁器", "亮眼色彩搭配省力槓桿設計，輕鬆榨取每一滴新鮮果汁，完整保留維生素C。")},
	{"Egg Slicer", ("不鏽鋼切蛋器", "輕輕一壓即可切出完美均勻的蛋片，是製作三明治、沙拉與冷盤的必備神器。")},
	{"Electric Stove", ("便攜式多功能電磁爐", "輕巧不佔空間，支援多段火力調節與定時功能，租屋族與小家庭的料理好幫手。")},
	{"Fine Mesh Strainer", ("不鏽鋼細目漏勺過濾網", "網格極細緻，過濾高湯、撈浮沫或篩麵粉都超級好用，確保料理口感細滑。")},
	{"Fork", ("經典不鏽鋼餐叉", "優質加厚不鏽鋼打造，線條流暢握感厚實，為您的餐桌增添一絲優雅氣息。")},
	{"Glass", ("北歐風透明玻璃水杯", "清透無鉛玻璃材質，極簡造型，完美展現各類飲品的漸層色澤與清涼感。")},
	{"Grater Black", ("多功能四面刨絲器", "鋒利耐用，無論是起司、蘿蔔絲或檸檬皮都能輕鬆刨削，大幅縮短備料時間。")},
	{"Hand Blender", ("手持式攪拌棒四件組", "攪拌、切碎、打發一機搞定，體積輕巧好收納，製作副食品或濃湯的最佳幫手。")},
	{"Ice Cube Tray", ("食品級矽膠製冰盒", "柔軟材質讓脫模超級輕鬆，附帶防塵蓋設計，確保冰塊無異味，炎夏必備。")},
	{"Kitchen Sieve", ("烘焙專用細緻麵粉篩", "均勻過篩結塊麵粉，讓蛋糕與餅乾口感更加蓬鬆細緻，烘焙新手的完美工具。")},
	{"Knife", ("主廚級大馬士革切片刀", "鋒利無比且配重完美，輕鬆處理各類肉品與蔬菜，體驗行雲流水般的刀工。")},
	{"Lunch Box", ("日系分隔微波便當盒", "防漏設計且分格不串味，材質安全可直接微波加熱，讓帶便當成為一種享受。")},
	{"Microwave Oven", ("智能微電腦微波爐", "多段微波火力與內建多種自動烹調模式，加熱、解凍迅速均勻，廚房效率神機。")},
	{"Mug Tree Stand", ("實木馬克杯收納掛架", "穩固不傾倒，有效利用桌面垂直空間，同時展示您心愛的馬克杯收藏。")},
	{"Pan", ("麥飯石不沾平底鍋", "採用醫療級不沾塗層，只需少油即可煎出完美荷包蛋，清洗只需一抹即淨。")},
	{"Plate", ("陶瓷簡約西餐盤", "高溫燒製釉面光滑，耐磨防刮，完美的留白設計讓每道家常菜都像米其林大餐。")},
	{"Red Tongs", ("耐高溫矽膠料理夾", "防燙防滑握把，夾取熱食或翻麵煎肉超順手，前端矽膠設計保護鍋具不刮傷。")},
	{"Silver Pot With Glass Cap", ("304不鏽鋼雙耳湯鍋附蓋", "導熱快且保溫效果好，透明玻璃鍋蓋讓烹煮進度一目了然，燉湯滷肉必備。")},
	{"Slotted Turner", ("不鏽鋼鏤空煎魚鏟", "濾油鏤空設計，極薄鏟口輕鬆翻面不破皮，讓您煎魚、煎牛排更加得心應手。")},
	{"Spice Rack", ("旋轉式調味料收納架", "包含多個透明玻璃調味罐，360度旋轉拿取超方便，讓廚房檯面告別雜亂。")},
	{"Spoon", ("圓潤不鏽鋼喝湯匙", "邊緣打磨光滑不刮嘴，深度適中，無論是喝濃湯或吃甜品都能擁有極佳手感。")},
	{"Tray", ("防滑皮革把手托盤", "木紋質感搭配防滑表面，無論是端茶水或作為客廳桌面的裝飾收納都極具品味。")},
	{"Wooden Rolling Pin", ("實木烘焙桿麵棍", "打磨光滑不黏麵糰，重量適中好施力，無論包餃子或做披薩餅皮都能輕鬆擀平。")},
	{"Yellow Peeler", ("陶瓷刀片削皮刀", "鋒利且永不生鏽，貼合蔬果曲線，輕鬆削去薄皮不浪費果肉，亮色系超好找。")},

    // === 💻 筆電與 3C 周邊 (Laptops & Electronics) ===
    {"Apple MacBook Pro 14 Inch Space Grey", ("MacBook Pro 14吋 太空灰", "搭載 Apple 突破性的 M 晶片，提供難以置信的超狂效能與驚人的電池續航力。")},
	{"Asus Zenbook Pro Dual Screen Laptop", ("ASUS 雙螢幕創作者筆電", "創新的雙螢幕設計帶來前所未有的多工處理體驗，是影像創作者的最佳工作站。")},
	{"Huawei Matebook X Pro", ("華為 Matebook 觸控筆電", "極致輕薄金屬機身搭配 3K 觸控全面屏，提供令人驚豔的視覺享受與頂級商務體驗。")},
	{"Lenovo Yoga 920", ("聯想 Yoga 翻轉筆電", "獨家錶鍊式轉軸設計，筆電平板一秒切換，支援觸控筆，隨時記錄無限靈感。")},
	{"New DELL XPS 13 9300 Laptop", ("DELL XPS 13 極窄邊框筆電", "將 13 吋螢幕完美塞入 11 吋機身，採用頂級碳纖維材質，輕巧與效能的極致展現。")},
	{"Amazon Echo Plus", ("Amazon Echo Plus 智慧音箱", "內建強大 Alexa 語音助理，提供 360 度環繞音效，輕鬆聲控您的專屬智慧家庭。")},
	{"Apple Airpods", ("Apple AirPods 藍牙耳機", "開蓋即連線的魔法體驗！提供清晰音質與絕佳通話品質，感受真正的無線自由。")},
	{"Apple AirPods Max Silver", ("AirPods Max 銀色耳罩耳機", "結合高保真音質與頂尖主動降噪技術，極致舒適的耳罩設計，帶您沉浸音樂世界。")},
	{"Apple Airpower Wireless Charger", ("多設備無線充電板", "隨放隨充！支援 iPhone、Apple Watch 與 AirPods 同時充電，告別雜亂的充電線。")},
	{"Apple HomePod Mini Cosmic Grey", ("HomePod Mini 太空灰", "體積極致小巧卻能爆發驚人音量，無縫串聯 Apple 生態系，打造全屋智慧音響。")},
	{"Apple iPhone Charger", ("原廠 20W USB-C 快充頭", "提供穩定安全的快速充電體驗，只需 30 分鐘即可充入 50% 電量，效率滿分。")},
	{"Apple MagSafe Battery Pack", ("MagSafe 磁吸行動電源", "一貼即充的絕佳便利性！完美對位您的 iPhone，出門在外隨時補充戰鬥力。")},
	{"Apple Watch Series 4 Gold", ("Apple Watch S4 金色", "不僅是手錶，更是您的健康守護者。支援心電圖與跌倒偵測，隨時掌握身體狀況。")},
	{"Beats Flex Wireless Earphones", ("Beats Flex 頸掛式藍牙耳機", "磁吸式耳塞設計，提供 12 小時超長續航，是日常通勤與運動休閒的最佳伴侶。")},
	{"iPhone 12 Silicone Case with MagSafe Plum", ("iPhone 12 MagSafe 矽膠保護殼", "如絲綢般柔滑的矽膠觸感，內建磁石完美支援 MagSafe 配件，提供全方位防護。")},
	{"Monopod", ("專業級相機單腳架", "輕量化鋁合金材質，快速伸縮鎖定，為您的動態攝影與微距拍攝提供極佳穩定度。")},
	{"Selfie Lamp with iPhone", ("手機專用美顏補光燈", "多段色溫與亮度調節，一秒夾上手機，讓您在夜間或室內也能拍出無瑕網美照。")},
	{"Selfie Stick Monopod", ("藍牙遙控三腳架自拍桿", "結合自拍桿與穩固三腳架，配備分離式藍牙遙控器，出遊合照不求人。")},
	{"TV Studio Camera Pedestal", ("廣播級攝影機氣壓腳架", "專業電視台級設備，提供如絲般順滑的平移與升降運鏡，適合高階影音製作團隊。")},

    // === 👕 衣服、鞋包與配件 (Fashion, Bags & Accessories) ===
    {"Blue & Black Check Shirt", ("藍黑格紋純棉襯衫", "經典不敗的英倫格紋設計，親膚純棉材質，無論單穿或當作薄外套都極具型格。")},
	{"Gigabyte Aorus Men Tshirt", ("Aorus 電競純棉短T", "採用透氣舒適純棉材質，印有經典電競信仰 Logo，展現硬核玩家的專屬休閒穿搭。")},
	{"Man Plaid Shirt", ("經典休閒格紋襯衫", "俐落剪裁修飾身型，百搭的復古格紋圖騰，是男士衣櫃裡不可或缺的質感單品。")},
	{"Man Short Sleeve Shirt", ("修身剪裁短袖襯衫", "輕薄透氣材質，適合炎熱夏季穿著，展現乾淨俐落的微正式感，上班休閒兩相宜。")},
	{"Men Check Shirt", ("商務休閒格紋襯衫", "硬挺領型與舒適布料的完美結合，搭配西裝褲或牛仔褲，輕鬆穿出都會雅痞風。")},
	{"Nike Air Jordan 1 Red And Black", ("Air Jordan 1 經典黑紅", "風靡全球的傳奇籃球鞋！經典的黑紅配色與高筒設計，街頭潮流穿搭的終極指標。")},
	{"Nike Baseball Cleats", ("Nike 專業棒球釘鞋", "提供卓越的抓地力與爆發力，輕量化鞋身設計，助您在紅土球場上跑出最佳表現。")},
	{"Puma Future Rider Trainers", ("Puma 復古休閒老爹鞋", "結合復古外型與現代避震科技，色彩鮮明，為您的日常穿搭注入滿滿街頭活力。")},
	{"Sports Sneakers Off White & Red", ("Off-White 風格運動潮鞋", "大膽的解構設計與標誌性紅色束帶，極致吸睛的街頭話題鞋款，展現強烈個人態度。")},
	{"Sports Sneakers Off White Red", ("休閒氣墊運動鞋 白/紅", "輕量透氣網布鞋面搭配緩震氣墊，不僅外型時尚，更提供全天候行走的舒適感。")},
	{"Blue Women's Handbag", ("知性藍真皮托特包", "擁有超大容量與質感真皮手感，可放入筆電與A4文件，都會職場女性的必備通勤包。")},
	{"Heshe Women's Leather Bag", ("Heshe 歐美風真皮水桶包", "精緻工藝縫線與立體包型，隨性又不失優雅，是日常逛街與約會的最佳點綴。")},
	{"Prada Women Bag", ("Prada 經典倒三角標誌包", "象徵奢華品味的頂級精品，細緻的十字紋牛皮展現不凡身分，讓您成為全場焦點。")},
	{"White Faux Leather Backpack", ("純白極簡防潑水後背包", "輕巧無負擔的百搭包款，具備多層收納空間與防潑水機能，適合輕旅行與學生族群。")},
	{"Women Handbag Black", ("經典百搭純黑晚宴包", "雋永不敗的極簡純黑設計，低調中散發高貴氣質，完美襯托您的各式派對洋裝。")},
	{"Black Women's Gown", ("赫本風純黑無袖晚禮服", "極致優雅的立體剪裁，完美展現女性曼妙曲線，是出席重大晚宴絕對不會出錯的戰袍。")},
	{"Corset Leather With Skirt", ("性感皮革馬甲綁帶套裝", "大膽前衛的皮革材質搭配修身短裙，展現強烈個性與迷人魅力，跑趴必備吸睛穿搭。")},
	{"Corset With Black Skirt", ("蕾絲馬甲搭高腰黑裙", "甜美與性感的完美平衡，精緻蕾絲細節修飾腰線，讓您散發難以抗拒的女人味。")},
	{"Dress Pea", ("法式復古波卡圓點洋裝", "俏皮經典的圓點圖騰，搭配輕飄飄的雪紡材質，穿上瞬間自帶浪漫的法式少女感。")},
	{"Marni Red & Black Suit", ("Marni 紅黑撞色設計套裝", "高級時裝品牌的剪裁工藝，強烈的撞色對比展現自信氣場，都會女強人的時髦首選。")},
	{"Green Crystal Earring", ("翡翠綠水晶水滴耳環", "璀璨奪目的切工折射出耀眼光芒，隨著步伐搖曳生姿，為您的整體造型畫龍點睛。")},
	{"Green Oval Earring", ("復古祖母綠橢圓耳釘", "經典氣質款！低調奢華的墨綠寶石鑲嵌於精緻金屬托座，展現皇室般的古典優雅。")},
	{"Tropical Earring", ("熱帶風情流蘇造型耳環", "色彩繽紛的南洋度假風格設計，視覺效果強烈，戴上它立刻擁有飛往海島的好心情。")},
	{"Black & Brown Slipper", ("經典真皮雙帶涼拖鞋", "人體工學軟木鞋床設計，越穿越貼合腳型，百搭黑棕配色，夏日休閒穿搭必備。")},
	{"Calvin Klein Heel Shoes", ("CK 尖頭氣質高跟鞋", "完美修飾腿部線條的性感尖頭設計，舒適穩定的鞋跟，讓您每一步都散發自信光芒。")},
	{"Golden Shoes Woman", ("璀璨亮片晚宴細跟鞋", "全鞋身覆蓋閃耀亮片，宛如灰姑娘的玻璃鞋，讓您在聚光燈下成為最耀眼的女王。")},
	{"Pampi Shoes", ("百搭舒適莫卡辛豆豆鞋", "超柔軟真皮鞋面與極致彎折彈性，久走不累腳，是日常休閒與輕旅行的最佳鞋款。")},
	{"Red Shoes", ("法式優雅正紅瑪莉珍鞋", "復古可愛的繫帶設計搭配吸睛正紅色，瞬間點亮整體穿搭，散發甜美復古風情。")},

    // === ⌚ 手錶精品 (Watches) ===
    {"Brown Leather Belt Watch", ("復古真皮錶帶男仕腕錶", "搭載精準石英機芯，搭配質感頂級牛皮錶帶，完美展現成熟穩重的男士獨特品味。")},
	{"Longines Master Collection", ("浪琴巨擘系列機械錶", "傳承百年瑞士製錶工藝，優雅麥穗紋錶盤搭配藍鋼指針，象徵奢華與高雅的卓越之作。")},
	{"Rolex Cellini Date Black Dial", ("勞力士徹利尼黑面日期錶", "摒棄粗獷，展現古典之美。低調奢華的黑面盤結合實用日期功能，彰顯非凡品味。")},
	{"Rolex Cellini Moonphase", ("勞力士徹利尼月相錶", "結合隕石月相盤的極致複雜工藝，將浩瀚宇宙濃縮於腕間，是頂級鐘錶收藏家的摯愛。")},
	{"Rolex Datejust", ("勞力士日誌型經典腕錶", "全球最經典的防水腕錶之一！配備標誌性放大鏡日期窗與五珠帶，耐用且極具保值性。")},
	{"Rolex Submariner Watch", ("勞力士黑水鬼潛水錶", "傳奇的不敗神話！具備超強防水與夜光功能，無可挑惕的耐用度，霸氣十足的夢幻逸品。")},
	{"IWC Ingenieur Automatic Steel", ("IWC 萬國工程師系列自動錶", "由大師 Gérald Genta 設計的硬朗防磁錶款，精鋼錶殼展現陽剛的工程學幾何美感。")},
	{"Rolex Datejust Women", ("勞力士女裝日誌型鑽錶", "專為女性打造的優雅尺寸，鑲嵌璀璨美鑽，完美結合頂級製錶技術與高級珠寶工藝。")},
	{"Watch Gold for Women", ("米蘭金屬編織帶女錶", "閃耀細緻的金色米蘭錶帶服貼手腕，猶如精美手鍊般點綴，為您的穿搭增添輕奢質感。")},
	{"Women's Wrist Watch", ("簡約知性細帶小圓錶", "清晰易讀的極簡錶盤設計，搭配溫柔的細緻皮帶，是日常上班與約會的百搭氣質單品。")},

    // === 📱 智慧型手機與平板 (Smartphones & Tablets) ===
    {"iPhone 5s", ("Apple iPhone 5s", "改變世界的經典神機！首創 Touch ID 指紋辨識，鋁合金鑽石切邊設計，令人懷念的完美手感。")},
	{"iPhone 6", ("Apple iPhone 6", "引領大螢幕潮流的革命性機種，圓潤機身與 Retina 顯示器，流暢的系統體驗依然經典。")},
	{"iPhone 13 Pro", ("Apple iPhone 13 Pro", "支援 120Hz ProMotion 螢幕，微距攝影與電影級模式，帶您體驗無與倫比的流暢與專業拍攝。")},
	{"iPhone X", ("Apple iPhone X", "十週年紀念之作！跨時代全螢幕設計與 Face ID 臉部辨識技術，重新定義智慧型手機的未來。")},
	{"Samsung Universe 9", ("三星 Universe 9 旗艦機", "極致鮮豔的曲面 AMOLED 螢幕，搭配超大電量與強大效能，是您追劇與遊戲的絕佳神隊友。")},
	{"OPPOF19", ("OPPO F19 輕薄閃充手機", "極致輕薄機身，搭載獨家 VOOC 閃充技術，充電 5 分鐘通話 2 小時，隨時保持滿電活力。")},
	{"Huawei P30", ("華為 P30 徠卡旗艦機", "與徠卡聯合設計的專業鏡頭，超強感光夜拍能力，讓您在低光源下依然能拍出大師級美照。")},
	{"Oppo A57", ("OPPO A57 大電量娛樂機", "超大容量電池搭配立體聲雙喇叭，無論看影片或聽音樂都能擁有沈浸式的影音享受。")},
	{"Oppo F19 Pro Plus", ("OPPO F19 Pro+ 5G", "支援 5G 高速上網，配備 AI 錄影增強技術，讓您隨手拍出清晰動人的 Vlog 影像紀錄。")},
	{"Oppo K1", ("OPPO K1 螢幕指紋機", "將高階光感螢幕指紋解鎖技術帶入平價市場，水滴全螢幕帶來更廣闊的視覺衝擊。")},
	{"Realme C35", ("realme C35 極光設計機", "最美入門機！直角邊框搭配超薄機身，5000萬畫素主鏡頭讓您輕鬆記錄生活精彩瞬間。")},
	{"Realme X", ("realme X 升降鏡頭全螢幕", "炫酷升降式前鏡頭實現真正的「無瀏海」全螢幕體驗，驍龍處理器提供穩定順暢效能。")},
	{"Realme XT", ("realme XT 6400萬鷹眼機", "首款搭載 6400 萬超高畫素鏡頭，極致解析力讓您放大再放大，細節依然清晰可見。")},
	{"Samsung Galaxy S7", ("三星 Galaxy S7 防水平板", "具備 IP68 最高等級防塵防水，雙曲面玻璃設計與超快對焦鏡頭，經典旗艦的實力之作。")},
	{"Samsung Galaxy S8", ("三星 Galaxy S8 無邊際螢幕", "打破邊框限制的 Infinity Display 無邊際螢幕，猶如將整個世界握在手中般震撼。")},
	{"Samsung Galaxy S10", ("三星 Galaxy S10 超視覺旗艦", "螢幕下超聲波指紋辨識與強大的三鏡頭系統，支援無線超充分享，科技與美學的巔峰。")},
	{"Vivo S1", ("vivo S1 前置 3200 萬美顏機", "專為愛自拍的您設計！超高畫素前鏡頭搭配 AI 智慧美顏，不用修圖也能拍出仙女肌。")},
	{"Vivo V9", ("vivo V9 雙鏡頭 AI 旗艦", "極窄邊框全面屏設計，後置雙鏡頭支援硬體級人像景深，讓您輕鬆拍出單眼般的人像照。")},
	{"Vivo X21", ("vivo X21 隱形指紋手機", "引領業界的螢幕下指紋解鎖技術，極具未來感的極簡外型，給您最酷炫的解鎖體驗。")},
	{"iPad Mini 2021 Starlight", ("iPad Mini 6 星光色", "一手掌握的超級效能！全新全面屏設計搭配支援磁吸 Apple Pencil，隨身最強數位筆記本。")},
	{"Samsung Galaxy Tab S8 Plus Grey", ("三星 Galaxy Tab S8+ 平板", "超大 12.4 吋 AMOLED 螢幕，附贈超低延遲 S Pen，無論繪圖創作或觀影娛樂都無懈可擊。")},
	{"Samsung Galaxy Tab White", ("三星 Galaxy Tab 輕巧平板", "極致輕薄好攜帶，提供持久續航力與護眼模式，是孩童學習與家庭日常使用的最佳選擇。")},

    // === 🏀 運動器材 (Sports Equipment) ===
    {"American Football", ("美式足球標準訓練球", "耐磨複合皮革材質，防滑顆粒設計提供絕佳握感，適合傳球與接球等激烈美式足球賽事。")},
	{"Baseball Ball", ("標準硬式棒球", "真皮外皮搭配紅色精密雙縫線，手感紮實，提供投球時完美的轉速與握感。")},
	{"Baseball Glove", ("頂級牛皮棒球手套", "嚴選天然牛皮手工穿線，吸震護墊設計有效減輕接球衝擊，內野手與外野手防守利器。")},
	{"Basketball", ("室內外通用標準籃球", "加深溝槽設計提升控球穩定度，吸濕耐磨PU表皮，無論木地板或柏油路都能發揮最佳表現。")},
	{"Basketball Rim", ("高強度鋼製籃球框", "實心鋼材打造配備緩震彈簧，耐得住強力灌籃的衝擊，為您打造專業級的主場體驗。")},
	{"Cricket Ball", ("專業板球硬球", "採用傳統多層軟木與優質皮革縫製，硬度十足且縫線凸出，為投手提供極致的變化球路。")},
	{"Cricket Bat", ("頂級英國柳木板球蝙蝠", "精心挑選輕量且彈性極佳的柳木，極致的擊球甜蜜點，幫助擊球員輕鬆將球擊出邊界。")},
	{"Cricket Helmet", ("抗衝擊板球防護頭盔", "高強度外殼搭配堅固的鋼製護網，內部配備吸震海綿，全面保護擊球員頭部安全。")},
	{"Cricket Wicket", ("實木板球三柱門套組", "堅固耐用的實木 stump 與 bail，防守方的靈魂核心，板球賽事不可或缺的標準配備。")},
	{"Feather Shuttlecock", ("比賽級鵝毛羽毛球", "精選優質鵝毛與複合軟木球頭，飛行軌跡極致穩定不飄移，殺球速度感十足。")},
	{"Football", ("FIFA 認證標準足球", "無縫熱貼合技術確保球體完美圓潤不吸水，提供極佳的腳感與精準的飛行軌跡。")},
	{"Golf Ball", ("遠距低風阻高爾夫球", "專利空氣動力學雙渦流凹痕設計，有效降低風阻並提升極致飛行距離，果嶺上的致勝關鍵。")},
	{"Iron Golf", ("高容錯率高爾夫鐵桿", "低重心設計結合超大擊球甜蜜點，讓初學者也能輕鬆擊出高飛與筆直的好球。")},
	{"Metal Baseball Bat", ("高彈力鋁合金棒球棒", "輕量化鋁合金材質，揮擊速度更快，清脆的擊球音效與超強的反彈力，轟出全壘打的秘密武器。")},
	{"Tennis Ball", ("高彈性耐磨網球", "採用高密度耐磨織布與增壓內膽，提供持久穩定的彈跳力，適合各種硬地與紅土球場。")},
	{"Tennis Racket", ("碳纖維避震網球拍", "航太級碳纖維打造，超輕量且具備極佳減震效果，大幅降低手臂負擔，揮拍更具爆發力。")},
	{"Volleyball", ("超軟皮室內排球", "超柔軟微纖維表皮，有效減輕接球時的手臂痛感，飛行穩定，是排球校隊與聯賽首選。")},

    // === 🕶️ 太陽眼鏡 (Sunglasses) ===
    {"Black Sun Glasses", ("極黑經典偏光太陽眼鏡", "100% 阻擋紫外線並消除眩光，無論開車或戶外活動都能擁有最清晰舒適的視覺。")},
	{"Classic Sun Glasses", ("復古金屬方框墨鏡", "好萊塢明星最愛的經典不敗款，修飾臉型效果極佳，瞬間提升整體穿搭的時尚氣場。")},
	{"Green and Black Glasses", ("黑綠雙色漸層飛行員墨鏡", "大膽吸睛的漸層配色，展現強烈街頭潮流感，為您的夏日穿搭注入滿滿酷炫活力。")},
	{"Party Glasses", ("派對造型發光眼鏡", "內建 LED 炫彩燈光與趣味造型，一戴上立刻成為派對與音樂節最引人注目的焦點。")},
	{"Sunglasses", ("抗 UV400 時尚太陽眼鏡", "輕量化鏡架無壓迫感，全面防護雙眼免受紫外線傷害，夏日海島度假的必備單品。")},

    // === 🚗 汽車 (Cars) ===
    {"300 Touring", ("Chrysler 300 Touring 房車", "美式豪華房車代表作。霸氣的車頭水箱護罩與寬敞奢華的內裝，盡顯總裁級的不凡氣度。")},
	{"Charger SXT RWD", ("Dodge Charger SXT 後驅版", "純正美式肌肉車血統！強悍的馬力輸出與極具侵略性的車身線條，點燃您的熱血駕駛魂。")},
	{"Dodge Hornet GT Plus", ("Dodge Hornet GT 鋼砲休旅", "兼具休旅車的實用空間與小鋼砲的敏捷操控，帥氣外型讓您在城市穿梭中超級吸睛。")},
	{"Durango SXT RWD", ("Dodge Durango 七人座休旅", "結合肌肉車的性能與七人座的超大靈活空間，滿足全家出遊與拖曳重物的全方位需求。")},
	{"Pacifica Touring", ("Chrysler Pacifica 頂級保母車", "極致尊榮的座艙享受，配備電動滑門與豐富影音娛樂系統，為家人打造最舒適的移動城堡。")},
    
    // === 🛵 機車 (Motorcycles) ===
    {"Generic Motorcycle", ("經典復古國民檔車", "低座高且易於保養，省油耐操的經典引擎，無論日常通勤或假日輕檔車小跑都非常適合。")},
	{"Kawasaki Z800", ("Kawasaki Z800 運動街車", "極具侵略性的「淒」肌肉外型，搭載四缸引擎爆發綿密強悍動力，重機迷的夢想坐騎。")},
	{"MotoGP CI.H1", ("MotoGP 廠車級仿賽重機", "完整移植賽道頂尖科技，極致輕量化車架與猛獸級馬力，帶您體驗貼地飛行的極速快感。")},
	{"Scooter Motorcycle", ("都會通勤白牌速克達", "超大車廂置物空間與靈活的車身，起步輕快且極度省油，應付台灣擁擠市區交通的最佳利器。")},
	{"Sportbike Motorcycle", ("流線擾流全尺寸仿賽", "配備空氣動力學整流罩與低趴戰鬥坐姿，提供極佳的高速穩定性，滿足對速度的絕對渴望。")},
    
    // === 🧼 身體沐浴 (Body Wash & Lotion) ===
    {"Attitude Super Leaves Hand Soap", ("Attitude 超級葉子天然洗手乳", "萃取天然旱金蓮葉精華，溫和清潔雙手同時帶來深層保濕，散發清新淡雅的植萃香氣。")},
	{"Olay Ultra Moisture Shea Butter Body Wash", ("Olay 乳木果油極致滋潤沐浴乳", "注入高濃度乳木果油精華，洗後肌膚宛如塗過乳液般柔嫩絲滑，徹底告別乾燥脫屑。")},
	{"Vaseline Men Body and Face Lotion", ("凡士林男士臉部與身體潤膚乳", "專為男士肌膚設計的清爽無油配方，15秒快速吸收不黏膩，全天候防護乾燥與粗糙。")}
		};

		private const string FALLBACK_DESCRIPTION_TEMPLATE = "我們嚴選的 {0}，為您帶來獨特的生活體驗。原廠特色介紹：{1}";

		private class DummyJsonResponse
		{
			[JsonPropertyName("products")]
			public List<DummyProduct> Products { get; set; }
		}

		private class DummyProduct
		{
			[JsonPropertyName("title")] public string Title { get; set; }
			[JsonPropertyName("description")] public string Description { get; set; }
			[JsonPropertyName("price")] public decimal Price { get; set; }
			[JsonPropertyName("stock")] public int Stock { get; set; }
			[JsonPropertyName("category")] public string Category { get; set; }
			[JsonPropertyName("brand")] public string Brand { get; set; }
			[JsonPropertyName("images")] public List<string> Images { get; set; } = new();
		}

		public static async Task SeedAsync(ISpanShopDBContext context) // 記得確認你的 DbContext 名稱
		{
			if (context.Products.Any()) return;

			try
			{
				var dummyProducts = await FetchProductsFromApiAsync();
				if (dummyProducts == null || dummyProducts.Count == 0) return;

				var store = EnsureStoreExists(context);

				// 第三步：動態建置「多層次分類」與「品牌」
				var categories = ExtractAndCreateHierarchyCategories(context, dummyProducts);
				var brands = ExtractAndCreateBrands(context, dummyProducts);
				context.SaveChanges();

				var products = new List<Product>();
				foreach (var dummy in dummyProducts)
				{
					// 取得子分類
					var childCatName = CategoryHierarchyMap.ContainsKey(dummy.Category)
						? CategoryHierarchyMap[dummy.Category].ChildName
						: char.ToUpper(dummy.Category[0]) + dummy.Category.Substring(1);
					var category = categories.FirstOrDefault(c => c.Name == childCatName);

					// 取得翻譯後的品牌
					var rawBrand = dummy.Brand ?? "原廠直營";
					var translatedBrandName = BrandTranslationMap.ContainsKey(rawBrand)
						? BrandTranslationMap[rawBrand]
						: rawBrand;
					var brand = brands.FirstOrDefault(b => b.Name == translatedBrandName);

					int priceInTwd = (int)(dummy.Price * USD_TO_TWD);

					string productName;
					string productDescription;
					if (ProductTranslationMap.ContainsKey(dummy.Title))
					{
						var translation = ProductTranslationMap[dummy.Title];
						productName = translation.Title;
						productDescription = translation.Description;
					}
					else
					{
						productName = dummy.Title;
						productDescription = string.Format(FALLBACK_DESCRIPTION_TEMPLATE, dummy.Title, dummy.Description);
					}

					var product = new Product
					{
						StoreId = store.Id,
						CategoryId = category?.Id ?? categories.First().Id,
						BrandId = brand?.Id ?? brands.First().Id,
						Name = productName,
						Description = productDescription,
						MinPrice = priceInTwd,
						MaxPrice = priceInTwd,
						Status = 1,
						CreatedAt = DateTime.Now,
						UpdatedAt = DateTime.Now,
						ProductImages = new List<ProductImage>(),
						ProductVariants = new List<ProductVariant>()
					};

					// 處理圖片
					if (dummy.Images != null && dummy.Images.Count > 0)
					{
						for (int i = 0; i < dummy.Images.Count; i++)
						{
							product.ProductImages.Add(new ProductImage
							{
								ImageUrl = dummy.Images[i],
								IsMain = (i == 0),
								SortOrder = i
							});
						}
					}

					// 處理預設規格
					product.ProductVariants.Add(new ProductVariant
					{
						SkuCode = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
						VariantName = "標準版",
						SpecValueJson = "{}",
						Price = priceInTwd,
						Stock = _random.Next(50, 201),
						SafetyStock = 10,
						IsDeleted = false
					});
					// === 資料倍增術：將 1 筆真實商品變種成 5 筆 ===
					string[] suffixes = { "", " (2025 全新升級版)", " (特仕限量版)", " (海外平輸版)", " - 聯名精裝版" };

					for (int k = 0; k < suffixes.Length; k++)
					{
						var clonedProduct = new Product
						{
							StoreId = product.StoreId,
							CategoryId = product.CategoryId,
							BrandId = product.BrandId,
							Name = product.Name + suffixes[k], // 加上變種後綴
							Description = product.Description,

							// 使用 ?? 0 確保如果沒價格就當作 0，避免 null 報錯
							MinPrice = (product.MinPrice ?? 0) + (k * 150),
							MaxPrice = (product.MaxPrice ?? 0) + (k * 150),

							// 明確轉型成 byte (強制轉型)
							Status = (byte)_random.Next(0, 2),

							CreatedAt = DateTime.Now.AddDays(-_random.Next(1, 100)), // 隨機產生過去 100 天內的建檔日期
							UpdatedAt = DateTime.Now,
							ProductImages = product.ProductImages.Select(img => new ProductImage
							{
								ImageUrl = img.ImageUrl,
								IsMain = img.IsMain,
								SortOrder = img.SortOrder
							}).ToList(),
							ProductVariants = new List<ProductVariant>
								{
									new ProductVariant
									{
										SkuCode = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
										VariantName = "標準版",
										SpecValueJson = "{}",
                
										// 一樣處理 Nullable 轉型問題
										Price = (product.MinPrice ?? 0) + (k * 150),

										Stock = _random.Next(10, 500), // 打亂庫存數量
										SafetyStock = 10,
										IsDeleted = false
									}
								}
							};
						products.Add(clonedProduct);
					}
				}

				// 將最後 15 筆設為「待審核」測試資料
				var pendingReviewProducts = products.Skip(Math.Max(0, products.Count - 15)).ToList();
				foreach (var p in pendingReviewProducts)
				{
					p.Status = 2;
					if (!p.Name.StartsWith("[待審核] "))
						p.Name = "[待審核] " + p.Name;
				}

				context.Products.AddRange(products);
				await context.SaveChangesAsync();
				Console.WriteLine($"✅ 成功匯入 {products.Count} 筆商品到資料庫");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ 播種過程出錯：{ex.Message}");
				throw;
			}
		}

		/// <summary>
		/// 確保資料庫中隨時維持至少 15 筆「待審核 (Status==2)」的商品，供測試使用。
		/// 每次應用程式啟動時呼叫，不足時自動從其他商品補足差額。
		/// </summary>
		public static async Task EnsurePendingProductsAsync(ISpanShopDBContext context)
		{
			var currentCount = context.Products.Count(p => p.Status == 2);
			if (currentCount >= 15)
			{
				Console.WriteLine($"ℹ️  待審核商品已有 {currentCount} 筆，無需補充。");
				return;
			}

			var needed = 15 - currentCount;

			// 抓取不是待審核、且名稱未加前綴的商品來補足
			var candidates = context.Products
				.Where(p => p.Status != 2 && !p.Name.StartsWith("[待審核] "))
				.Take(needed)
				.ToList();

			foreach (var p in candidates)
			{
				p.Status = 2;
				p.Name = "[待審核] " + p.Name;
				p.UpdatedAt = DateTime.Now;
			}

			await context.SaveChangesAsync();
			Console.WriteLine($"✅ 已補充 {candidates.Count} 筆待審核商品，目前共 {currentCount + candidates.Count} 筆");
		}

		private static async Task<List<DummyProduct>> FetchProductsFromApiAsync()
		{
			using var client = new HttpClient();
			var response = await client.GetAsync(DUMMYJSON_URL);
			response.EnsureSuccessStatusCode();
			var jsonContent = await response.Content.ReadAsStringAsync();
			var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
			var dummyResponse = System.Text.Json.JsonSerializer.Deserialize<DummyJsonResponse>(jsonContent, options);
			return dummyResponse?.Products ?? new List<DummyProduct>();
		}

		private static Store EnsureStoreExists(ISpanShopDBContext context) // 注意這裡的 DBContext 名稱要跟你的一致
		{
			var store = context.Stores.FirstOrDefault();
			if (store != null)
			{
				return store;
			}

			// 建立或取得 Role
			var role = context.Roles.FirstOrDefault();
			if (role == null)
			{
				role = new Role
				{
					RoleName = "Seller",
					Description = "賣家角色"
				};
				context.Roles.Add(role);
				context.SaveChanges();
			}

			// 建立 User
			var user = new User
			{
				RoleId = role.Id,
				Account = "dataseed_seller",
				Password = "hashed_password_placeholder",
				Email = "dataseed@example.com",
				IsConfirmed = true,
				IsBlacklisted = false,
				IsSeller = true,
				CreatedAt = DateTime.Now,
				UpdatedAt = DateTime.Now
			};
			context.Users.Add(user);
			context.SaveChanges();

			// 建立 Store
			store = new Store
			{
				UserId = user.Id,
				StoreName = "原廠直營",
				Description = "精選商品，品質保證",
				IsVerified = true,
				CreatedAt = DateTime.Now
			};
			context.Stores.Add(store);
			context.SaveChanges();

			return store;
		}

		/// <summary>
		/// ★★★ 建立多層次分類的核心邏輯 ★★★
		/// </summary>
		private static List<Category> ExtractAndCreateHierarchyCategories(ISpanShopDBContext context, List<DummyProduct> dummyProducts)
		{
			var flatCategoryList = new List<Category>();
			var apiCategories = dummyProducts.Select(p => p.Category).Distinct().ToList();

			foreach (var apiCat in apiCategories)
			{
				string parentName = apiCat;
				string childName = apiCat;

				if (CategoryHierarchyMap.ContainsKey(apiCat))
				{
					parentName = CategoryHierarchyMap[apiCat].ParentName;
					childName = CategoryHierarchyMap[apiCat].ChildName;
				}

				// 1. 處理主分類 (Parent)
				var parentCategory = context.Categories.FirstOrDefault(c => c.Name == parentName);
				if (parentCategory == null)
				{
					parentCategory = new Category
					{
						Name = parentName,
						Sort = 0,
						IsVisible = true,
						ParentId = null // ★ 告訴資料庫：主分類沒有爸爸
					};
					context.Categories.Add(parentCategory);
					context.SaveChanges(); // 先存檔才能拿到主分類的 ID
				}

				if (!flatCategoryList.Any(c => c.Name == parentName))
					flatCategoryList.Add(parentCategory);

				// 2. 處理子分類 (Child)
				var childCategory = context.Categories.FirstOrDefault(c => c.Name == childName);
                if (childCategory == null)
                {
                    childCategory = new Category
                    {
                        Name = childName,
                        Sort = 0,
                        IsVisible = true,
                        ParentId = parentCategory.Id // ★ 告訴資料庫：這個子分類的爸爸是剛剛建好的主分類
                    };
                    context.Categories.Add(childCategory);
					context.SaveChanges();
				}

				if (!flatCategoryList.Any(c => c.Name == childName))
					flatCategoryList.Add(childCategory);
			}

			return flatCategoryList;
		}

		private static List<Brand> ExtractAndCreateBrands(ISpanShopDBContext context, List<DummyProduct> dummyProducts)
		{
			var brands = new List<Brand>();
			var rawBrandNames = dummyProducts.Select(p => p.Brand ?? "原廠直營").Distinct().ToList();

			foreach (var rawName in rawBrandNames)
			{
				// 使用翻譯字典轉換
				var translatedName = BrandTranslationMap.ContainsKey(rawName)
					? BrandTranslationMap[rawName]
					: rawName;

				var existing = context.Brands.FirstOrDefault(b => b.Name == translatedName);
				if (existing == null)
				{
					var brand = new Brand
					{
						Name = translatedName,
						Description = $"{translatedName} 官方直營品牌",
						LogoUrl = "https://via.placeholder.com/64x64",
						Sort = 0,
						IsVisible = true,
						IsDeleted = false
					};
					context.Brands.Add(brand);
					context.SaveChanges(); // 立即存檔確保不重複
					brands.Add(brand);
				}
				else
				{
					if (!brands.Any(b => b.Id == existing.Id))
						brands.Add(existing);
				}
			}
			return brands;
		}
	}
}
``n
### 檔案: .\ISpanShop.MVC\Controllers\CategorySpecsController.cs
`${ext}
using Microsoft.AspNetCore.Mvc;

namespace ISpanShop.MVC.Controllers
{
	public class CategorySpecsController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}

``n
### 檔案: .\ISpanShop.MVC\Controllers\HomeController.cs
`${ext}
using ISpanShop.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ISpanShop.MVC.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}

``n
### 檔案: .\ISpanShop.MVC\Controllers\ProductsController.cs
`${ext}
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using ISpanShop.Models.DTOs;
using ISpanShop.Services.Interfaces;
using ISpanShop.MVC.Models.ViewModels;

namespace ISpanShop.MVC.Controllers
{
    /// <summary>
    /// 商品管理控制器 - 提供 MVC 後台商品管理功能
    /// </summary>
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;

        /// <summary>
        /// 建構子 - 注入 ProductService
        /// </summary>
        /// <param name="productService">商品 Service</param>
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// 商品列表 - 全站商品總覽（支援多維度篩選：分類、關鍵字、商家、狀態 與 分頁）
        /// </summary>
        /// <param name="parentCategoryId">主分類 ID</param>
        /// <param name="categoryId">子分類 ID</param>
        /// <param name="keyword">關鍵字搜尋</param>
        /// <param name="storeId">商家 ID</param>
        /// <param name="status">商品狀態</param>
        /// <param name="page">頁碼（從 1 開始）</param>
        /// <returns>商品列表 View</returns>
        public IActionResult Index(int? parentCategoryId, int? categoryId, string? keyword, int? storeId, int? brandId, int? status, DateTime? startDate, DateTime? endDate, int page = 1)
        {
            var criteria = new ProductSearchCriteria
            {
                ParentCategoryId = parentCategoryId,
                CategoryId = categoryId,
                Keyword = keyword,
                StoreId = storeId,
                BrandId = brandId,
                Status = status,
                StartDate = startDate,
                EndDate = endDate,
                PageNumber = page,
                PageSize = 10
            };

            // 取得分頁商品列表
            var pagedDtos = _productService.GetProductsPaged(criteria);

            // 將 DTO 轉換為 ViewModel
            var pagedVm = PagedResult<ProductListVm>.Create(
                pagedDtos.Data.Select(dto => new ProductListVm
                {
                    Id = dto.Id,
                    StoreName = dto.StoreName,
                    CategoryName = dto.CategoryName,
                    BrandName = dto.BrandName,
                    Name = dto.Name,
                    MinPrice = dto.MinPrice,
                    MaxPrice = dto.MaxPrice,
                    Status = dto.Status,
                    MainImageUrl = dto.MainImageUrl,
                    CreatedAt = dto.CreatedAt
                }).ToList(),
                pagedDtos.TotalCount,
                pagedDtos.CurrentPage,
                pagedDtos.PageSize
            );

            // 取得所有分類並區分主/子分類
            var allCategories = _productService.GetAllCategories().ToList();
            ViewBag.ParentCategories = allCategories.Where(c => c.ParentId == null).ToList();
            ViewBag.AllSubCategories = allCategories.Where(c => c.ParentId != null).ToList();
            ViewBag.CurrentParentCategoryId = parentCategoryId;
            ViewBag.CurrentCategoryId = categoryId;

            // 取得商家清單並傳給前端
            var stores = _productService.GetStoreOptions().ToList();
            ViewBag.Stores = stores;

            // 取得品牌清單並傳給前端
            var brands = _productService.GetBrandOptions().ToList();
            ViewBag.Brands = brands;

            ViewBag.CurrentKeyword = keyword;
            ViewBag.CurrentStoreId = storeId;
            ViewBag.CurrentBrandId = brandId;
            ViewBag.CurrentStatus = status;
            ViewBag.CurrentStartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.CurrentEndDate = endDate?.ToString("yyyy-MM-dd");

            return View(pagedVm);
        }

        /// <summary>
        /// 待審核商品列表
        /// </summary>
        /// <returns>待審核商品列表 View</returns>
        public IActionResult PendingReview()
        {
            // 待審核商品（Status == 2）
            var pendingProductDtos = _productService.GetPendingProducts();
            var viewModels = pendingProductDtos.Select(dto => new ProductReviewListVm
            {
                Id          = dto.Id,
                StoreId     = dto.StoreId,
                CategoryName = dto.CategoryName,
                BrandName   = dto.BrandName,
                StoreName   = dto.StoreName,
                Name        = dto.Name,
                Description = dto.Description,
                Status      = dto.Status,
                CreatedAt   = dto.CreatedAt,
                UpdatedAt   = dto.UpdatedAt,
                MainImageUrl = dto.MainImageUrl
            }).ToList();

            // 近期退回紀錄（Status == 3，取最新 10 筆）
            var rejectedDtos = _productService.GetRecentRejectedProducts(10);
            ViewBag.RejectedRecords = rejectedDtos.Select(dto => new ProductReviewListVm
            {
                Id           = dto.Id,
                StoreId      = dto.StoreId,
                StoreName    = dto.StoreName,
                CategoryName = dto.CategoryName,
                Name         = dto.Name,
                Status       = dto.Status,
                RejectReason = dto.RejectReason,
                UpdatedAt    = dto.UpdatedAt,
                MainImageUrl = dto.MainImageUrl
            }).ToList();

            return View(viewModels);
        }

        /// <summary>
        /// 變更商品狀態
        /// </summary>
        /// <param name="id">商品 ID</param>
        /// <param name="newStatus">新的狀態值</param>
        /// <returns>重新導向至待審核列表</returns>
        [HttpPost]
        public IActionResult ChangeStatus(int id, byte newStatus)
        {
            _productService.ChangeProductStatus(id, newStatus);
            return RedirectToAction(nameof(PendingReview));
        }

        /// <summary>
        /// [AJAX] 核准商品審核 - 將狀態設為 1 (上架)
        /// </summary>
        [HttpPost]
        public IActionResult ApproveProduct(int id)
        {
            _productService.ApproveProduct(id);
            return Json(new { success = true, message = "商品已核准上架。" });
        }

        /// <summary>
        /// [AJAX] 退回商品審核 - 將狀態設為 3 (審核退回)
        /// </summary>
        [HttpPost]
        public IActionResult RejectProduct(int id, string reason)
        {
            _productService.RejectProduct(id, reason);
            return Json(new { success = true, message = $"商品已退回。退回原因：{reason}" });
        }

        /// <summary>
        /// [AJAX] 根據主分類取得對應子分類清單，供篩選器連動使用
        /// </summary>
        /// <param name="parentId">主分類 ID</param>
        /// <returns>JSON 格式子分類清單 [{ id, name }]</returns>
        [HttpGet]
        public IActionResult GetSubCategories(int parentId)
        {
            var subs = _productService.GetAllCategories()
                .Where(c => c.ParentId == parentId)
                .Select(c => new { id = c.Id, name = c.Name });
            return Json(subs);
        }

        /// <summary>
        /// [AJAX] 根據子分類取得對應品牌清單，供篩選器連動使用
        /// </summary>
        /// <param name="categoryId">子分類 ID；為 null 時回傳全部品牌</param>
        /// <returns>JSON 格式品牌清單 [{ id, name }]</returns>
        [HttpGet]
        public IActionResult GetBrandsByCategory(int? categoryId)
        {
            var brands = _productService.GetBrandsByCategory(categoryId)
                .Select(b => new { id = b.Id, name = b.Name });
            return Json(brands);
        }

        /// <summary>
        /// [AJAX] 批次更新商品上下架狀態
        /// </summary>
        /// <param name="dto">包含商品 ID 集合與目標狀態</param>
        /// <returns>JSON 格式結果 { success, message, count }</returns>
        [HttpPost]
        public async Task<IActionResult> BatchUpdateStatus([FromBody] ProductBatchUpdateStatusDto dto)
        {
            if (dto == null || dto.ProductIds == null || dto.ProductIds.Count == 0)
                return Json(new { success = false, message = "請至少勾選一筆商品。", count = 0 });

            var count = await _productService.UpdateBatchStatusAsync(dto.ProductIds, dto.TargetStatus);
            var action = dto.TargetStatus == 1 ? "上架" : "下架";
            return Json(new { success = true, message = $"成功將 {count} 筆商品設為{action}。", count });
        }

        /// <summary>
        /// 商品詳情 - 顯示完整的商品資訊、圖片與規格
        /// </summary>
        /// <param name="id">商品 ID</param>
        /// <returns>商品詳情 View</returns>
        public IActionResult Details(int id)
        {
            // 從 Service 取得 DTO
            var productDto = _productService.GetProductDetail(id);

            // 若找不到資料，返回 NotFound
            if (productDto == null)
            {
                return NotFound();
            }

            // 將 DTO 轉換為 ViewModel
            var viewModel = new ProductDetailVm
            {
                Id = productDto.Id,
                Name = productDto.Name,
                StoreName = productDto.StoreName,
                CategoryName = productDto.CategoryName,
                BrandName = productDto.BrandName,
                Description = productDto.Description,
                Status = productDto.Status,
                Images = productDto.Images,
                Variants = productDto.Variants.Select(v => new ProductVariantDetailVm
                {
                    SkuCode = v.SkuCode,
                    VariantName = v.VariantName,
                    Price = v.Price,
                    Stock = v.Stock,
                    SpecValueJson = v.SpecValueJson
                }).ToList()
            };

            return View(viewModel);
        }
    }
}

``n
### 檔案: .\ISpanShop.MVC\Models\CategorySpecCreateVm.cs
`${ext}
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISpanShop.MVC.Models.ViewModels
{
    /// <summary>
    /// 分類規格新增 ViewModel - 用於接收前端新增表單資料
    /// </summary>
    public class CategorySpecCreateVm
    {
        /// <summary>
        /// 規格名稱 (例如：顏色、尺寸、容量)
        /// </summary>
        [Required(ErrorMessage = "規格名稱為必填")]
        [StringLength(50, ErrorMessage = "規格名稱不可超過 50 字元")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 輸入方式 (text=文字輸入, select=下拉選單, checkbox=多選框, radio=單選框)
        /// </summary>
        [Required(ErrorMessage = "輸入方式為必填")]
        public string InputType { get; set; } = "text";

        /// <summary>
        /// 是否為必填項目
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// 排序順序
        /// </summary>
        [Range(0, 9999, ErrorMessage = "排序順序需介於 0 到 9999")]
        public int SortOrder { get; set; }

        /// <summary>
        /// 選項名稱列表 (當 InputType 為 select/checkbox/radio 時使用)
        /// </summary>
        public List<string> Options { get; set; } = new List<string>();
    }
}

``n
### 檔案: .\ISpanShop.MVC\Models\CategorySpecEditVm.cs
`${ext}
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISpanShop.MVC.Models.ViewModels
{
    /// <summary>
    /// 分類規格編輯 ViewModel - 用於接收前端編輯表單資料
    /// </summary>
    public class CategorySpecEditVm
    {
        /// <summary>
        /// 規格 ID
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// 規格名稱 (例如：顏色、尺寸、容量)
        /// </summary>
        [Required(ErrorMessage = "規格名稱為必填")]
        [StringLength(50, ErrorMessage = "規格名稱不可超過 50 字元")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 輸入方式 (text=文字輸入, select=下拉選單, checkbox=多選框, radio=單選框)
        /// </summary>
        [Required(ErrorMessage = "輸入方式為必填")]
        public string InputType { get; set; } = "text";

        /// <summary>
        /// 是否為必填項目
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// 排序順序
        /// </summary>
        [Range(0, 9999, ErrorMessage = "排序順序需介於 0 到 9999")]
        public int SortOrder { get; set; }

        /// <summary>
        /// 選項名稱列表 (當 InputType 為 select/checkbox/radio 時使用)
        /// </summary>
        public List<string> Options { get; set; } = new List<string>();
    }
}

``n
### 檔案: .\ISpanShop.MVC\Models\ErrorViewModel.cs
`${ext}
namespace ISpanShop.MVC.Models
{
	public class ErrorViewModel
	{
		public string? RequestId { get; set; }

		public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
	}
}

``n
### 檔案: .\ISpanShop.MVC\Models\ProductDetailVm.cs
`${ext}
using System.Collections.Generic;

namespace ISpanShop.MVC.Models.ViewModels
{
    /// <summary>
    /// 商品詳情 ViewModel - 用於商品詳情頁展示（包含完整的圖片與規格）
    /// </summary>
    public class ProductDetailVm
    {
        /// <summary>
        /// 商品 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 商品名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 商店名稱
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        /// 分類名稱
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// 品牌名稱
        /// </summary>
        public string BrandName { get; set; }

        /// <summary>
        /// 商品描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 商品狀態 (1=已上架, 2=待審核, 0=下架)
        /// </summary>
        public byte? Status { get; set; }

        /// <summary>
        /// 商品狀態文字顯示
        /// </summary>
        public string StatusText
        {
            get => Status switch
            {
                1 => "已上架",
                2 => "待審核",
                0 => "下架",
                _ => "未知"
            };
        }

        /// <summary>
        /// 商品狀態 Badge 樣式
        /// </summary>
        public string StatusBadgeClass
        {
            get => Status switch
            {
                1 => "badge-success",
                2 => "badge-warning",
                0 => "badge-danger",
                _ => "badge-secondary"
            };
        }

        /// <summary>
        /// 商品圖片 URL 列表
        /// </summary>
        public List<string> Images { get; set; } = new();

        /// <summary>
        /// 商品規格變體列表
        /// </summary>
        public List<ProductVariantDetailVm> Variants { get; set; } = new();
    }
}

``n
### 檔案: .\ISpanShop.MVC\Models\ProductListVm.cs
`${ext}
using System;

namespace ISpanShop.MVC.Models.ViewModels
{
    /// <summary>
    /// 商品列表 ViewModel - 用於全站商品總覽頁面
    /// </summary>
    public class ProductListVm
    {
        /// <summary>
        /// 商品 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 商店名稱
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        /// 分類名稱
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// 品牌名稱
        /// </summary>
        public string BrandName { get; set; }

        /// <summary>
        /// 商品名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 最低價格
        /// </summary>
        public decimal? MinPrice { get; set; }

        /// <summary>
        /// 最高價格
        /// </summary>
        public decimal? MaxPrice { get; set; }

        /// <summary>
        /// 商品狀態 (1=已上架, 2=待審核, 0=下架)
        /// </summary>
        public byte? Status { get; set; }

        /// <summary>
        /// 主圖 URL
        /// </summary>
        public string MainImageUrl { get; set; }

        /// <summary>
        /// 建檔日期
        /// </summary>
        public DateTime? CreatedAt { get; set; }
    }
}

``n
### 檔案: .\ISpanShop.MVC\Models\ProductReviewListVm.cs
`${ext}
using System;

namespace ISpanShop.MVC.Models.ViewModels
{
    /// <summary>
    /// 商品審核列表 ViewModel - 用於待審核商品列表頁面
    /// </summary>
    public class ProductReviewListVm
    {
        /// <summary>
        /// 商品 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 店鋪 ID
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// 分類名稱
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// 品牌名稱
        /// </summary>
        public string BrandName { get; set; }

        /// <summary>
        /// 商店名稱
        /// </summary>
        public string? StoreName { get; set; }

        /// <summary>
        /// 商品名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 商品描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 商品狀態 (0=下架, 1=上架, 2=待審核, 3=審核退回)
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// 退回原因（Status==3 時填入）
        /// </summary>
        public string? RejectReason { get; set; }

        /// <summary>
        /// 最後更新時間（退回時間）
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// 主圖 URL
        /// </summary>
        public string? MainImageUrl { get; set; }
    }
}

``n
### 檔案: .\ISpanShop.MVC\Models\ProductVariantDetailVm.cs
`${ext}
namespace ISpanShop.MVC.Models.ViewModels
{
    /// <summary>
    /// 商品規格詳情 ViewModel - 用於商品詳情頁展示
    /// </summary>
    public class ProductVariantDetailVm
    {
        /// <summary>
        /// SKU 代碼
        /// </summary>
        public string SkuCode { get; set; }

        /// <summary>
        /// 規格名稱
        /// </summary>
        public string VariantName { get; set; }

        /// <summary>
        /// 價格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 庫存量
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        /// 規格值 JSON (如 {"color":"黑","size":"M"})
        /// </summary>
        public string SpecValueJson { get; set; }
    }
}

``n
### 檔案: .\ISpanShop.MVC\Views\CategorySpecs\Create.cshtml
`${ext}
@*
    For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
*@
@{
}

``n
### 檔案: .\ISpanShop.MVC\Views\CategorySpecs\Index.cshtml
`${ext}
@*
    For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
*@
@{
}

``n
### 檔案: .\ISpanShop.MVC\Views\Home\Index.cshtml
`${ext}
@{
    ViewData["Title"] = "Home Page";
}

<div class="text-center">
    <h1 class="display-4">Welcome</h1>
    <p>Learn about <a href="https://learn.microsoft.com/aspnet/core">building Web apps with ASP.NET Core</a>.</p>
</div>

``n
### 檔案: .\ISpanShop.MVC\Views\Home\Privacy.cshtml
`${ext}
@{
    ViewData["Title"] = "Privacy Policy";
}
<h1>@ViewData["Title"]</h1>

<p>Use this page to detail your site's privacy policy.</p>

``n
### 檔案: .\ISpanShop.MVC\Views\Products\Details.cshtml
`${ext}
@model ISpanShop.MVC.Models.ViewModels.ProductDetailVm

@{
    ViewData["Title"] = $"商品詳情 - {Model?.Name ?? "未知"}";
    // 狀態防呆與高質感色彩設定
    var isPublished = Model.Status == 1;
    var statusText = isPublished ? "已上架販售" : "未上架 / 停售";
    var statusBg = isPublished ? "bg-success-soft text-success" : "bg-secondary-soft text-secondary";
    var statusIcon = isPublished ? "bi-check-circle-fill" : "bi-dash-circle-fill";
}

<div class="container-lg py-5 font-noto">
    <div class="d-flex justify-content-between align-items-center mb-4">
        <a href="javascript:history.back()" class="btn btn-back">
            <i class="bi bi-arrow-left me-2"></i>返回商品總覽
        </a>
        <a href="@Url.Action("Edit", new { id = Model.Id })" class="btn btn-edit">
            <i class="bi bi-pencil-square me-2"></i>編輯商品
        </a>
    </div>

    <div class="row g-5 mb-5">
        <div class="col-lg-5">
            <div class="product-gallery-card">
                @if (Model.Images != null && Model.Images.Any())
                {
                    <div id="productCarousel" class="carousel slide" data-bs-ride="carousel">
                        <div class="carousel-inner rounded-4 overflow-hidden">
                            @for (int i = 0; i < Model.Images.Count; i++)
                            {
                                <div class="carousel-item @(i == 0 ? "active" : "")">
                                    <img src="@Model.Images[i]" class="d-block w-100 gallery-main-img" alt="商品圖片 @(i + 1)">
                                </div>
                            }
                        </div>
                        @if (Model.Images.Count > 1)
                        {
                            <button class="carousel-control-prev gallery-btn" type="button" data-bs-target="#productCarousel" data-bs-slide="prev">
                                <span class="carousel-control-prev-icon" aria-hidden="true"></span>
                                <span class="visually-hidden">上一張</span>
                            </button>
                            <button class="carousel-control-next gallery-btn" type="button" data-bs-target="#productCarousel" data-bs-slide="next">
                                <span class="carousel-control-next-icon" aria-hidden="true"></span>
                                <span class="visually-hidden">下一張</span>
                            </button>
                        }
                    </div>

                    @if (Model.Images.Count > 1)
                    {
                        <div class="d-flex gap-2 mt-3 overflow-auto thumbnail-container py-1">
                            @for (int i = 0; i < Model.Images.Count; i++)
                            {
                                <img src="@Model.Images[i]" class="gallery-thumbnail" alt="縮圖 @(i + 1)"
                                     data-bs-target="#productCarousel" data-bs-slide-to="@i" />
                            }
                        </div>
                    }
                }
                else
                {
                    <div class="empty-image-placeholder rounded-4">
                        <i class="bi bi-image"></i>
                        <p>尚無商品圖片</p>
                    </div>
                }
            </div>
        </div>

        <div class="col-lg-7">
            <div class="d-flex flex-column h-100 justify-content-center px-lg-3">
                
                <div class="d-flex gap-2 mb-3">
                    @if (!string.IsNullOrEmpty(Model.CategoryName))
                    {
                        <span class="badge-soft bg-primary-soft text-primary">@Model.CategoryName</span>
                    }
                    @if (!string.IsNullOrEmpty(Model.BrandName))
                    {
                        <span class="badge-soft bg-info-soft text-info">@Model.BrandName</span>
                    }
                </div>

                <h1 class="display-5 fw-bold text-dark mb-3 tracking-tight">@Model.Name</h1>
                
                <div class="mb-4">
                    <span class="badge rounded-pill px-3 py-2 fs-6 @statusBg fw-normal shadow-sm">
                        <i class="bi @statusIcon"></i> @statusText
                    </span>
                    <span class="text-muted ms-3 fs-6"><i class="bi bi-shop me-1"></i> @Model.StoreName</span>
                </div>

                <hr class="divider-subtle mb-4" />

                <div class="mb-5">
                    <h6 class="text-uppercase text-muted fw-bold mb-3 tracking-wide fs-7">商品介紹</h6>
                    <p class="text-secondary lh-lg fs-6">
                        @(string.IsNullOrEmpty(Model.Description) ? "賣家尚未提供商品描述。" : Model.Description)
                    </p>
                </div>

                <div class="row g-3 mt-auto">
                    <div class="col-sm-6">
                        <div class="stat-card">
                            <div class="stat-icon bg-blue-soft text-blue"><i class="bi bi-box-seam"></i></div>
                            <div class="stat-info">
                                <span class="stat-label">規格數量</span>
                                <span class="stat-value">@(Model.Variants?.Count ?? 0) <small>個</small></span>
                            </div>
                        </div>
                    </div>
                    <div class="col-sm-6">
                        <div class="stat-card">
                            <div class="stat-icon bg-purple-soft text-purple"><i class="bi bi-images"></i></div>
                            <div class="stat-info">
                                <span class="stat-label">圖片數量</span>
                                <span class="stat-value">@(Model.Images?.Count ?? 0) <small>張</small></span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="spec-section-card mt-5">
        <h4 class="fw-bold mb-4 text-dark"><i class="bi bi-list-columns-reverse me-2 text-primary"></i>規格與庫存明細</h4>
        
        @if (Model.Variants != null && Model.Variants.Any())
        {
            <div class="table-responsive">
                <table class="table modern-table align-middle mb-0">
                    <thead>
                        <tr>
                            <th>SKU 代碼</th>
                            <th>規格名稱</th>
                            <th class="text-end">價格</th>
                            <th class="text-center">庫存狀態</th>
                        </tr>
                    </thead>
                    <tbody>
                        @foreach (var variant in Model.Variants)
                        {
                            <tr>
                                <td><code class="sku-badge">@variant.SkuCode</code></td>
                                <td class="fw-medium text-dark">@variant.VariantName</td>
                                <td class="text-end fw-bold text-primary fs-5">@variant.Price.ToString("C0")</td>
                                <td class="text-center">
                                    @if (variant.Stock > 10)
                                    {
                                        <span class="badge bg-success-soft text-success rounded-pill px-3">@variant.Stock 件</span>
                                    }
                                    else if (variant.Stock > 0)
                                    {
                                        <span class="badge bg-warning-soft text-warning rounded-pill px-3">緊張：@variant.Stock 件</span>
                                    }
                                    else
                                    {
                                        <span class="badge bg-danger-soft text-danger rounded-pill px-3">已售罄</span>
                                    }
                                </td>
                            </tr>
                        }
                    </tbody>
                </table>
            </div>
        }
        else
        {
            <div class="text-center py-5">
                <div class="empty-state-icon mb-3"><i class="bi bi-inboxes"></i></div>
                <h5 class="text-muted fw-normal">暫無規格資訊</h5>
            </div>
        }
    </div>
</div>

<style>
    /* 引入 Google 現代字體 */
    @@import url('https://fonts.googleapis.com/css2?family=Noto+Sans+TC:wght@300;400;500;700;900&display=swap');

    body {
        background-color: #f7f9fc; /* 非常柔和的背景色，讓白卡片凸顯 */
    }

    .font-noto {
        font-family: 'Noto Sans TC', sans-serif;
    }

    .tracking-tight { letter-spacing: -0.5px; }
    .tracking-wide { letter-spacing: 1.5px; }
    .fs-7 { font-size: 0.85rem; }

    /* 按鈕樣式 */
    .btn-back {
        background-color: #fff;
        color: #4a5568;
        border: 1px solid #e2e8f0;
        border-radius: 10px;
        padding: 0.5rem 1.2rem;
        font-weight: 500;
        transition: all 0.2s;
    }
    .btn-back:hover {
        background-color: #edf2f7;
        color: #1a202c;
    }

    .btn-edit {
        background-color: #2b6cb0;
        color: white;
        border-radius: 10px;
        padding: 0.5rem 1.5rem;
        font-weight: 500;
        box-shadow: 0 4px 6px rgba(43, 108, 176, 0.2);
        transition: all 0.2s;
    }
    .btn-edit:hover {
        background-color: #2c5282;
        color: white;
        transform: translateY(-1px);
        box-shadow: 0 6px 12px rgba(43, 108, 176, 0.3);
    }

    /* 圖片庫區塊 */
    .gallery-main-img {
        aspect-ratio: 1 / 1;
        object-fit: cover;
        background-color: #fff;
    }
    .gallery-thumbnail {
        width: 70px;
        height: 70px;
        object-fit: cover;
        border-radius: 8px;
        cursor: pointer;
        opacity: 0.6;
        transition: all 0.2s;
        border: 2px solid transparent;
    }
    .gallery-thumbnail:hover, .gallery-thumbnail.active {
        opacity: 1;
        border-color: #2b6cb0;
        transform: translateY(-2px);
    }
    .empty-image-placeholder {
        aspect-ratio: 1 / 1;
        background-color: #edf2f7;
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        color: #a0aec0;
    }
    .empty-image-placeholder i { font-size: 3rem; margin-bottom: 1rem; }

    /* 柔和徽章 (Soft Badges) - 高級感來源 */
    .badge-soft {
        padding: 0.4em 0.8em;
        border-radius: 6px;
        font-size: 0.8rem;
        font-weight: 600;
        letter-spacing: 0.5px;
    }
    .bg-primary-soft { background-color: #ebf8ff; }
    .text-primary { color: #3182ce !important; }
    
    .bg-info-soft { background-color: #e6fffa; }
    .text-info { color: #319795 !important; }

    .bg-success-soft { background-color: #c6f6d5; }
    .text-success { color: #276749 !important; }

    .bg-secondary-soft { background-color: #e2e8f0; }
    .text-secondary { color: #4a5568 !important; }

    .bg-warning-soft { background-color: #feebc8; }
    .text-warning { color: #c05621 !important; }

    .bg-danger-soft { background-color: #fed7d7; }
    .text-danger { color: #c53030 !important; }

    /* 數據小卡 */
    .stat-card {
        background: #fff;
        border: 1px solid #edf2f7;
        border-radius: 16px;
        padding: 1.2rem;
        display: flex;
        align-items: center;
        gap: 1rem;
        box-shadow: 0 2px 10px rgba(0,0,0,0.01);
    }
    .stat-icon {
        width: 48px;
        height: 48px;
        border-radius: 12px;
        display: flex;
        align-items: center;
        justify-content: center;
        font-size: 1.5rem;
    }
    .bg-blue-soft { background-color: #ebf8ff; }
    .text-blue { color: #2b6cb0; }
    .bg-purple-soft { background-color: #faf5ff; }
    .text-purple { color: #6b46c1; }
    
    .stat-info { display: flex; flex-direction: column; }
    .stat-label { font-size: 0.85rem; color: #a0aec0; font-weight: 500; }
    .stat-value { font-size: 1.25rem; font-weight: 700; color: #2d3748; }

    /* 分隔線 */
    .divider-subtle {
        border-color: #e2e8f0;
        opacity: 1;
    }

    /* 規格詳情區塊 (底部大白卡) */
    .spec-section-card {
        background: #fff;
        border-radius: 20px;
        padding: 2.5rem;
        box-shadow: 0 10px 30px rgba(0,0,0,0.03);
    }

    .modern-table th {
        background-color: #f7fafc;
        color: #718096;
        font-weight: 600;
        text-transform: uppercase;
        letter-spacing: 0.5px;
        font-size: 0.85rem;
        padding: 1rem 1.5rem;
        border-bottom: none;
    }
    .modern-table th:first-child { border-top-left-radius: 10px; border-bottom-left-radius: 10px; }
    .modern-table th:last-child { border-top-right-radius: 10px; border-bottom-right-radius: 10px; }
    
    .modern-table td {
        padding: 1.2rem 1.5rem;
        border-bottom: 1px solid #edf2f7;
    }
    .modern-table tbody tr:last-child td { border-bottom: none; }
    
    .sku-badge {
        background-color: #edf2f7;
        color: #4a5568;
        padding: 0.3rem 0.6rem;
        border-radius: 6px;
        font-family: 'Monaco', monospace;
        font-size: 0.85rem;
    }

    .empty-state-icon i {
        font-size: 4rem;
        color: #cbd5e0;
    }
</style>

<style>
    .carousel-control-prev-icon,
    .carousel-control-next-icon {
        background-color: rgba(0, 0, 0, 0.3);
        border-radius: 50%;
        padding: 1.5rem;
    }
</style>
``n
### 檔案: .\ISpanShop.MVC\Views\Products\Index.cshtml
`${ext}
@{
    Layout = "_Layout";
}
@model ISpanShop.Models.DTOs.PagedResult<ISpanShop.MVC.Models.ViewModels.ProductListVm>
@using System.Text.Json

@{
    ViewData["Title"] = "商品列表";
    var parentCategories = ViewBag.ParentCategories as IEnumerable<ISpanShop.Models.DTOs.CategoryDto> ?? Enumerable.Empty<ISpanShop.Models.DTOs.CategoryDto>();
    var allSubCategories = ViewBag.AllSubCategories as IEnumerable<ISpanShop.Models.DTOs.CategoryDto> ?? Enumerable.Empty<ISpanShop.Models.DTOs.CategoryDto>();
    var stores = ViewBag.Stores as IEnumerable<(int Id, string Name)> ?? Enumerable.Empty<(int, string)>();
    var brands = ViewBag.Brands as IEnumerable<(int Id, string Name)> ?? Enumerable.Empty<(int, string)>();
    int? currentParentId = ViewBag.CurrentParentCategoryId;
    int? currentCategoryId = ViewBag.CurrentCategoryId;
    string? currentKeyword = ViewBag.CurrentKeyword;
    int? currentStoreId = ViewBag.CurrentStoreId;
    int? currentBrandId = ViewBag.CurrentBrandId;
    int? currentStatus = ViewBag.CurrentStatus;
    string? currentStartDate = ViewBag.CurrentStartDate;
    string? currentEndDate = ViewBag.CurrentEndDate;
}

<div class="container-fluid py-4 font-noto">
    <div class="d-flex justify-content-between align-items-end mb-4">
        <div>
            <h2 class="fw-bold tracking-tight mb-1" style="color:#1e2d3d;">全站商品總覽</h2>
            <p class="mb-0 fs-7" style="color:#4a5a6a;">
                共計 <span class="fw-bold text-primary">@Model.TotalCount</span> 個商品，
                第 <span class="fw-bold text-primary">@Model.CurrentPage</span> / @Model.TotalPages 頁
            </p>
        </div>
        <div class="d-flex align-items-center gap-2">
            <div id="selection-toolbar" class="d-none d-flex align-items-center gap-2 me-1 px-3 py-2 rounded-pill border border-warning-subtle bg-warning-subtle text-warning-emphasis fs-7 fw-bold">
                <i class="bx bx-check-square me-1"></i>
                已選取 <span id="selectionCount">0</span> 項
                <button type="button" id="btnClearSelection" class="btn-close btn-close-sm ms-2" style="font-size:0.6rem;" title="清除選取"></button>
            </div>
            <button type="button" id="btnBatchPublish" class="btn rounded-pill px-4 shadow-sm" disabled
                    style="background:linear-gradient(135deg,#1abb9c,#15967d);color:#fff;border:none;font-weight:600;transition:all 0.2s;opacity:0.5;">
                <i class="bx bx-cloud-upload me-1"></i> 批次上架
            </button>
            <button type="button" id="btnBatchUnpublish" class="btn rounded-pill px-4 shadow-sm" disabled
                    style="background:linear-gradient(135deg,#8a9bb0,#6b7c93);color:#fff;border:none;font-weight:600;transition:all 0.2s;opacity:0.5;">
                <i class="bx bx-cloud-download me-1"></i> 批次下架
            </button>
            <a href="@Url.Action("Create")" class="btn btn-primary rounded-pill px-4 shadow-sm">
                <i class="bx bx-plus me-1"></i> 新增商品
            </a>
        </div>
    </div>

    <div class="filter-card mb-5">
        <form method="get" asp-action="Index" class="row g-3 align-items-end">
            <div class="col-md-3">
                <label class="form-label">關鍵字搜尋</label>
                <div class="input-group">
                    <span class="input-group-text"><i class="bx bx-search"></i></span>
                    <input type="text" name="keyword" value="@currentKeyword" class="form-control border-start-0 ps-0" placeholder="輸入商品名稱...">
                </div>
            </div>
            <div class="col-md-2">
                <label class="form-label">主分類</label>
                <select id="parentCategorySelect" name="parentCategoryId" class="form-select">
                    <option value="">-- 全部 --</option>
                    @foreach (var pc in parentCategories)
                    {
                        <option value="@pc.Id" selected="@(currentParentId == pc.Id)">@pc.Name</option>
                    }
                </select>
            </div>
            <div class="col-md-2">
                <label class="form-label">子分類</label>
                <select id="categorySelect" name="categoryId" class="form-select">
                    <option value="">-- 全部 --</option>
                    @foreach (var sc in allSubCategories)
                    {
                        <option value="@sc.Id" selected="@(currentCategoryId == sc.Id)">@sc.Name</option>
                    }
                </select>
            </div>
            <div class="col-md-2">
                <label class="form-label">品牌</label>
                <select id="brandSelect" name="brandId" class="form-select">
                    <option value="">-- 全部 --</option>
                    @foreach (var b in brands)
                    {
                        <option value="@b.Id" selected="@(currentBrandId == b.Id)">@b.Name</option>
                    }
                </select>
            </div>
            <div class="col-md-2">
                <label class="form-label">狀態</label>
                <select name="status" class="form-select">
                    <option value="">-- 全部 --</option>
                    <option value="1" selected="@(currentStatus == 1)">已上架</option>
                    <option value="0" selected="@(currentStatus == 0)">未上架</option>
                </select>
            </div>
            <div class="col-md-5 mt-2">
                <label class="form-label">建檔日期區間</label>
                <div class="input-group">
                    <span class="input-group-text">從</span>
                    <input type="date" name="startDate" value="@currentStartDate" class="form-control">
                    <span class="input-group-text">至</span>
                    <input type="date" name="endDate" value="@currentEndDate" class="form-control">
                </div>
            </div>
            <div class="col-md-7 mt-2 d-flex justify-content-end gap-2 align-items-end">
                <button type="submit" class="btn btn-primary rounded-pill px-4">
                    <i class="bx bx-filter-alt me-1"></i> 套用篩選
                </button>
                <a href="@Url.Action("Index")" class="btn btn-outline-light rounded-pill px-4">清除條件</a>
            </div>
        </form>
    </div>

    <div class="table-responsive px-2 pb-3">
        <table class="table table-custom align-middle">
            <thead>
                <tr>
                    <th style="width:48px;"><input type="checkbox" id="selectAll" class="form-check-input" /></th>
                    <th style="width:60px;">圖片</th>
                    <th style="min-width:250px;">商品名稱</th>
                    <th>分類</th>
                    <th>品牌</th>
                    <th>價格</th>
                    <th>建檔日期</th>
                    <th>狀態</th>
                    <th class="text-end">操作</th>
                </tr>
            </thead>
            <tbody>
                @if (Model.Data != null && Model.Data.Any())
                {
                    foreach (var item in Model.Data)
                    {
                        var isPublished = item.Status == 1;
                        var statusText = isPublished ? "已上架" : "未上架";
                        var statusBg = isPublished ? "bg-success-soft text-success" : "bg-secondary-soft text-secondary";
                        <tr>
                            <td><input type="checkbox" class="product-select form-check-input" value="@item.Id" /></td>
                            <td><img src="@item.MainImageUrl" alt="商品圖" class="list-thumbnail" onerror="this.src='https://via.placeholder.com/50'" /></td>
                            <td>
                                <div class="fw-bold fs-6 mb-1" style="color:#1e2d3d;">@item.Name</div>
                                <div class="fs-7" style="color:#5a6a7a;"><i class="bx bx-store me-1"></i>@item.StoreName</div>
                            </td>
                            <td><span class="badge-soft bg-primary-soft text-primary">@item.CategoryName</span></td>
                            <td><span class="badge-soft bg-info-soft text-info">@item.BrandName</span></td>
                            <td class="fw-bold" style="color:#1e2d3d;">
                                @if (item.MinPrice == item.MaxPrice)
                                {
                                    <span>NT$@item.MinPrice.GetValueOrDefault().ToString("N0")</span>
                                }
                                else
                                {
                                    <span>NT$@item.MinPrice.GetValueOrDefault().ToString("N0") ~ @item.MaxPrice.GetValueOrDefault().ToString("N0")</span>
                                }
                            </td>
                            <td class="fs-7" style="color:#4a5a6a;">@item.CreatedAt.GetValueOrDefault().ToString("yyyy-MM-dd")</td>
                            <td><span class="badge rounded-pill px-3 py-2 @statusBg fw-normal">@statusText</span></td>
                            <td class="text-end">
                                <a href="@Url.Action("Details", new { id = item.Id })" class="btn btn-sm btn-action rounded-pill px-3">查看詳情</a>
                            </td>
                        </tr>
                    }
                }
                else
                {
                    <tr>
                        <td colspan="9" class="text-center py-5">
                            <div class="mb-2" style="color:#a0aec0;font-size:2.5rem;"><i class="bx bx-inbox"></i></div>
                            <h5 class="fw-normal" style="color:#718096;">找不到符合條件的商品</h5>
                        </td>
                    </tr>
                }
            </tbody>
        </table>
    </div>

    @if (Model.TotalPages > 1)
    {
        <nav class="mt-5 mb-4">
            <ul class="pagination justify-content-center gap-2">
                <li class="page-item @(Model.CurrentPage == 1 ? "disabled" : "")">
                    <a class="page-link shadow-sm rounded-pill px-3" href="@Url.Action("Index", new { page = Model.CurrentPage - 1, keyword = currentKeyword, parentCategoryId = currentParentId, categoryId = currentCategoryId, brandId = currentBrandId, status = currentStatus, startDate = currentStartDate, endDate = currentEndDate })">
                        <i class="bx bx-chevron-left me-1"></i> 上一頁
                    </a>
                </li>
                @for (int i = 1; i <= Model.TotalPages; i++)
                {
                    if (i == 1 || i == Model.TotalPages || (i >= Model.CurrentPage - 2 && i <= Model.CurrentPage + 2))
                    {
                        <li class="page-item @(i == Model.CurrentPage ? "active" : "")">
                            <a class="page-link shadow-sm rounded-circle fw-bold" style="width:40px;height:40px;display:flex;align-items:center;justify-content:center;"
                               href="@Url.Action("Index", new { page = i, keyword = currentKeyword, parentCategoryId = currentParentId, categoryId = currentCategoryId, brandId = currentBrandId, status = currentStatus, startDate = currentStartDate, endDate = currentEndDate })">@i</a>
                        </li>
                    }
                    else if (i == Model.CurrentPage - 3 || i == Model.CurrentPage + 3)
                    {
                        <li class="page-item disabled"><span class="page-link border-0 bg-transparent fw-bold" style="color:#a0aec0;">...</span></li>
                    }
                }
                <li class="page-item @(Model.CurrentPage == Model.TotalPages ? "disabled" : "")">
                    <a class="page-link shadow-sm rounded-pill px-3" href="@Url.Action("Index", new { page = Model.CurrentPage + 1, keyword = currentKeyword, parentCategoryId = currentParentId, categoryId = currentCategoryId, brandId = currentBrandId, status = currentStatus, startDate = currentStartDate, endDate = currentEndDate })">
                        下一頁 <i class="bx bx-chevron-right ms-1"></i>
                    </a>
                </li>
            </ul>
        </nav>
    }
</div>

<style>
    @@import url('https://fonts.googleapis.com/css2?family=Noto+Sans+TC:wght@300;400;500;700;900&display=swap');

    body {
        background-color: #f4f6f8;
    }

    .font-noto {
        font-family: 'Noto Sans TC', sans-serif;
    }

    .tracking-tight {
        letter-spacing: -0.5px;
    }

    .fs-7 {
        font-size: 0.92rem;
    }

    /* 白色篩選卡片 */
    .filter-card {
        background: #ffffff;
        border-radius: 16px;
        padding: 1.75rem 2rem;
        box-shadow: 0 4px 20px rgba(0,0,0,0.05);
        border: 1px solid #e8edf2;
    }

        .filter-card .form-label {
            color: #5a6a7a !important;
            font-size: 0.78rem !important;
            font-weight: 700;
            letter-spacing: 1px;
            text-transform: uppercase;
            margin-bottom: 0.4rem;
        }

        .filter-card .form-control,
        .filter-card .form-select,
        .filter-card .input-group-text {
            border-color: #dde3ea !important;
            color: #2d3748 !important;
            box-shadow: none;
            transition: all 0.2s;
        }

            .filter-card .form-control:focus,
            .filter-card .form-select:focus {
                border-color: #1abb9c !important;
                box-shadow: 0 0 0 3px rgba(26,187,156,0.15) !important;
            }

        .filter-card .input-group-text {
            color: #8a9bb0 !important;
            background: #f8fafc !important;
        }

        .filter-card .btn-outline-light {
            color: #718096 !important;
            border-color: #dde3ea !important;
        }

            .filter-card .btn-outline-light:hover {
                background: #f4f6f8 !important;
                color: #2d3748 !important;
            }

    /* 表格 */
    .table-custom {
        border-collapse: separate;
        border-spacing: 0 12px;
    }

        .table-custom th {
            border: none;
            color: #3d5166;
            font-weight: 700;
            letter-spacing: 0.8px;
            font-size: 0.88rem;
            padding: 0.5rem 1.5rem;
            background: transparent;
            text-transform: uppercase;
        }

        .table-custom td {
            background: #ffffff;
            border: none;
            padding: 1.1rem 1.5rem;
            font-size: 0.97rem;
            color: #2d3748;
        }

            .table-custom td:first-child {
                border-top-left-radius: 14px;
                border-bottom-left-radius: 14px;
            }

            .table-custom td:last-child {
                border-top-right-radius: 14px;
                border-bottom-right-radius: 14px;
            }

        .table-custom tbody tr {
            box-shadow: 0 3px 12px rgba(0,0,0,0.04);
            transition: all 0.25s ease;
        }

            .table-custom tbody tr:hover {
                transform: translateY(-3px);
                box-shadow: 0 10px 24px rgba(0,0,0,0.08);
            }

    .list-thumbnail {
        width: 50px;
        height: 50px;
        object-fit: cover;
        border-radius: 10px;
        box-shadow: 0 2px 8px rgba(0,0,0,0.07);
    }

    .badge-soft {
        padding: 0.4em 0.85em;
        border-radius: 6px;
        font-size: 0.85rem;
        font-weight: 600;
    }

    .bg-primary-soft {
        background-color: #ebf8ff;
    }

    .text-primary {
        color: #3182ce !important;
    }

    .bg-info-soft {
        background-color: #e6fffa;
    }

    .text-info {
        color: #319795 !important;
    }

    .bg-success-soft {
        background-color: #c6f6d5;
    }

    .text-success {
        color: #276749 !important;
    }

    .bg-secondary-soft {
        background-color: #e2e8f0;
    }

    .text-secondary {
        color: #4a5568 !important;
    }

    .btn-action {
        font-weight: 600;
        transition: all 0.2s;
        background-color: #3182ce;
        color: #ffffff;
        border: 1px solid #3182ce;
    }

        .btn-action:hover {
            background-color: #2563aa !important;
            color: #ffffff !important;
            border-color: #2563aa !important;
            box-shadow: 0 4px 12px rgba(49,130,206,0.4);
        }
</style>

@section Scripts {
    <script>
        (function () {
            var parentSel   = document.getElementById('parentCategorySelect');
            var categorySel = document.getElementById('categorySelect');
            var brandSel    = document.getElementById('brandSelect');
            var urlGetSubs   = '@Url.Action("GetSubCategories", "Products")';
            var urlGetBrands = '@Url.Action("GetBrandsByCategory", "Products")';
            var savedCategoryId = @(currentCategoryId.HasValue? currentCategoryId.Value.ToString() : "null");
            var savedBrandId    = @(currentBrandId.HasValue? currentBrandId.Value.ToString()    : "null");

            function rebuildSelect(sel, items, selectedId) {
                var ph = (sel.querySelector('option[value=""]') || {}).textContent || '-- 全部 --';
                sel.innerHTML = '<option value="">' + ph + '</option>';
                (items || []).forEach(function(item) {
                    var opt = document.createElement('option');
                    opt.value = item.id; opt.textContent = item.name;
                    if (selectedId != null && item.id == selectedId) opt.selected = true;
                    sel.appendChild(opt);
                });
            }

            parentSel.addEventListener('change', function() {
                var pid = this.value;
                rebuildSelect(categorySel, [], null); rebuildSelect(brandSel, [], null);
                if (!pid) return;
                fetch(urlGetSubs + '?parentId=' + pid).then(r => r.json()).then(subs => rebuildSelect(categorySel, subs, null));
            });

            categorySel.addEventListener('change', function() {
                var cid = this.value; rebuildSelect(brandSel, [], null);
                if (!cid) return;
                fetch(urlGetBrands + '?categoryId=' + cid).then(r => r.json()).then(brands => rebuildSelect(brandSel, brands, null));
            });

            if (savedCategoryId) {
                fetch(urlGetBrands + '?categoryId=' + savedCategoryId).then(r => r.json()).then(brands => rebuildSelect(brandSel, brands, savedBrandId));
            }

            var SESSION_KEY = 'selectedProductIds';
            var selectAll = document.getElementById('selectAll');
            var btnPublish = document.getElementById('btnBatchPublish');
            var btnUnpublish = document.getElementById('btnBatchUnpublish');
            var toolbar = document.getElementById('selection-toolbar');
            var countEl = document.getElementById('selectionCount');
            var btnClear = document.getElementById('btnClearSelection');
            var urlBatchUpdate = '@Url.Action("BatchUpdateStatus", "Products")';

            function loadSelected() { try { return JSON.parse(sessionStorage.getItem(SESSION_KEY)) || []; } catch(e) { return []; } }
            function saveSelected(ids) { sessionStorage.setItem(SESSION_KEY, JSON.stringify(ids)); }
            function updateState() {
                var ids = loadSelected(); var count = ids.length;
                if (count > 0) { toolbar.classList.remove('d-none'); toolbar.classList.add('d-flex'); countEl.textContent = count; }
                else           { toolbar.classList.add('d-none'); toolbar.classList.remove('d-flex'); countEl.textContent = 0; }
                btnPublish.disabled = btnUnpublish.disabled = (count === 0);
                btnPublish.style.opacity   = count > 0 ? '1' : '0.5';
                btnUnpublish.style.opacity = count > 0 ? '1' : '0.5';
            }

            (function() {
                var ids = loadSelected(); var total = 0; var checked = 0;
                document.querySelectorAll('.product-select').forEach(function(cb) {
                    total++;
                    if (ids.indexOf(parseInt(cb.value)) !== -1) { cb.checked = true; checked++; }
                });
                if (total > 0) { selectAll.checked = (checked === total); selectAll.indeterminate = (checked > 0 && checked < total); }
                updateState();
            })();

            selectAll.addEventListener('change', function() {
                var ids = loadSelected();
                document.querySelectorAll('.product-select').forEach(function(cb) {
                    var id = parseInt(cb.value); cb.checked = selectAll.checked;
                    if (selectAll.checked) { if (ids.indexOf(id) === -1) ids.push(id); }
                    else { ids = ids.filter(x => x !== id); }
                });
                saveSelected(ids); updateState();
            });

            document.querySelectorAll('.product-select').forEach(function(cb) {
                cb.addEventListener('change', function() {
                    var id = parseInt(this.value); var ids = loadSelected();
                    if (this.checked) { if (ids.indexOf(id) === -1) ids.push(id); }
                    else { ids = ids.filter(x => x !== id); }
                    saveSelected(ids);
                    var total = document.querySelectorAll('.product-select').length;
                    var checked = document.querySelectorAll('.product-select:checked').length;
                    selectAll.checked = (checked === total); selectAll.indeterminate = (checked > 0 && checked < total);
                    updateState();
                });
            });

            function batchUpdate(targetStatus) {
                var ids = loadSelected(); var action = targetStatus === 1 ? '上架' : '下架';
                Swal.fire({
                    title: '批次' + action + '確認',
                    html: '確定要將這 <strong>' + ids.length + '</strong> 項商品設為「<strong>' + action + '</strong>」嗎？',
                    icon: 'question', showCancelButton: true,
                    confirmButtonColor: targetStatus === 1 ? '#38a169' : '#718096',
                    cancelButtonColor: '#a0aec0', confirmButtonText: '確認' + action, cancelButtonText: '取消'
                }).then(function(result) {
                    if (!result.isConfirmed) return;
                    fetch(urlBatchUpdate, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ productIds: ids, targetStatus: targetStatus }) })
                    .then(r => r.json()).then(function(res) {
                        if (res.success) {
                            sessionStorage.removeItem(SESSION_KEY);
                            Swal.fire({ title: '操作成功！', text: res.message, icon: 'success', confirmButtonColor: '#3182ce', timer: 2000, timerProgressBar: true }).then(() => location.reload());
                        } else {
                            Swal.fire({ title: '操作失敗', text: res.message, icon: 'error', confirmButtonColor: '#e53e3e' });
                        }
                    }).catch(() => Swal.fire({ title: '連線錯誤', text: '請稍後再試。', icon: 'error', confirmButtonColor: '#e53e3e' }));
                });
            }

            btnClear.addEventListener('click', function() {
                sessionStorage.removeItem(SESSION_KEY);
                document.querySelectorAll('.product-select').forEach(cb => cb.checked = false);
                selectAll.checked = false; selectAll.indeterminate = false; updateState();
            });

            btnPublish.addEventListener('click',   () => batchUpdate(1));
            btnUnpublish.addEventListener('click', () => batchUpdate(0));
        })();
    </script>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
}

``n
### 檔案: .\ISpanShop.MVC\Views\Products\PendingReview.cshtml
`${ext}
@{
    Layout = "_Layout";
}
@model IEnumerable<ISpanShop.MVC.Models.ViewModels.ProductReviewListVm>

@{
    ViewData["Title"] = "商品審核中心";
    var rejectedList = ViewBag.RejectedRecords as IEnumerable<ISpanShop.MVC.Models.ViewModels.ProductReviewListVm>;
}

<div class="container-fluid py-4 font-noto">

    <div class="d-flex justify-content-between align-items-end mb-4">
        <div>
            <h2 class="fw-bold tracking-tight mb-1" style="color:#1e2d3d;">商品審核中心</h2>
            <p class="mb-0 fs-7" style="color:#4a5a6a;">
                待審核商品共 <span class="fw-bold text-warning" id="pendingCount">@(Model?.Count() ?? 0)</span> 筆
            </p>
        </div>
        <a href="@Url.Action("Index")" class="btn btn-back rounded-pill px-4">
            <i class="bx bx-arrow-back me-1"></i> 回商品總覽
        </a>
    </div>

    @* Bootstrap Nav Tabs *@
    <ul class="nav nav-tabs mb-0 border-0" id="reviewTabs" role="tablist">
        <li class="nav-item" role="presentation">
            <button class="nav-link active px-4 py-2 fw-semibold" id="pending-tab"
                    data-bs-toggle="tab" data-bs-target="#pendingPanel"
                    type="button" role="tab">
                <i class="bx bx-hourglass me-1 text-warning"></i>
                待審核
                <span class="badge rounded-pill ms-1"
                      style="background:#fef3c7;color:#92400e;font-size:0.78rem;">@(Model?.Count() ?? 0)</span>
            </button>
        </li>
        <li class="nav-item" role="presentation">
            <button class="nav-link px-4 py-2 fw-semibold" id="rejected-tab"
                    data-bs-toggle="tab" data-bs-target="#rejectedPanel"
                    type="button" role="tab">
                <i class="bx bx-x-circle me-1 text-danger"></i>
                近期退回紀錄
                <span class="badge rounded-pill ms-1"
                      style="background:#fee2e2;color:#991b1b;font-size:0.78rem;">@(rejectedList?.Count() ?? 0)</span>
            </button>
        </li>
    </ul>

    <div class="tab-content" id="reviewTabsContent">

        <div class="tab-pane fade show active" id="pendingPanel" role="tabpanel">
            <div class="filter-card rounded-top-0" style="border-top-left-radius:0!important;border-top-right-radius:0!important;">
                @if (Model != null && Model.Any())
                {
                    <div class="table-responsive">
                        <table class="table table-custom align-middle">
                            <thead>
                                <tr>
                                    <th>商品名稱</th>
                                    <th>商店</th>
                                    <th>分類 / 品牌</th>
                                    <th style="min-width:260px;">商品描述摘要</th>
                                    <th>建檔日期</th>
                                    <th class="text-end" style="min-width:200px;">審核操作</th>
                                </tr>
                            </thead>
                            <tbody>
                                @foreach (var product in Model)
                                {
                                    <tr data-id="@product.Id">
                                        <td>
                                            <div class="fw-bold" style="color:#1e2d3d;">@product.Name</div>
                                        </td>
                                        <td class="fs-7" style="color:#5a6a7a;">@(product.StoreName ?? $"Store #{product.StoreId}")</td>
                                        <td>
                                            <span class="badge-soft bg-primary-soft text-primary me-1">@product.CategoryName</span>
                                            <span class="badge-soft bg-info-soft text-info">@product.BrandName</span>
                                        </td>
                                        <td>
                                            <span class="fs-7" style="color:#4a5a6a;">
                                                @if (!string.IsNullOrWhiteSpace(product.Description))
                                                {
                                                    @(product.Description.Length > 60
                                                                                            ? product.Description.Substring(0, 60) + "…"
                                                                                            : product.Description)
                                                                                }
                                                else
                                                {
                                                    <span class="text-danger fw-bold">（無描述）</span>
                                                }
                                            </span>
                                        </td>
                                        <td class="fs-7" style="color:#4a5a6a;">@(product.CreatedAt?.ToString("yyyy-MM-dd HH:mm") ?? "-")</td>
                                        <td class="text-end">
                                            <button class="btn btn-sm btn-approve rounded-pill px-3 me-1"
                                                    data-id="@product.Id" data-name="@product.Name">
                                                <i class="bx bx-check-circle me-1"></i>核准上架
                                            </button>
                                            <button class="btn btn-sm btn-reject rounded-pill px-3"
                                                    data-id="@product.Id" data-name="@product.Name">
                                                <i class="bx bx-x-circle me-1"></i>退回
                                            </button>
                                        </td>
                                    </tr>
                                }
                            </tbody>
                        </table>
                    </div>
                }
                else
                {
                    <div class="text-center py-5">
                        <div class="mb-3" style="color:#1abb9c;font-size:2.5rem;"><i class="bx bx-check-circle"></i></div>
                        <h5 class="fw-bold mb-1" style="color:#1e2d3d;">目前沒有待審核的商品</h5>
                        <p class="fs-7 mb-0" style="color:#718096;">所有商品均已完成審核，稍後再來查看。</p>
                    </div>
                }
            </div>
        </div>

        <div class="tab-pane fade" id="rejectedPanel" role="tabpanel">
            <div class="filter-card rounded-top-0" style="border-top-left-radius:0!important;border-top-right-radius:0!important;">
                @if (rejectedList != null && rejectedList.Any())
                {
                    <div class="table-responsive">
                        <table class="table table-custom align-middle">
                            <thead>
                                <tr>
                                    <th style="width:70px;">圖片</th>
                                    <th>商品名稱</th>
                                    <th>商家</th>
                                    <th>退回原因</th>
                                    <th>退回時間</th>
                                </tr>
                            </thead>
                            <tbody>
                                @foreach (var item in rejectedList)
                                {
                                    <tr>
                                        <td>
                                            @if (!string.IsNullOrEmpty(item.MainImageUrl))
                                            {
                                                <img src="@item.MainImageUrl" alt="@item.Name"
                                                     style="width:54px;height:54px;object-fit:cover;border-radius:10px;box-shadow:0 2px 8px rgba(0,0,0,0.08);" />
                                            }
                                            else
                                            {
                                                <div style="width:54px;height:54px;border-radius:10px;background:#f1f5f9;display:flex;align-items:center;justify-content:center;font-size:1.3rem;">🖼️</div>
                                            }
                                        </td>
                                        <td class="fw-semibold" style="color:#1e2d3d;">@item.Name</td>
                                        <td class="fs-7" style="color:#5a6a7a;">@item.StoreName</td>
                                        <td>
                                            @if (!string.IsNullOrWhiteSpace(item.RejectReason))
                                            {
                                                <span style="background:#fff0f0;color:#c53030;border-radius:6px;font-size:0.82rem;font-weight:500;padding:0.3em 0.7em;display:inline-block;">
                                                    @item.RejectReason
                                                </span>
                                            }
                                            else
                                            {
                                                <span class="fs-7" style="color:#a0aec0;">（未填寫）</span>
                                            }
                                        </td>
                                        <td class="fs-7" style="color:#4a5a6a;">@(item.UpdatedAt?.ToString("yyyy-MM-dd HH:mm") ?? "—")</td>
                                    </tr>
                                }
                            </tbody>
                        </table>
                    </div>
                }
                else
                {
                    <div class="text-center py-5">
                        <div class="mb-2" style="font-size:2rem;">📭</div>
                        <p class="mb-0" style="color:#a0aec0;">近期無退回紀錄</p>
                    </div>
                }
            </div>
        </div>

    </div>

</div>

<style>
    @@import url('https://fonts.googleapis.com/css2?family=Noto+Sans+TC:wght@300;400;500;700&display=swap');

    body {
        background-color: #f4f6f8;
    }

    .font-noto {
        font-family: 'Noto Sans TC', sans-serif;
    }

    .tracking-tight {
        letter-spacing: -0.5px;
    }

    .fs-7 {
        font-size: 0.9rem;
    }

    /* 回商品總覽按鈕 - 明顯青綠色 */
    .btn-back {
        background-color: #1abb9c;
        color: #ffffff;
        border: none;
        font-weight: 600;
        transition: all 0.2s;
        box-shadow: 0 3px 10px rgba(26,187,156,0.3);
    }

        .btn-back:hover {
            background-color: #0e8a72;
            color: #ffffff;
            box-shadow: 0 5px 16px rgba(14,138,114,0.4);
            transform: translateY(-1px);
        }

    /* Tabs */
    #reviewTabs .nav-link {
        color: #718096;
        border: 1px solid transparent;
        border-bottom: none;
        border-radius: 12px 12px 0 0;
        background: rgba(255,255,255,0.5);
        transition: all 0.2s;
    }

        #reviewTabs .nav-link:hover {
            color: #2d3748;
            background: rgba(255,255,255,0.8);
        }

        #reviewTabs .nav-link.active {
            color: #2d3748;
            background: #fff;
            border-color: rgba(226,232,240,0.8);
            border-bottom-color: #fff;
        }

    /* 卡片 */
    .filter-card {
        background: #ffffff;
        border-radius: 0 20px 20px 20px;
        padding: 2rem;
        box-shadow: 0 4px 20px rgba(0,0,0,0.04);
        border: 1px solid rgba(226,232,240,0.8);
    }

    /* 表格 */
    .table-custom {
        border-collapse: separate;
        border-spacing: 0 10px;
    }

        .table-custom th {
            border: none;
            color: #3d5166;
            font-weight: 700;
            letter-spacing: 1px;
            font-size: 0.85rem;
            padding: 0.4rem 1.2rem;
            background: transparent;
            text-transform: uppercase;
        }

        .table-custom td {
            background: #fff;
            border: none;
            padding: 1rem 1.2rem;
            font-size: 0.95rem;
        }

            .table-custom td:first-child {
                border-top-left-radius: 12px;
                border-bottom-left-radius: 12px;
            }

            .table-custom td:last-child {
                border-top-right-radius: 12px;
                border-bottom-right-radius: 12px;
            }

        .table-custom tbody tr {
            box-shadow: 0 3px 12px rgba(0,0,0,0.03);
            transition: all 0.25s ease;
        }

            .table-custom tbody tr:hover {
                transform: translateY(-2px);
                box-shadow: 0 8px 22px rgba(0,0,0,0.07);
            }

    .badge-soft {
        padding: 0.35em 0.75em;
        border-radius: 6px;
        font-size: 0.8rem;
        font-weight: 600;
    }

    .bg-primary-soft {
        background-color: #ebf8ff;
    }

    .text-primary {
        color: #3182ce !important;
    }

    .bg-info-soft {
        background-color: #e6fffa;
    }

    .text-info {
        color: #319795 !important;
    }

    /* 審核按鈕 */
    .btn-approve {
        background-color: #c6f6d5;
        color: #276749;
        border: none;
        font-weight: 600;
        transition: all 0.2s;
    }

        .btn-approve:hover {
            background-color: #1abb9c;
            color: #fff;
            box-shadow: 0 4px 12px rgba(26,187,156,0.35);
        }

    .btn-reject {
        background-color: #fed7d7;
        color: #9b2c2c;
        border: none;
        font-weight: 600;
        transition: all 0.2s;
    }

        .btn-reject:hover {
            background-color: #e53e3e;
            color: #fff;
            box-shadow: 0 4px 12px rgba(229,62,62,0.3);
        }
</style>

@section Scripts {
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script>
        (function () {
            var urlApprove = '@Url.Action("ApproveProduct", "Products")';
            var urlReject  = '@Url.Action("RejectProduct", "Products")';

            document.addEventListener('DOMContentLoaded', function () {
                if (window.location.hash === '#rejectedTab') {
                    var tabEl = document.querySelector('#rejected-tab');
                    if (tabEl && typeof bootstrap !== 'undefined') {
                        new bootstrap.Tab(tabEl).show();
                    }
                    history.replaceState(null, '', window.location.pathname + window.location.search);
                }
            });

            function removeRow(id) {
                var row = document.querySelector('tr[data-id="' + id + '"]');
                if (row) row.remove();
                var remaining = document.querySelectorAll('#pendingPanel tbody tr').length;
                document.querySelectorAll('#pendingCount, .text-warning.fw-bold').forEach(function(el) { el.textContent = remaining; });
                var badge = document.querySelector('#pending-tab .badge');
                if (badge) badge.textContent = remaining;
                if (remaining === 0) location.reload();
            }

            document.querySelectorAll('.btn-approve').forEach(function (btn) {
                btn.addEventListener('click', function () {
                    var id = this.dataset.id; var name = this.dataset.name;
                    Swal.fire({
                        title: '確認核准上架',
                        html: '確定要核准商品<br><strong>「' + name + '」</strong>上架嗎？',
                        icon: 'question', showCancelButton: true,
                        confirmButtonColor: '#1abb9c', cancelButtonColor: '#a0aec0',
                        confirmButtonText: '確認核准', cancelButtonText: '取消'
                    }).then(function (result) {
                        if (!result.isConfirmed) return;
                        fetch(urlApprove, { method: 'POST', headers: { 'Content-Type': 'application/x-www-form-urlencoded' }, body: 'id=' + id })
                        .then(r => r.json()).then(function(res) {
                            if (res.success) {
                                removeRow(id);
                                Swal.fire({ title: '核准成功！', text: res.message, icon: 'success', confirmButtonColor: '#1abb9c', timer: 2000, timerProgressBar: true });
                            } else {
                                Swal.fire({ title: '操作失敗', text: res.message, icon: 'error', confirmButtonColor: '#e53e3e' });
                            }
                        }).catch(() => Swal.fire({ title: '連線錯誤', text: '請稍後再試。', icon: 'error' }));
                    });
                });
            });

            document.querySelectorAll('.btn-reject').forEach(function (btn) {
                btn.addEventListener('click', function () {
                    var id = this.dataset.id; var name = this.dataset.name;
                    Swal.fire({
                        title: '退回商品審核',
                        html: '商品：<strong>「' + name + '」</strong>',
                        input: 'textarea', inputLabel: '退回原因（必填）',
                        inputPlaceholder: '請填寫退回原因...',
                        inputAttributes: { rows: 3 },
                        icon: 'warning', showCancelButton: true,
                        confirmButtonColor: '#e53e3e', cancelButtonColor: '#a0aec0',
                        confirmButtonText: '確認退回', cancelButtonText: '取消',
                        inputValidator: function(value) { if (!value || !value.trim()) return '退回原因為必填欄位'; }
                    }).then(function (result) {
                        if (!result.isConfirmed) return;
                        fetch(urlReject + '?id=' + id + '&reason=' + encodeURIComponent(result.value.trim()), { method: 'POST' })
                        .then(r => r.json()).then(function(res) {
                            if (res.success) {
                                removeRow(id);
                                Swal.fire({ title: '已退回！', text: res.message, icon: 'success', confirmButtonColor: '#1abb9c', timer: 2000, timerProgressBar: true })
                                .then(() => { window.location.hash = '#rejectedTab'; window.location.reload(); });
                            } else {
                                Swal.fire({ title: '操作失敗', text: res.message, icon: 'error', confirmButtonColor: '#e53e3e' });
                            }
                        }).catch(() => Swal.fire({ title: '連線錯誤', text: '請稍後再試。', icon: 'error' }));
                    });
                });
            });
        })();
    </script>
}

``n
### 檔案: .\ISpanShop.MVC\Views\Shared\_Layout.cshtml
`${ext}
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - ISpanShop 後台管理系統</title>
    <link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.min.css" />
    <link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />
    <link rel="stylesheet" href="~/ISpanShop.MVC.styles.css" asp-append-version="true" />
</head>
<body>
    <!-- Navbar -->
    <header>
        <nav class="navbar navbar-expand-sm navbar-toggleable-sm navbar-dark bg-dark border-bottom box-shadow mb-3">
            <div class="container-fluid">
                <a class="navbar-brand" asp-area="" asp-controller="Home" asp-action="Index">
                    <strong>ISpanShop 後台管理系統</strong>
                </a>
                <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target=".navbar-collapse" aria-controls="navbarSupportedContent"
                        aria-expanded="false" aria-label="Toggle navigation">
                    <span class="navbar-toggler-icon"></span>
                </button>
            </div>
        </nav>
    </header>

    <!-- Main Layout -->
    <div class="container-fluid">
        <div class="row">
            <!-- Sidebar -->
            <aside class="col-md-2 col-lg-2 bg-light p-3 border-end" style="min-height: calc(100vh - 150px);">
                <div class="nav flex-column nav-pills">
                    <h5 class="mb-3">功能選單</h5>
                    <a class="nav-link" asp-controller="Products" asp-action="Index">
                        全站商品總覽
                    </a>
                    <a class="nav-link" asp-controller="Products" asp-action="PendingReview">
                        待審核商品
                    </a>
                </div>
            </aside>

            <!-- Main Content -->
            <main class="col-md-10 col-lg-10 p-4" role="main">
                @RenderBody()
            </main>
        </div>
    </div>

    <!-- Footer -->
    <footer class="border-top footer text-muted mt-5 py-3">
        <div class="container-fluid">
            <div class="row">
                <div class="col-md-12 text-center">
                    &copy; 2026 - ISpanShop 後台管理系統
                </div>
            </div>
        </div>
    </footer>

    <script src="~/lib/jquery/dist/jquery.min.js"></script>
    <script src="~/lib/bootstrap/dist/js/bootstrap.bundle.min.js"></script>
    <script src="~/js/site.js" asp-append-version="true"></script>
    @await RenderSectionAsync("Scripts", required: false)
</body>
</html>

``n
### 檔案: .\ISpanShop.MVC\Views\Shared\_SneatLayout.cshtml
`${ext}
@{
}
<!DOCTYPE html>
<html lang="zh-Hant" dir="ltr" data-theme="theme-default"
      data-assets-path="/sneat/assets/"
      data-template="vertical-menu-template-free">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no, minimum-scale=1.0, maximum-scale=1.0" />
    <title>@ViewData["Title"] - ISpanShop 後台管理系統</title>
    <link rel="icon" type="image/x-icon" href="~/sneat/assets/img/favicon/favicon.ico" />
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin />
    <link href="https://fonts.googleapis.com/css2?family=Public+Sans:ital,wght@0,300;0,400;0,500;0,600;0,700;1,300;1,400;1,500;1,600;1,700&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="~/sneat/assets/vendor/fonts/boxicons.css" />
    <link rel="stylesheet" href="~/sneat/assets/vendor/css/core.css" />
    <link rel="stylesheet" href="~/sneat/assets/vendor/css/theme-default.css" />
    <link rel="stylesheet" href="~/sneat/assets/css/demo.css" />
    <link rel="stylesheet" href="~/sneat/assets/vendor/libs/perfect-scrollbar/perfect-scrollbar.css" />

    <script>
        (function(){
            var html = document.documentElement;
            /* 1. 收合狀態 */
            if(window.innerWidth>=1200 && sessionStorage.getItem('sidebarCollapsed')==='1'){
                html.classList.add('pre-collapsed');
            }
            /* 2. 預先展開子選單，避免頁面切換閃爍 */
            var openKey = sessionStorage.getItem('openMenuKey');
            if(openKey){ html.setAttribute('data-open-menu', openKey); }
            /* 3. tab title */
            var savedTitle = sessionStorage.getItem('pageTitle');
            if(savedTitle){ document.title = savedTitle; }
        })();
    </script>

    <style>
        *, *::before, *::after {
            box-sizing: border-box;
        }

        html, body {
            margin: 0;
            padding: 0;
            height: 100%;
        }

        body {
            background-color: #f4f6f8;
            font-family: 'Public Sans', sans-serif;
        }

        :root {
            --sidebar-w: 260px;
            --sidebar-mini-w: 76px;
            --transition: 0.25s ease;
        }

        #app-layout {
            display: flex;
            min-height: 100vh;
        }

        /* SIDEBAR */
        #layout-menu {
            width: var(--sidebar-w);
            min-height: 100vh;
            background-color: #2a3f54;
            display: flex;
            flex-direction: column;
            flex-shrink: 0;
            transition: width var(--transition);
            overflow: hidden;
            z-index: 1100;
        }

        .app-brand {
            display: flex;
            align-items: center;
            padding: 1rem 0 0.75rem;
            border-bottom: 1px solid rgba(255,255,255,0.08);
            flex-shrink: 0;
            overflow: hidden;
            transition: padding var(--transition);
            /* 展開時讓 logo icon 對齊選單 icon（左邊距 1.25rem，icon 寬 1.2rem，置中在 34px 範圍內） */
            padding-left: 1.75rem;
        }

        .app-brand-text {
            color: #fff;
            font-weight: 700;
            font-size: 1rem;
            margin-left: .6rem;
            white-space: nowrap;
            transition: opacity var(--transition), width var(--transition);
        }

        /* MENU */
        .menu-inner {
            list-style: none;
            padding: .25rem 0;
            margin: 0;
            overflow-y: auto;
            overflow-x: hidden;
            flex: 1;
            display: block !important;
        }

            .menu-inner::-webkit-scrollbar {
                width: 4px;
            }

            .menu-inner::-webkit-scrollbar-thumb {
                background: rgba(255,255,255,0.15);
                border-radius: 4px;
            }

        .menu-header {
            padding: .75rem 1.25rem .25rem;
            font-size: .72rem;
            font-weight: 700;
            letter-spacing: 1px;
            color: #73a0bf;
            text-transform: uppercase;
            white-space: nowrap;
            transition: opacity var(--transition);
            display: block !important;
        }

        .menu-item {
            position: relative;
            display: block !important;
        }

        /* 每個主選單項目加上底部間距，製造分隔感 */
        .menu-inner > .menu-item,
        .menu-inner > .menu-header {
            margin-bottom: 6px;
        }

        .menu-link {
            display: flex !important;
            align-items: center;
            padding: .55rem 1.25rem;
            color: #a9c1d8;
            text-decoration: none;
            transition: background var(--transition), color var(--transition);
            white-space: nowrap;
            cursor: pointer;
            border-radius: 6px;
            margin: 0 .5rem;
        }

            .menu-link:hover {
                color: #fff;
                background: rgba(255,255,255,0.08);
            }

        .menu-item.active > .menu-link {
            color: #fff;
            background: linear-gradient(90deg,#1abb9c,#18a084);
            border-radius: 8px;
        }

        .menu-icon {
            font-size: 1.2rem;
            flex-shrink: 0;
            margin-right: .65rem;
            width: 1.2rem;
            text-align: center;
            line-height: 1;
        }

        .menu-label {
            flex: 1;
            font-size: .925rem;
            white-space: nowrap;
            transition: opacity var(--transition);
            overflow: hidden;
        }

        .menu-arrow {
            font-size: .85rem;
            margin-left: auto;
            transition: transform var(--transition), opacity var(--transition);
            color: #73a0bf;
            flex-shrink: 0;
        }

        .menu-short {
            display: none;
        }

        .mini-wrap {
            display: none !important;
        }
        /* In expanded state, standalone icons show; mini-wrap hidden */

        /* SUBMENU */
        .menu-sub {
            display: grid !important;
            grid-template-rows: 0fr;
            background: rgba(0,0,0,0.12);
            border-radius: 0 0 6px 6px;
            margin: 0 .5rem;
        }

            .menu-sub > ul {
                overflow: hidden;
                list-style: none;
                margin: 0;
                padding: 0;
            }

            .menu-sub.animated {
                transition: grid-template-rows 0.35s cubic-bezier(0.4,0,0.2,1);
            }

        .menu-item.open > .menu-sub {
            grid-template-rows: 1fr;
        }

        .menu-item.open > .menu-link .menu-arrow {
            transform: rotate(90deg);
        }

        .menu-sub > ul > .menu-item > .menu-link {
            padding: .45rem 1.25rem .45rem 3rem;
            font-size: .875rem;
            display: flex !important;
            margin: 0;
            border-radius: 0;
        }

            .menu-sub > ul > .menu-item > .menu-link:hover {
                background: rgba(255,255,255,0.06);
            }

        .menu-sub > ul > .menu-item.active > .menu-link {
            background: rgba(26,187,156,0.18);
            color: #1abb9c;
            border-radius: 0;
            margin: 0;
            padding-left: 3rem;
        }

        .menu-sub > ul > .menu-item:last-child > .menu-link {
            border-radius: 0 0 6px 6px;
        }

        .menu-sub > ul > .menu-item.active:last-child > .menu-link {
            border-radius: 0 0 6px 6px;
        }
        /* submenu items: use opacity animation only when NOT pre-open */
        .menu-sub > ul > .menu-item {
            opacity: 0;
            transition: opacity 0.2s ease;
        }
            /* items already visible on page load - no animation */
            .menu-sub > ul > .menu-item.pre-open {
                opacity: 1 !important;
                transition: none !important;
            }

        .menu-item.open > .menu-sub > ul > .menu-item {
            opacity: 1;
        }

        /* FLYOUT */
        .menu-flyout {
            display: none;
            position: fixed;
            left: var(--sidebar-mini-w);
            min-width: 180px;
            background: #1e2d3d;
            border-radius: 0 8px 8px 0;
            padding: .5rem 0;
            z-index: 99999;
            box-shadow: 4px 4px 16px rgba(0,0,0,0.3);
        }

        .menu-flyout-title {
            padding: .4rem 1rem .3rem;
            font-size: .72rem;
            font-weight: 700;
            letter-spacing: 1px;
            color: #73a0bf;
            text-transform: uppercase;
            border-bottom: 1px solid rgba(255,255,255,0.08);
            margin-bottom: .25rem;
        }

        .menu-flyout a {
            display: block;
            padding: .45rem 1rem;
            color: #a9c1d8;
            text-decoration: none;
            font-size: .875rem;
            white-space: nowrap;
        }

            .menu-flyout a:hover {
                color: #fff;
                background: rgba(255,255,255,0.08);
            }

        /* MAIN */
        #main-wrapper {
            flex: 1;
            min-width: 0;
            display: flex;
            flex-direction: column;
        }

        #layout-navbar {
            height: 56px;
            background: #fff;
            border-bottom: 1px solid #e8edf2;
            box-shadow: 0 2px 10px rgba(0,0,0,0.05);
            display: flex;
            align-items: center;
            padding: 0 1.25rem;
            position: sticky;
            top: 0;
            z-index: 1050;
            flex-shrink: 0;
        }

        #sidebarToggleBtn {
            font-size: 1.35rem;
            color: #697a8d;
            cursor: pointer;
            background: none;
            border: none;
            padding: .3rem .4rem;
            margin-right: .5rem;
            border-radius: 6px;
            line-height: 1;
            display: flex;
            align-items: center;
            flex-shrink: 0;
        }

            #sidebarToggleBtn:hover {
                background: #f0f2f4;
                color: #2d3748;
            }

        .navbar-right {
            margin-left: auto;
            display: flex;
            align-items: center;
        }

        .content-wrapper {
            flex: 1;
            display: flex;
            flex-direction: column;
        }

        .content-body {
            flex: 1;
            padding: 1.5rem;
        }

        .content-footer {
            padding: .75rem 1.5rem;
            font-size: .875rem;
            color: #697a8d;
            border-top: 1px solid #e8edf2;
            background: #fff;
        }

        /* ══ COLLAPSED ══ */
        body.sidebar-collapsed #layout-menu,
        html.pre-collapsed #layout-menu {
            width: var(--sidebar-mini-w);
        }

        body.sidebar-collapsed .app-brand-text,
        html.pre-collapsed .app-brand-text {
            opacity: 0;
            width: 0;
            overflow: hidden;
        }

        body.sidebar-collapsed .menu-header,
        html.pre-collapsed .menu-header {
            display: none !important;
        }

        body.sidebar-collapsed .app-brand,
        html.pre-collapsed .app-brand {
            justify-content: center;
            padding: 6px 4px;
            border-bottom-color: transparent;
        }

        /* menu-link 改成 block 置中容器，mini-wrap 控制 icon+短標籤緊靠 */
        body.sidebar-collapsed .menu-link,
        html.pre-collapsed .menu-link {
            display: block !important;
            text-align: center;
            padding: 6px 2px !important;
            margin: 0 4px !important;
            border-radius: 8px;
            line-height: 1;
        }

        body.sidebar-collapsed .mini-wrap,
        html.pre-collapsed .mini-wrap {
            display: inline-flex !important;
            flex-direction: column;
            align-items: center;
            gap: 1px;
        }

        body.sidebar-collapsed .menu-icon,
        html.pre-collapsed .menu-icon {
            display: block !important;
            margin: 0 !important;
            font-size: 1.15rem;
            line-height: 1;
            width: auto !important;
        }

        body.sidebar-collapsed .menu-short,
        html.pre-collapsed .menu-short {
            display: block !important;
            font-size: 0.58rem;
            line-height: 1;
            margin: 0;
            white-space: nowrap;
        }

        /* hide standalone icons when collapsed - only mini-wrap icons show */
        body.sidebar-collapsed .menu-link > .menu-icon,
        html.pre-collapsed .menu-link > .menu-icon {
            display: none !important;
        }

        body.sidebar-collapsed .menu-label,
        html.pre-collapsed .menu-label {
            display: none !important;
        }

        body.sidebar-collapsed .menu-arrow,
        html.pre-collapsed .menu-arrow {
            display: none !important;
        }

        body.sidebar-collapsed .menu-item.active > .menu-link,
        html.pre-collapsed .menu-item.active > .menu-link {
            border-radius: 8px;
            margin: 0 4px !important;
        }

        /* 收合時選單間距 */
        body.sidebar-collapsed .menu-inner > .menu-item,
        html.pre-collapsed .menu-inner > .menu-item {
            margin-bottom: 16px;
        }

        body.sidebar-collapsed .menu-sub,
        html.pre-collapsed .menu-sub {
            grid-template-rows: 0fr !important;
            transition: none !important;
        }

        /* MOBILE */
        @@media (max-width: 1199px) {
            #layout-menu {
                position: fixed;
                top: 0;
                left: 0;
                bottom: 0;
                transform: translateX(-100%);
                transition: transform var(--transition);
                width: var(--sidebar-w) !important;
            }

            body.mobile-menu-open #layout-menu {
                transform: translateX(0);
            }

            #sidebar-overlay {
                display: none;
                position: fixed;
                inset: 0;
                background: rgba(0,0,0,0.5);
                z-index: 1099;
            }

            body.mobile-menu-open #sidebar-overlay {
                display: block;
            }
        }

        /* GLOBAL */
        .btn-primary {
            background-color: #1abb9c !important;
            border-color: #1abb9c !important;
            color: #fff !important;
        }

            .btn-primary:hover {
                background-color: #18a084 !important;
                border-color: #18a084 !important;
            }

        .page-item.active .page-link {
            background-color: #1abb9c !important;
            border-color: #1abb9c !important;
        }

        .text-primary {
            color: #1abb9c !important;
        }

        a.text-primary:hover {
            color: #18a084 !important;
        }

        .avatar {
            width: 40px;
            height: 40px;
            border-radius: 50%;
            overflow: hidden;
            position: relative;
            display: inline-block;
        }

            .avatar img {
                width: 100%;
                height: 100%;
                object-fit: cover;
            }

        .avatar-online::after {
            content: '';
            position: absolute;
            bottom: 2px;
            right: 2px;
            width: 9px;
            height: 9px;
            background: #71dd37;
            border: 2px solid #fff;
            border-radius: 50%;
        }
    </style>
</head>
<body>
    <div id="sidebar-overlay"></div>
    <div id="app-layout">
        <aside id="layout-menu">
            <div class="app-brand">
                <a href="/" style="display:flex;align-items:center;text-decoration:none;overflow:hidden;">
                    <span style="width:34px;height:34px;border-radius:8px;background:linear-gradient(135deg,#696cff,#9155fd);display:flex;align-items:center;justify-content:center;flex-shrink:0;">
                        <i class="bx bx-store-alt" style="font-size:1.35rem;color:#fff;"></i>
                    </span>
                    <span class="app-brand-text">ISpanShop</span>
                </a>
            </div>
            <ul class="menu-inner">

                <li class="menu-item @(ViewContext.RouteData.Values["controller"]?.ToString() == "Home" ? "active" : "")">
                    <a href="/" class="menu-link">
                        <span class="mini-wrap">
                            <i class="menu-icon bx bx-home-circle"></i>
                            <span class="menu-short">首頁</span>
                        </span>
                        <i class="menu-icon bx bx-home-circle"></i>
                        <span class="menu-label">首頁</span>
                    </a>
                </li>

                <li class="menu-header"><span>商品管理</span></li>
                <li class="menu-item has-sub @(ViewContext.RouteData.Values["controller"]?.ToString() == "Products" ? "active" : "")">
                    <span class="menu-link">
                        <span class="mini-wrap">
                            <i class="menu-icon bx bx-store"></i>
                            <span class="menu-short">商品</span>
                        </span>
                        <i class="menu-icon bx bx-store"></i>
                        <span class="menu-label">商品管理</span>
                        <i class="menu-arrow bx bx-chevron-right"></i>
                    </span>
                    <div class="menu-sub">
                        <ul>
                            <li class="menu-item @(ViewContext.RouteData.Values["controller"]?.ToString() == "Products" && ViewContext.RouteData.Values["action"]?.ToString() == "Index" ? "active" : "")">
                                <a href="@Url.Action("Index", "Products")" class="menu-link">全站商品總覽</a>
                            </li>
                            <li class="menu-item @(ViewContext.RouteData.Values["controller"]?.ToString() == "Products" && ViewContext.RouteData.Values["action"]?.ToString() == "PendingReview" ? "active" : "")">
                                <a href="@Url.Action("PendingReview", "Products")" class="menu-link">待審核商品</a>
                            </li>
                        </ul>
                    </div>
                    <div class="menu-flyout">
                        <div class="menu-flyout-title">商品管理</div>
                        <a href="@Url.Action("Index", "Products")">全站商品總覽</a>
                        <a href="@Url.Action("PendingReview", "Products")">待審核商品</a>
                    </div>
                </li>

                <li class="menu-header"><span>示範頁面</span></li>
                <li class="menu-item has-sub">
                    <span class="menu-link">
                        <span class="mini-wrap">
                            <i class="menu-icon bx bx-grid-alt"></i>
                            <span class="menu-short">報表</span>
                        </span>
                        <i class="menu-icon bx bx-grid-alt"></i>
                        <span class="menu-label">Dashboard</span>
                        <i class="menu-arrow bx bx-chevron-right"></i>
                    </span>
                    <div class="menu-sub">
                        <ul>
                            <li class="menu-item"><a href="#" class="menu-link">Dashboard 1</a></li>
                            <li class="menu-item"><a href="#" class="menu-link">Dashboard 2</a></li>
                            <li class="menu-item"><a href="#" class="menu-link">Dashboard 3</a></li>
                        </ul>
                    </div>
                    <div class="menu-flyout">
                        <div class="menu-flyout-title">Dashboard</div>
                        <a href="#">Dashboard 1</a>
                        <a href="#">Dashboard 2</a>
                        <a href="#">Dashboard 3</a>
                    </div>
                </li>

                <li class="menu-item has-sub">
                    <span class="menu-link">
                        <span class="mini-wrap">
                            <i class="menu-icon bx bx-user"></i>
                            <span class="menu-short">會員</span>
                        </span>
                        <i class="menu-icon bx bx-user"></i>
                        <span class="menu-label">會員管理</span>
                        <i class="menu-arrow bx bx-chevron-right"></i>
                    </span>
                    <div class="menu-sub">
                        <ul>
                            <li class="menu-item"><a href="#" class="menu-link">會員列表</a></li>
                            <li class="menu-item"><a href="#" class="menu-link">黑名單管理</a></li>
                        </ul>
                    </div>
                    <div class="menu-flyout">
                        <div class="menu-flyout-title">會員管理</div>
                        <a href="#">會員列表</a>
                        <a href="#">黑名單管理</a>
                    </div>
                </li>

                <li class="menu-item has-sub">
                    <span class="menu-link">
                        <span class="mini-wrap">
                            <i class="menu-icon bx bx-receipt"></i>
                            <span class="menu-short">訂單</span>
                        </span>
                        <i class="menu-icon bx bx-receipt"></i>
                        <span class="menu-label">訂單管理</span>
                        <i class="menu-arrow bx bx-chevron-right"></i>
                    </span>
                    <div class="menu-sub">
                        <ul>
                            <li class="menu-item"><a href="#" class="menu-link">全部訂單</a></li>
                            <li class="menu-item"><a href="#" class="menu-link">待出貨</a></li>
                            <li class="menu-item"><a href="#" class="menu-link">退貨申請</a></li>
                        </ul>
                    </div>
                    <div class="menu-flyout">
                        <div class="menu-flyout-title">訂單管理</div>
                        <a href="#">全部訂單</a>
                        <a href="#">待出貨</a>
                        <a href="#">退貨申請</a>
                    </div>
                </li>

            </ul>
        </aside>

        <div id="main-wrapper">
            <nav id="layout-navbar">
                <button id="sidebarToggleBtn" title="收合/展開選單">
                    <i class="bx bx-menu"></i>
                </button>
                <div class="navbar-right">
                    <div class="dropdown">
                        <a href="#" class="d-flex align-items-center gap-2 text-decoration-none"
                           data-bs-toggle="dropdown" aria-expanded="false">
                            <div class="avatar avatar-online">
                                <img src="~/sneat/assets/img/avatars/1.png" alt />
                            </div>
                            <span class="fw-semibold d-none d-xl-block" style="color:#2d3748;font-size:.9rem;">John Doe</span>
                            <i class="bx bx-chevron-down d-none d-xl-block" style="color:#8a9bb0;"></i>
                        </a>
                        <ul class="dropdown-menu dropdown-menu-end">
                            <li>
                                <a class="dropdown-item" href="#">
                                    <div class="d-flex align-items-center gap-2">
                                        <div class="avatar avatar-online"><img src="~/sneat/assets/img/avatars/1.png" alt /></div>
                                        <div><span class="fw-semibold d-block">管理員</span><small class="text-muted">Admin</small></div>
                                    </div>
                                </a>
                            </li>
                            <li><hr class="dropdown-divider" /></li>
                            <li><a class="dropdown-item" href="#"><i class="bx bx-user me-2"></i>個人資料</a></li>
                            <li><a class="dropdown-item" href="#"><i class="bx bx-cog me-2"></i>設定</a></li>
                            <li><hr class="dropdown-divider" /></li>
                            <li><a class="dropdown-item" href="#"><i class="bx bx-power-off me-2"></i>登出</a></li>
                        </ul>
                    </div>
                </div>
            </nav>
            <div class="content-wrapper">
                <div class="content-body">@RenderBody()</div>
                <footer class="content-footer">
                    &copy; <script>document.write(new Date().getFullYear())</script> ISpanShop 後台管理系統
                </footer>
            </div>
        </div>
    </div>

    <script src="~/sneat/assets/vendor/libs/jquery/jquery.js"></script>
    <script src="~/sneat/assets/vendor/libs/popper/popper.js"></script>
    <script src="~/sneat/assets/vendor/js/bootstrap.js"></script>
    @await RenderSectionAsync("Scripts", required: false)

    <script>
        (function () {
            var btn     = document.getElementById('sidebarToggleBtn');
            var overlay = document.getElementById('sidebar-overlay');
            var body    = document.body;
            var html    = document.documentElement;
            var KEY     = 'sidebarCollapsed';
            var DESKTOP = 1200;
            function isCollapsed() { return body.classList.contains('sidebar-collapsed'); }
            function isDesktop()   { return window.innerWidth >= DESKTOP; }

            /* 儲存當前頁面 title，下次進入前先顯示 */
            sessionStorage.setItem('pageTitle', document.title);

            /* 展開時隱藏 mini-wrap，只在收合時才用 */
            function applyExpandedIcons() {
                document.querySelectorAll('.menu-link .mini-wrap').forEach(function(w){ w.style.display='none'; });
            }
            function applyCollapsedIcons() {
                /* collapsed 狀態：用 CSS 控制，不需要 JS */
            }

            if (html.classList.contains('pre-collapsed')) {
                body.classList.add('sidebar-collapsed');
                html.classList.remove('pre-collapsed');
            } else {
                applyExpandedIcons();
            }

            if (!isCollapsed() || !isDesktop()) {
                document.querySelectorAll('.has-sub').forEach(function (item) {
                    if (item.querySelector('.menu-sub .menu-item.active')) {
                        item.classList.add('open');
                        /* mark children already-visible: skip flash on same-page re-render */
                        item.querySelectorAll('.menu-sub > ul > .menu-item').forEach(function(li){
                            li.classList.add('pre-open');
                        });
                    }
                });
            }

            requestAnimationFrame(function () {
                html.removeAttribute('data-open-menu');

                requestAnimationFrame(function () {
                    document.querySelectorAll('.menu-sub').forEach(function (sub) {
                        sub.classList.add('animated');
                    });
                });
            });

            btn.addEventListener('click', function () {
                if (isDesktop()) {
                    var c = body.classList.toggle('sidebar-collapsed');
                    sessionStorage.setItem(KEY, c ? '1' : '0');
                    if (!c) { applyExpandedIcons(); }
                } else {
                    body.classList.toggle('mobile-menu-open');
                }
                document.querySelectorAll('.menu-flyout').forEach(function (f) { f.style.display = 'none'; });
            });

            overlay.addEventListener('click', function () { body.classList.remove('mobile-menu-open'); });
            window.addEventListener('resize', function () { if (isDesktop()) body.classList.remove('mobile-menu-open'); });

            document.querySelectorAll('.has-sub > .menu-link').forEach(function (link) {
                link.addEventListener('click', function (e) {
                    e.preventDefault();
                    if (isCollapsed() && isDesktop()) return;
                    var item = this.closest('.menu-item');
                    var isOpen = item.classList.contains('open');
                    document.querySelectorAll('.menu-item.open').forEach(function (el) { if (el !== item) el.classList.remove('open'); });
                    item.classList.toggle('open', !isOpen);
                });
            });

            var activeFlyout = null;
            var flyoutTimer  = null;
            var hoverTimer   = null;
            function showFlyout(flyout, item) {
                clearTimeout(flyoutTimer);
                if (activeFlyout && activeFlyout !== flyout) { activeFlyout.style.display = 'none'; }
                var r = item.getBoundingClientRect();
                flyout.style.top = r.top + 'px';
                flyout.style.display = 'block';
                activeFlyout = flyout;
            }
            function hideFlyout(flyout) {
                flyoutTimer = setTimeout(function () { if (flyout) flyout.style.display = 'none'; activeFlyout = null; }, 120);
            }
            document.querySelectorAll('.has-sub').forEach(function (item) {
                var flyout = item.querySelector('.menu-flyout');
                if (!flyout) return;
                item.addEventListener('mouseenter', function () {
                    if (!isCollapsed() || !isDesktop()) return;
                    hoverTimer = setTimeout(function () { showFlyout(flyout, item); }, 180);
                });
                item.addEventListener('mouseleave', function () {
                    clearTimeout(hoverTimer);
                    if (isCollapsed() && isDesktop()) hideFlyout(flyout);
                });
                flyout.addEventListener('mouseenter', function () { clearTimeout(flyoutTimer); });
                flyout.addEventListener('mouseleave', function () { hideFlyout(flyout); });
            });
        })();
    </script>
</body>
</html>

``n
### 檔案: .\ISpanShop.MVC\Views\Shared\_ValidationScriptsPartial.cshtml
`${ext}
<script src="~/lib/jquery-validation/dist/jquery.validate.min.js"></script>
<script src="~/lib/jquery-validation-unobtrusive/jquery.validate.unobtrusive.min.js"></script>

``n
### 檔案: .\ISpanShop.MVC\Views\Shared\Error.cshtml
`${ext}
@model ErrorViewModel
@{
    ViewData["Title"] = "Error";
}

<h1 class="text-danger">Error.</h1>
<h2 class="text-danger">An error occurred while processing your request.</h2>

@if (Model.ShowRequestId)
{
    <p>
        <strong>Request ID:</strong> <code>@Model.RequestId</code>
    </p>
}

<h3>Development Mode</h3>
<p>
    Swapping to <strong>Development</strong> environment will display more detailed information about the error that occurred.
</p>
<p>
    <strong>The Development environment shouldn't be enabled for deployed applications.</strong>
    It can result in displaying sensitive information from exceptions to end users.
    For local debugging, enable the <strong>Development</strong> environment by setting the <strong>ASPNETCORE_ENVIRONMENT</strong> environment variable to <strong>Development</strong>
    and restarting the app.
</p>

``n
### 檔案: .\ISpanShop.MVC\Views\_ViewImports.cshtml
`${ext}
@using ISpanShop.MVC
@using ISpanShop.MVC.Models
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers

``n
### 檔案: .\ISpanShop.MVC\Views\_ViewStart.cshtml
`${ext}
@{
    Layout = "_Layout";
}
``n
### 檔案: .\ISpanShop.MVC\Program.cs
`${ext}
using ISpanShop.Models.EfModels;
using ISpanShop.Repositories;
using ISpanShop.Repositories.Interfaces;
using ISpanShop.Services;
using ISpanShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ISpanShop.MVC
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllersWithViews();

			builder.Services.AddDbContext<ISpanShopDBContext>(options =>
				options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


			builder.Services.AddScoped<IProductService, ProductService>();


			builder.Services.AddScoped<IProductRepository, ProductRepository>();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");


			using (var scope = app.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				var context = services.GetRequiredService<ISpanShop.Models.EfModels.ISpanShopDBContext>(); // �T�{�A�� DbContext �W��


				await ISpanShop.Models.DataSeeder.SeedAsync(context);
				// 每次啟動確保有 15 筆待審核商品（供測試使用）
				await ISpanShop.Models.DataSeeder.EnsurePendingProductsAsync(context);
			}

			app.Run();
		}
	}
}

``n
### 檔案: .\ISpanShop.Repositories\Interfaces\ICategorySpecRepository.cs
`${ext}
using System.Collections.Generic;
using ISpanShop.Models.EfModels;

namespace ISpanShop.Repositories.Interfaces
{
    /// <summary>
    /// 分類規格 Repository 介面 - 定義分類規格相關的資料存取操作
    /// </summary>
    public interface ICategorySpecRepository
    {
        /// <summary>
        /// 取得所有分類規格列表 (包含關聯的選項)
        /// </summary>
        /// <returns>分類規格集合</returns>
        IEnumerable<Attribute> GetAll();

        /// <summary>
        /// 根據 ID 取得分類規格 (包含關聯的選項)
        /// </summary>
        /// <param name="id">規格 ID</param>
        /// <returns>分類規格實體，若不存在則返回 null</returns>
        Attribute? GetById(int id);

        /// <summary>
        /// 新增分類規格 (同時處理選項的新增)
        /// </summary>
        /// <param name="attribute">分類規格實體</param>
        void Add(Attribute attribute);

        /// <summary>
        /// 更新分類規格 (同時處理選項的新增與刪除)
        /// </summary>
        /// <param name="attribute">分類規格實體</param>
        /// <param name="newOptions">新的選項名稱列表</param>
        void Update(Attribute attribute, List<string> newOptions);

        /// <summary>
        /// 刪除分類規格 (連動刪除關聯的選項與分類綁定)
        /// </summary>
        /// <param name="id">規格 ID</param>
        void Delete(int id);
    }
}

``n
### 檔案: .\ISpanShop.Repositories\Interfaces\IProductRepository.cs
`${ext}
using System.Collections.Generic;
using System.Threading.Tasks;
using ISpanShop.Models.DTOs;
using ISpanShop.Models.EfModels;

namespace ISpanShop.Repositories.Interfaces
{
    /// <summary>
    /// 商品 Repository 介面 - 定義商品相關的資料存取操作
    /// </summary>
    public interface IProductRepository
    {
        /// <summary>
        /// 新增商品 - 包含規格變體與圖片的完整新增
        /// </summary>
        /// <param name="product">商品實體</param>
        void AddProduct(Product product);

        /// <summary>
        /// 檢查 SKU 代碼是否已存在
        /// </summary>
        /// <param name="skuCode">SKU 代碼</param>
        /// <returns>true 表示已存在，false 表示不存在</returns>
        bool IsSkuExists(string skuCode);

        /// <summary>
        /// 取得待審核商品列表 (Status == 2)
        /// </summary>
        /// <returns>待審核商品集合</returns>
        IEnumerable<Product> GetPendingProducts();

        /// <summary>
        /// 更新商品狀態
        /// </summary>
        /// <param name="id">商品 ID</param>
        /// <param name="status">新的狀態值</param>
        void UpdateProductStatus(int id, byte status);

        /// <summary>
        /// 取得所有商品列表
        /// </summary>
        /// <returns>所有商品集合</returns>
        IEnumerable<Product> GetAllProducts();

        /// <summary>
        /// 根據 ID 取得商品詳情（包含圖片與規格）
        /// </summary>
        /// <param name="id">商品 ID</param>
        /// <returns>商品實體，若不存在則返回 null</returns>
        Product? GetProductById(int id);

        /// <summary>
        /// 分頁取得商品列表，支援分類篩選
        /// </summary>
        /// <param name="criteria">搜尋條件（分類篩選 + 分頁）</param>
        /// <returns>商品集合與總筆數</returns>
        (IEnumerable<Product> Items, int TotalCount) GetProductsPaged(ProductSearchCriteria criteria);

        /// <summary>
        /// 取得所有分類（含父子關聯）
        /// </summary>
        /// <returns>所有分類實體集合</returns>
        IEnumerable<Category> GetAllCategories();

        /// <summary>
        /// 取得所有商家清單 (Id, StoreName)
        /// </summary>
        /// <returns>商家清單</returns>
        IEnumerable<(int Id, string Name)> GetStoreOptions();

        /// <summary>
        /// 取得所有品牌清單 (Id, Name)
        /// </summary>
        /// <returns>品牌清單</returns>
        IEnumerable<(int Id, string Name)> GetBrandOptions();

        /// <summary>
        /// 根據子分類取得該分類下商品涵蓋的品牌清單
        /// </summary>
        /// <param name="categoryId">子分類 ID；為 null 時回傳全部品牌</param>
        /// <returns>品牌清單</returns>
        IEnumerable<(int Id, string Name)> GetBrandsByCategory(int? categoryId);

        /// <summary>
        /// 批次更新商品上下架狀態
        /// </summary>
        /// <param name="productIds">要更新的商品 ID 集合</param>
        /// <param name="targetStatus">目標狀態：1 為上架，0 為下架</param>
        /// <returns>實際更新的筆數</returns>
        Task<int> UpdateBatchStatusAsync(List<int> productIds, byte targetStatus);

        /// <summary>
        /// 核准商品審核（Status → 1 上架）
        /// </summary>
        /// <param name="id">商品 ID</param>
        void ApproveProduct(int id);

        /// <summary>
        /// 退回商品審核（Status → 3 審核退回）
        /// </summary>
        /// <param name="id">商品 ID</param>
        void RejectProduct(int id, string? reason);

        /// <summary>
        /// 取得最近退回的商品清單（Status == 3），依 UpdatedAt 降冪排序
        /// </summary>
        /// <param name="top">最多取幾筆</param>
        IEnumerable<Product> GetRecentRejectedProducts(int top);
    }
}

``n
### 檔案: .\ISpanShop.Repositories\CategorySpecRepository.cs
`${ext}
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ISpanShop.Models.EfModels;
using ISpanShop.Repositories.Interfaces;

namespace ISpanShop.Repositories
{
    /// <summary>
    /// 分類規格 Repository 實作 - 處理分類規格的 CRUD 操作
    /// </summary>
    public class CategorySpecRepository : ICategorySpecRepository
    {
        private readonly ISpanShopDBContext _context;

        /// <summary>
        /// 建構子 - 注入 DbContext
        /// </summary>
        /// <param name="context">ISpanShop 資料庫上下文</param>
        public CategorySpecRepository(ISpanShopDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 取得所有分類規格列表 (包含關聯的選項，依排序順序排列)
        /// </summary>
        /// <returns>分類規格集合</returns>
        public IEnumerable<Attribute> GetAll()
        {
            return _context.Attributes
                .Include(a => a.AttributeOptions.OrderBy(o => o.SortOrder))
                .OrderBy(a => a.SortOrder)
                .ToList();
        }

        /// <summary>
        /// 根據 ID 取得分類規格 (包含關聯的選項)
        /// </summary>
        /// <param name="id">規格 ID</param>
        /// <returns>分類規格實體，若不存在則返回 null</returns>
        public Attribute? GetById(int id)
        {
            return _context.Attributes
                .Include(a => a.AttributeOptions.OrderBy(o => o.SortOrder))
                .FirstOrDefault(a => a.Id == id);
        }

        /// <summary>
        /// 新增分類規格 (同時處理選項的新增)
        /// </summary>
        /// <param name="attribute">分類規格實體</param>
        public void Add(Attribute attribute)
        {
            _context.Attributes.Add(attribute);
            _context.SaveChanges();
        }

        /// <summary>
        /// 更新分類規格 (同時處理選項的新增與刪除)
        /// </summary>
        /// <param name="attribute">分類規格實體</param>
        /// <param name="newOptions">新的選項名稱列表</param>
        public void Update(Attribute attribute, List<string> newOptions)
        {
            // 更新規格本身的資料
            _context.Attributes.Update(attribute);

            // 刪除舊的選項
            var existingOptions = _context.AttributeOptions
                .Where(o => o.AttributeId == attribute.Id)
                .ToList();
            _context.AttributeOptions.RemoveRange(existingOptions);

            // 新增新的選項 (只有在有選項的情況下才新增)
            if (newOptions != null && newOptions.Count > 0)
            {
                var sortOrder = 0;
                foreach (var optionName in newOptions.Where(o => !string.IsNullOrWhiteSpace(o)))
                {
                    _context.AttributeOptions.Add(new AttributeOption
                    {
                        AttributeId = attribute.Id,
                        OptionName = optionName.Trim(),
                        SortOrder = sortOrder++
                    });
                }
            }

            _context.SaveChanges();
        }

        /// <summary>
        /// 刪除分類規格 (連動刪除關聯的選項與分類綁定)
        /// </summary>
        /// <param name="id">規格 ID</param>
        public void Delete(int id)
        {
            var attribute = _context.Attributes.Find(id);
            if (attribute != null)
            {
                // EF Core 會自動依據 Cascade Delete 刪除關聯的 AttributeOptions 與 CategoryAttributes
                _context.Attributes.Remove(attribute);
                _context.SaveChanges();
            }
        }
    }
}

``n
### 檔案: .\ISpanShop.Repositories\ProductRepository.cs
`${ext}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ISpanShop.Models.DTOs;
using ISpanShop.Models.EfModels;
using ISpanShop.Repositories.Interfaces;

namespace ISpanShop.Repositories
{
    /// <summary>
    /// 商品 Repository 實作 - 處理商品的 CRUD 操作
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly ISpanShopDBContext _context;

        /// <summary>
        /// 建構子 - 注入 DbContext
        /// </summary>
        /// <param name="context">ISpanShop 資料庫上下文</param>
        public ProductRepository(ISpanShopDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 新增商品 - 依賴 EF Core 的關聯追蹤來一併儲存 Variant 和 Image
        /// </summary>
        /// <param name="product">商品實體</param>
        public void AddProduct(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        /// <summary>
        /// 檢查 SKU 代碼是否已存在
        /// </summary>
        /// <param name="skuCode">SKU 代碼</param>
        /// <returns>true 表示已存在，false 表示不存在</returns>
        public bool IsSkuExists(string skuCode)
        {
            return _context.ProductVariants.Any(pv => pv.SkuCode == skuCode);
        }

        /// <summary>
        /// 取得待審核商品列表 (Status == 2)
        /// </summary>
        /// <returns>待審核商品集合</returns>
        public IEnumerable<Product> GetPendingProducts()
        {
            return _context.Products
                .Include(p => p.Store)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Where(p => p.Status == 2)
                .ToList();
        }

        /// <summary>
        /// 更新商品狀態
        /// </summary>
        /// <param name="id">商品 ID</param>
        /// <param name="status">新的狀態值</param>
        public void UpdateProductStatus(int id, byte status)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                product.Status = status;
                product.UpdatedAt = DateTime.Now;
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// 取得所有商品列表 - 包含關聯資料
        /// </summary>
        /// <returns>所有商品集合</returns>
        public IEnumerable<Product> GetAllProducts()
        {
            return _context.Products
                .Include(p => p.Store)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.ProductImages)
                .ToList();
        }

        /// <summary>
        /// 根據 ID 取得商品詳情（包含圖片與規格）
        /// </summary>
        /// <param name="id">商品 ID</param>
        /// <returns>商品實體，若不存在則返回 null</returns>
        public Product? GetProductById(int id)
        {
            return _context.Products
                .Include(p => p.Store)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductVariants)
                .FirstOrDefault(p => p.Id == id);
        }

        /// <summary>
        /// 分頁取得商品列表，支援分類篩選與多維度搜尋
        /// </summary>
        /// <param name="criteria">搜尋條件（分類、關鍵字、商家、狀態篩選 + 分頁）</param>
        /// <returns>商品集合與總筆數</returns>
        public (IEnumerable<Product> Items, int TotalCount) GetProductsPaged(ProductSearchCriteria criteria)
        {
            var query = _context.Products
                .Include(p => p.Store)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.ProductImages)
                .AsQueryable();

            // 分類篩選：優先以子分類篩選，否則以主分類篩選所有子分類商品
            if (criteria.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == criteria.CategoryId.Value);
            }
            else if (criteria.ParentCategoryId.HasValue)
            {
                query = query.Where(p => p.Category.ParentId == criteria.ParentCategoryId.Value);
            }

            // 關鍵字搜尋：在名稱或描述中包含關鍵字（Description 可能為 null，需防呆）
            if (!string.IsNullOrWhiteSpace(criteria.Keyword))
            {
                var keyword = criteria.Keyword.Trim().ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(keyword) ||
                    (p.Description != null && p.Description.ToLower().Contains(keyword))
                );
            }

            // 商家篩選：精準匹配商家 ID
            if (criteria.StoreId.HasValue)
            {
                query = query.Where(p => p.StoreId == criteria.StoreId.Value);
            }

            // 品牌篩選：精準匹配品牌 ID
            if (criteria.BrandId.HasValue)
            {
                query = query.Where(p => p.BrandId == criteria.BrandId.Value);
            }

            // 商品狀態篩選：精準匹配狀態
            if (criteria.Status.HasValue)
            {
                query = query.Where(p => p.Status == criteria.Status.Value);
            }

            // 建檔日期起：>= StartDate
            if (criteria.StartDate.HasValue)
            {
                query = query.Where(p => p.CreatedAt >= criteria.StartDate.Value);
            }

            // 建檔日期迄：<= EndDate 當天最後一刻（23:59:59.9999999）
            if (criteria.EndDate.HasValue)
            {
                var endOfDay = criteria.EndDate.Value.AddDays(1).AddTicks(-1);
                query = query.Where(p => p.CreatedAt <= endOfDay);
            }

            int totalCount = query.Count();

            var items = query
                .Skip((criteria.PageNumber - 1) * criteria.PageSize)
                .Take(criteria.PageSize)
                .ToList();

            return (items, totalCount);
        }

        /// <summary>
        /// 取得所有分類（含父子關聯）
        /// </summary>
        /// <returns>所有分類集合</returns>
        public IEnumerable<Category> GetAllCategories()
        {
            return _context.Categories.ToList();
        }

        /// <summary>
        /// 取得所有商家清單 (Id, StoreName)
        /// </summary>
        /// <returns>商家清單</returns>
        public IEnumerable<(int Id, string Name)> GetStoreOptions()
        {
            return _context.Stores
                .Select(s => new { s.Id, s.StoreName })
                .ToList()
                .Select(s => (s.Id, s.StoreName));
        }
        /// <summary>
        /// 取得所有品牌清單 (Id, Name)
        /// </summary>
        /// <returns>品牌清單</returns>
        public IEnumerable<(int Id, string Name)> GetBrandOptions()
        {
            return _context.Brands
                .Where(b => b.IsDeleted != true)
                .Select(b => new { b.Id, b.Name })
                .ToList()
                .Select(b => (b.Id, b.Name));
        }
        /// <summary>
        /// 根據子分類取得該分類下商品涵蓋的品牌清單
        /// </summary>
        /// <param name="categoryId">子分類 ID；為 null 時回傳全部品牌</param>
        /// <returns>品牌清單（依名稱排序）</returns>
        public IEnumerable<(int Id, string Name)> GetBrandsByCategory(int? categoryId)
        {
            return _context.Products
                .Where(p => categoryId == null || p.CategoryId == categoryId)
                .Where(p => p.BrandId != null && p.Brand != null && p.Brand.IsDeleted != true)
                .Select(p => new { p.Brand!.Id, p.Brand!.Name })
                .Distinct()
                .OrderBy(b => b.Name)
                .ToList()
                .Select(b => (b.Id, b.Name));
        }

        /// <summary>
        /// 批次更新商品上下架狀態
        /// </summary>
        public async Task<int> UpdateBatchStatusAsync(List<int> productIds, byte targetStatus)
        {
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            foreach (var product in products)
            {
                product.Status = targetStatus;
                product.UpdatedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return products.Count;
        }

        /// <summary>
        /// 核准商品審核 - 將 Status 設為 1 (上架)
        /// </summary>
        public void ApproveProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                product.Status = 1;
                product.UpdatedAt = DateTime.Now;
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// 退回商品審核 - 將 Status 設為 3 (審核退回)，並儲存退回原因
        /// </summary>
        public void RejectProduct(int id, string? reason)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                product.Status = 3;
                product.RejectReason = reason;
                product.UpdatedAt = DateTime.Now;
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// 取得最近退回的商品清單（Status == 3），依 UpdatedAt 降冪排序
        /// </summary>
        public IEnumerable<Product> GetRecentRejectedProducts(int top)
        {
            return _context.Products
                .Include(p => p.Store)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.ProductImages)
                .Where(p => p.Status == 3)
                .OrderByDescending(p => p.UpdatedAt)
                .Take(top)
                .ToList();
        }
    }
}

``n
### 檔案: .\ISpanShop.Services\Interfaces\IProductService.cs
`${ext}
using System.Collections.Generic;
using System.Threading.Tasks;
using ISpanShop.Models.DTOs;

namespace ISpanShop.Services.Interfaces
{
    /// <summary>
    /// 商品 Service 介面 - 定義商品相關的商業邏輯操作
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// 建立新商品
        /// </summary>
        /// <param name="dto">商品建立 DTO</param>
        void CreateProduct(ProductCreateDto dto);

        /// <summary>
        /// 取得待審核商品列表
        /// </summary>
        /// <returns>待審核商品 DTO 集合</returns>
        IEnumerable<ProductReviewDto> GetPendingProducts();

        /// <summary>
        /// 變更商品狀態
        /// </summary>
        /// <param name="id">商品 ID</param>
        /// <param name="newStatus">新的狀態值</param>
        void ChangeProductStatus(int id, byte newStatus);

        /// <summary>
        /// 取得所有商品列表
        /// </summary>
        /// <returns>商品列表 DTO 集合</returns>
        IEnumerable<ProductListDto> GetAllProducts();

        /// <summary>
        /// 分頁取得商品列表，支援分類篩選
        /// </summary>
        /// <param name="criteria">搜尋條件（分類篩選 + 分頁）</param>
        /// <returns>分頁商品列表 DTO</returns>
        PagedResult<ProductListDto> GetProductsPaged(ProductSearchCriteria criteria);

        /// <summary>
        /// 取得所有分類清單（含主分類與子分類）
        /// </summary>
        /// <returns>分類 DTO 集合</returns>
        IEnumerable<CategoryDto> GetAllCategories();

        /// <summary>
        /// 根據 ID 取得商品詳情（包含圖片與規格）
        /// </summary>
        /// <param name="id">商品 ID</param>
        /// <returns>商品詳情 DTO，若不存在則返回 null</returns>
        ProductDetailDto? GetProductDetail(int id);

        /// <summary>
        /// 取得所有商家清單 (Id, Name)
        /// </summary>
        /// <returns>商家清單</returns>
        IEnumerable<(int Id, string Name)> GetStoreOptions();

        /// <summary>
        /// 取得所有品牌清單 (Id, Name)
        /// </summary>
        /// <returns>品牌清單</returns>
        IEnumerable<(int Id, string Name)> GetBrandOptions();

        /// <summary>
        /// 根據子分類取得該分類下商品涵蓋的品牌清單
        /// </summary>
        /// <param name="categoryId">子分類 ID；為 null 時回傳全部品牌</param>
        /// <returns>品牌清單</returns>
        IEnumerable<(int Id, string Name)> GetBrandsByCategory(int? categoryId);

        /// <summary>
        /// 批次更新商品上下架狀態
        /// </summary>
        /// <param name="productIds">要更新的商品 ID 集合</param>
        /// <param name="targetStatus">目標狀態：1 為上架，0 為下架</param>
        /// <returns>實際更新的筆數</returns>
        Task<int> UpdateBatchStatusAsync(List<int> productIds, byte targetStatus);

        /// <summary>
        /// 核准商品（待審核 → 上架）
        /// </summary>
        /// <param name="id">商品 ID</param>
        void ApproveProduct(int id);

        /// <summary>
        /// 退回商品（待審核 → 審核退回）
        /// </summary>
        /// <param name="id">商品 ID</param>
        /// <param name="reason">退回原因（記錄於回應訊息，不持久化）</param>
        void RejectProduct(int id, string? reason);

        /// <summary>
        /// 取得最近退回的商品清單（Status == 3），依 UpdatedAt 降冪排序
        /// </summary>
        /// <param name="top">最多取幾筆，預設 10</param>
        IEnumerable<ProductReviewDto> GetRecentRejectedProducts(int top = 10);
    }
}

``n
### 檔案: .\ISpanShop.Services\CategorySpecService.cs
`${ext}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISpanShop.Services
{
	internal class CategorySpecService
	{
	}
}

``n
### 檔案: .\ISpanShop.Services\ProductService.cs
`${ext}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ISpanShop.Models.DTOs;
using ISpanShop.Models.EfModels;
using ISpanShop.Repositories;
using ISpanShop.Repositories.Interfaces;
using ISpanShop.Services.Interfaces;

namespace ISpanShop.Services
{
    /// <summary>
    /// 商品 Service 實作 - 處理商品相關的商業邏輯
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        /// <summary>
        /// 建構子 - 注入 ProductRepository
        /// </summary>
        /// <param name="productRepository">商品 Repository</param>
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        /// <summary>
        /// 建立新商品 - 包含商業邏輯轉換與驗證
        /// </summary>
        /// <param name="dto">商品建立 DTO</param>
        public void CreateProduct(ProductCreateDto dto)
        {
            // DTO 轉換為 Product Entity
            var product = new Product
            {
                StoreId = dto.StoreId,
                CategoryId = dto.CategoryId,
                BrandId = dto.BrandId,
                Name = dto.Name,
                Description = dto.Description,
                VideoUrl = dto.VideoUrl,
                SpecDefinitionJson = dto.SpecDefinitionJson,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Status = AutoReviewProduct(dto.Name, dto.Description)
            };

            // 商業邏輯一：計算價格區間 (最低價與最高價)
            if (dto.Variants != null && dto.Variants.Count > 0)
            {
                product.MinPrice = dto.Variants.Min(v => v.Price);
                product.MaxPrice = dto.Variants.Max(v => v.Price);

                // 商業邏輯二：SKU 生成與 Variant 轉換
                product.ProductVariants = dto.Variants.Select(variantDto =>
                {
                    // 如果 SkuCode 為 null 或空字串，自動產生唯一代碼
                    string skuCode = string.IsNullOrWhiteSpace(variantDto.SkuCode)
                        ? GenerateUniqueSku()
                        : variantDto.SkuCode;

                    return new ProductVariant
                    {
                        SkuCode = skuCode,
                        VariantName = variantDto.VariantName,
                        SpecValueJson = variantDto.SpecValueJson,
                        Price = variantDto.Price,
                        Stock = variantDto.Stock,
                        SafetyStock = variantDto.SafetyStock,
                    };
                }).ToList();
            }

            // TODO: 處理實體檔案上傳與 ProductImages 資料表寫入，我們稍後再獨立處理。

            // 將轉換好的 Product Entity 交給 Repository 儲存
            _productRepository.AddProduct(product);
        }

        /// <summary>
        /// 生成唯一的 SKU 代碼
        /// </summary>
        /// <returns>唯一的 SKU 代碼</returns>
        private string GenerateUniqueSku()
        {
            // 使用 Guid 的前 8 個字元 + 時間戳記
            string guidPart = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
            string timePart = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            return $"{guidPart}-{timePart}";
        }

        /// <summary>
        /// 自動化商品審核機制
        /// 規則：含違禁詞 → 3(退回)；描述過短 → 2(待手動審核)；正常 → 1(自動通過上架)
        /// </summary>
        private byte AutoReviewProduct(string name, string description)
        {
            var badWords = new[]
            {
                "高仿", "原單", "槍械", "毒品", "贗品", "假貨", "冒牌", "盜版",
                "走私", "非法", "詐騙", "傳銷", "洗錢", "賭博", "色情",
                "暴力", "恐怖", "炸藥", "大麻", "槍", "彈藥", "仿冒"
            };

            string combined = (name + " " + (description ?? "")).ToLower();

            if (badWords.Any(w => combined.Contains(w.ToLower())))
                return 3; // 自動退回

            if (string.IsNullOrWhiteSpace(description) || description.Trim().Length < 10)
                return 2; // 描述過短，待手動審核

            return 1; // 自動通過，直接上架
        }

        /// <summary>
        /// 取得待審核商品列表 - 轉換為 DTO
        /// </summary>
        /// <returns>待審核商品 DTO 集合</returns>
        public IEnumerable<ProductReviewDto> GetPendingProducts()
        {
            var pendingProducts = _productRepository.GetPendingProducts();

            return pendingProducts.Select(p => new ProductReviewDto
            {
                Id          = p.Id,
                StoreId     = p.StoreId,
                CategoryName = p.Category?.Name ?? "未分類",
                BrandName   = p.Brand?.Name ?? "未設定",
                StoreName   = p.Store?.StoreName ?? "未知商店",
                Name        = p.Name,
                Description = p.Description,
                Status      = p.Status ?? 0,
                CreatedAt   = p.CreatedAt,
                UpdatedAt   = p.UpdatedAt,
                MainImageUrl = p.ProductImages
                    ?.FirstOrDefault(img => img.IsMain == true)?.ImageUrl
            }).ToList();
        }

        /// <summary>
        /// 變更商品狀態
        /// </summary>
        /// <param name="id">商品 ID</param>
        /// <param name="newStatus">新的狀態值</param>
        public void ChangeProductStatus(int id, byte newStatus)
        {
            _productRepository.UpdateProductStatus(id, newStatus);
        }

        /// <summary>
        /// 取得所有商品列表 - 轉換為 DTO
        /// </summary>
        /// <returns>商品列表 DTO 集合</returns>
        public IEnumerable<ProductListDto> GetAllProducts()
        {
            var allProducts = _productRepository.GetAllProducts();

            return allProducts.Select(p => new ProductListDto
            {
                Id = p.Id,
                StoreName = p.Store?.StoreName ?? "未知商店",
                CategoryName = p.Category?.Name ?? "未分類",
                BrandName = p.Brand?.Name ?? "未設定",
                Name = p.Name,
                MinPrice = p.MinPrice,
                MaxPrice = p.MaxPrice,
                Status = p.Status,
                MainImageUrl = p.ProductImages
                    ?.FirstOrDefault(img => img.IsMain==true)?.ImageUrl 
                    ?? p.ProductImages?.FirstOrDefault()?.ImageUrl 
                    ?? "https://via.placeholder.com/400x400?text=No+Image"
            }).ToList();
        }

        /// <summary>
        /// 分頁取得商品列表，支援分類篩選
        /// </summary>
        /// <param name="criteria">搜尋條件</param>
        /// <returns>分頁商品列表 DTO</returns>
        public PagedResult<ProductListDto> GetProductsPaged(ProductSearchCriteria criteria)
        {
            var (items, totalCount) = _productRepository.GetProductsPaged(criteria);

            var dtos = items.Select(p => new ProductListDto
            {
                Id = p.Id,
                StoreName = p.Store?.StoreName ?? "未知商店",
                CategoryName = p.Category?.Name ?? "未分類",
                BrandName = p.Brand?.Name ?? "未設定",
                Name = p.Name,
                MinPrice = p.MinPrice,
                MaxPrice = p.MaxPrice,
                Status = p.Status,
                MainImageUrl = p.ProductImages
                    ?.FirstOrDefault(img => img.IsMain == true)?.ImageUrl
                    ?? p.ProductImages?.FirstOrDefault()?.ImageUrl
                    ?? "https://via.placeholder.com/400x400?text=No+Image",
                CreatedAt = p.CreatedAt
            }).ToList();

            return PagedResult<ProductListDto>.Create(dtos, totalCount, criteria.PageNumber, criteria.PageSize);
        }

        /// <summary>
        /// 取得所有分類清單（含主分類與子分類）
        /// </summary>
        /// <returns>分類 DTO 集合</returns>
        public IEnumerable<CategoryDto> GetAllCategories()
        {
            return _productRepository.GetAllCategories()
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ParentId = c.ParentId
                })
                .ToList();
        }

        /// <summary>
        /// 根據 ID 取得商品詳情 - 轉換為 DTO（包含完整圖片與規格列表）
        /// </summary>
        /// <param name="id">商品 ID</param>
        /// <returns>商品詳情 DTO，若不存在則返回 null</returns>
        public ProductDetailDto? GetProductDetail(int id)
        {
            var product = _productRepository.GetProductById(id);

            if (product == null)
            {
                return null;
            }

            return new ProductDetailDto
            {
                Id = product.Id,
                Name = product.Name,
                StoreName = product.Store?.StoreName ?? "未知商店",
                CategoryName = product.Category?.Name ?? "未分類",
                BrandName = product.Brand?.Name ?? "未設定",
                Description = product.Description,
                Status = product.Status,
                Images = product.ProductImages?
                    .OrderBy(img => img.SortOrder)
                    .Select(img => img.ImageUrl)
                    .ToList() ?? new List<string>(),
                Variants = product.ProductVariants?
                    .Where(v => !v.IsDeleted==false)
                    .Select(v => new ProductVariantDetailDto
                    {
                        SkuCode = v.SkuCode,
                        VariantName = v.VariantName,
                        Price = v.Price,
                        Stock = v.Stock ?? 0,
                        SpecValueJson = v.SpecValueJson
                    })
                    .ToList() ?? new List<ProductVariantDetailDto>()
            };
        }

        /// <summary>
        /// 取得所有商家清單
        /// </summary>
        /// <returns>商家清單 (Id, Name)</returns>
        public IEnumerable<(int Id, string Name)> GetStoreOptions()
        {
            return _productRepository.GetStoreOptions();
        }

        /// <summary>
        /// 取得所有品牌清單
        /// </summary>
        /// <returns>品牌清單 (Id, Name)</returns>
        public IEnumerable<(int Id, string Name)> GetBrandOptions()
        {
            return _productRepository.GetBrandOptions();
        }

        /// <summary>
        /// 根據子分類取得該分類下商品涵蓋的品牌清單
        /// </summary>
        /// <param name="categoryId">子分類 ID；為 null 時回傳全部品牌</param>
        /// <returns>品牌清單 (Id, Name)</returns>
        public IEnumerable<(int Id, string Name)> GetBrandsByCategory(int? categoryId)
        {
            return _productRepository.GetBrandsByCategory(categoryId);
        }

        /// <summary>
        /// 批次更新商品上下架狀態
        /// </summary>
        public async Task<int> UpdateBatchStatusAsync(List<int> productIds, byte targetStatus)
        {
            if (productIds == null || productIds.Count == 0) return 0;
            return await _productRepository.UpdateBatchStatusAsync(productIds, targetStatus);
        }

        /// <summary>
        /// 核准商品審核 - 將狀態設為 1 (上架)
        /// </summary>
        public void ApproveProduct(int id)
        {
            _productRepository.ApproveProduct(id);
        }

        /// <summary>
        /// 退回商品審核 - 將狀態設為 3 (審核退回)
        /// </summary>
        public void RejectProduct(int id, string? reason)
        {
            _productRepository.RejectProduct(id, reason);
        }

        /// <summary>
        /// 取得最近退回的商品清單（Status == 3）
        /// </summary>
        public IEnumerable<ProductReviewDto> GetRecentRejectedProducts(int top = 10)
        {
            return _productRepository.GetRecentRejectedProducts(top)
                .Select(p => new ProductReviewDto
                {
                    Id          = p.Id,
                    StoreId     = p.StoreId,
                    CategoryName = p.Category?.Name ?? "未分類",
                    BrandName   = p.Brand?.Name ?? "未設定",
                    StoreName   = p.Store?.StoreName ?? "未知商店",
                    Name        = p.Name,
                    Description = p.Description,
                    Status      = p.Status ?? 3,
                    RejectReason = p.RejectReason,
                    CreatedAt   = p.CreatedAt,
                    UpdatedAt   = p.UpdatedAt,
                    MainImageUrl = p.ProductImages
                        ?.FirstOrDefault(img => img.IsMain == true)?.ImageUrl
                }).ToList();
        }
    }
}

``n
### 檔案: .\ISpanShop.WebAPI\Controllers\ProductsApiController.cs
`${ext}
using Microsoft.AspNetCore.Mvc;
using ISpanShop.Models.DTOs;
using ISpanShop.Services;
using ISpanShop.Services.Interfaces;

namespace ISpanShop.WebAPI.Controllers
{
    /// <summary>
    /// 商品 API 控制器 - 提供商品相關的 RESTful API 端點
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsApiController : ControllerBase
    {
        private readonly IProductService _productService;

        /// <summary>
        /// 建構子 - 注入 ProductService
        /// </summary>
        /// <param name="productService">商品 Service</param>
        public ProductsApiController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// 建立新商品
        /// </summary>
        /// <param name="dto">商品建立 DTO (使用 FormData 以支持圖片上傳)</param>
        /// <returns>建立成功訊息</returns>
        [HttpPost]
        public IActionResult CreateProduct([FromForm] ProductCreateDto dto)
        {
            _productService.CreateProduct(dto);
            return Ok(new { message = "商品建立成功" });
        }
    }
}

``n
### 檔案: .\ISpanShop.WebAPI\Controllers\WeatherForecastController.cs
`${ext}
using Microsoft.AspNetCore.Mvc;

namespace ISpanShop.WebAPI.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class WeatherForecastController : ControllerBase
	{
		private static readonly string[] Summaries = new[]
		{
			"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		};

		private readonly ILogger<WeatherForecastController> _logger;

		public WeatherForecastController(ILogger<WeatherForecastController> logger)
		{
			_logger = logger;
		}

		[HttpGet(Name = "GetWeatherForecast")]
		public IEnumerable<WeatherForecast> Get()
		{
			return Enumerable.Range(1, 5).Select(index => new WeatherForecast
			{
				Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
				TemperatureC = Random.Shared.Next(-20, 55),
				Summary = Summaries[Random.Shared.Next(Summaries.Length)]
			})
			.ToArray();
		}
	}
}

``n
### 檔案: .\ISpanShop.WebAPI\Program.cs
`${ext}

namespace ISpanShop.WebAPI
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}

``n
### 檔案: .\ISpanShop.WebAPI\WeatherForecast.cs
`${ext}
namespace ISpanShop.WebAPI
{
	public class WeatherForecast
	{
		public DateOnly Date { get; set; }

		public int TemperatureC { get; set; }

		public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

		public string? Summary { get; set; }
	}
}

``n
