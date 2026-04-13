using System.Collections.Generic;
using System.Threading.Tasks;
using ISpanShop.Models.DTOs.Brands;
using ISpanShop.Repositories.Brands;

namespace ISpanShop.Services.Brands
{
    public class BrandService
    {
        private readonly IBrandRepository _repo;

        public BrandService(IBrandRepository repo) => _repo = repo;

        /// <summary>取得有上架商品的品牌列表（含商品數）。categoryId 支援主分類展開；keyword 模糊篩選品牌名稱。</summary>
        public Task<IEnumerable<BrandWithCountDto>> GetBrandsAsync(int? categoryId, string? keyword)
            => _repo.GetBrandsAsync(categoryId, keyword);
    }
}
