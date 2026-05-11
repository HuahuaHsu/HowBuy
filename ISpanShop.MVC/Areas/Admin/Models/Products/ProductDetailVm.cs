using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace ISpanShop.MVC.Areas.Admin.Models.Products
{
    public class ProductDetailVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string StoreName { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public string Description { get; set; }
        public string PlainDescription => ToPlainText(Description);
        public string AdminDescriptionHtml => ToAdminDescriptionHtml(Description);
        public bool HasAdminDescription => !string.IsNullOrWhiteSpace(AdminDescriptionHtml);
        public byte? Status { get; set; }
        public int CategoryId { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? TotalSales { get; set; }
        public int? ViewCount { get; set; }
        public string? RejectReason { get; set; }
        public string? SpecDefinitionJson { get; set; }
        public string? AttributesJson { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        /// <summary>審核狀態 (0=待審核, 1=通過, 2=退回, 3=重新申請審核)</summary>
        public int ReviewStatus { get; set; }
        public string? ReviewedBy { get; set; }
        public DateTime? ReviewDate { get; set; }
        public string? ForceOffShelfReason { get; set; }
        public DateTime? ForceOffShelfDate { get; set; }
        public int? ForceOffShelfBy { get; set; }
        public DateTime? ReApplyDate { get; set; }

        public List<string> Images { get; set; } = new();
        public List<ProductVariantDetailVm> Variants { get; set; } = new();
        public List<ProductAttributeDisplayVm> Attributes { get; set; } = new();

        public string StatusText
        {
            get => Status switch
            {
                1 => "已上架",
                2 => "待審核",
                3 => "審核退回",
                4 => "強制下架",
                0 => "已下架",
                _ => "未知"
            };
        }

        public string StatusBadgeClass
        {
            get => Status switch
            {
                1 => "badge-success",
                2 => "badge-warning",
                3 => "badge-danger",
                4 => "badge-danger",
                0 => "badge-secondary",
                _ => "badge-secondary"
            };
        }

        private static string ToPlainText(string? html)
        {
            if (string.IsNullOrWhiteSpace(html)) return string.Empty;

            var withBreaks = Regex.Replace(html, @"</(p|div|br|li|h[1-6])\s*>", "\n", RegexOptions.IgnoreCase);
            var withoutTags = Regex.Replace(withBreaks, "<.*?>", string.Empty);
            var decoded = WebUtility.HtmlDecode(withoutTags).Replace('\u00A0', ' ');
            return Regex.Replace(decoded, @"[ \t]+", " ").Trim();
        }

        private static string ToAdminDescriptionHtml(string? html)
        {
            if (string.IsNullOrWhiteSpace(html)) return string.Empty;

            var imageTags = new List<string>();
            var withImageTokens = Regex.Replace(html, @"<img\b[^>]*>", match =>
            {
                var src = ExtractImageSrc(match.Value);
                if (string.IsNullOrWhiteSpace(src) || !IsAllowedImageSrc(src))
                {
                    return string.Empty;
                }

                var token = $"[[ADMIN_DESC_IMG_{imageTags.Count}]]";
                imageTags.Add(
                    $"<img src=\"{WebUtility.HtmlEncode(src)}\" alt=\"商品描述圖片\" loading=\"lazy\" class=\"oc-description-image\" />"
                );
                return $"\n{token}\n";
            }, RegexOptions.IgnoreCase);

            var withBreaks = Regex.Replace(withImageTokens, @"</(p|div|br|li|h[1-6])\s*>", "\n", RegexOptions.IgnoreCase);
            var withoutTags = Regex.Replace(withBreaks, "<.*?>", string.Empty);
            var decoded = WebUtility.HtmlDecode(withoutTags).Replace('\u00A0', ' ');
            var normalized = Regex.Replace(decoded, @"[ \t]+", " ").Trim();
            normalized = Regex.Replace(normalized, @"\n{3,}", "\n\n");

            var encoded = WebUtility.HtmlEncode(normalized).Replace("\r\n", "\n").Replace("\n", "<br />");
            for (var i = 0; i < imageTags.Count; i++)
            {
                encoded = encoded.Replace($"[[ADMIN_DESC_IMG_{i}]]", imageTags[i]);
            }

            return encoded;
        }

        private static string? ExtractImageSrc(string imgTag)
        {
            var match = Regex.Match(
                imgTag,
                @"\bsrc\s*=\s*(?:""(?<src>[^""]*)""|'(?<src>[^']*)'|(?<src>[^\s>]+))",
                RegexOptions.IgnoreCase
            );
            return match.Success ? WebUtility.HtmlDecode(match.Groups["src"].Value) : null;
        }

        private static bool IsAllowedImageSrc(string src)
        {
            if (src.StartsWith("/", StringComparison.Ordinal)) return true;
            return Uri.TryCreate(src, UriKind.Absolute, out var uri)
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }
    }

    public class ProductAttributeDisplayVm
    {
        public int AttributeId { get; set; }
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
