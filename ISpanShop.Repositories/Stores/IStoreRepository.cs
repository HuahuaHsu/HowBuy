using ISpanShop.Models.DTOs.Stores;
using System.Collections.Generic;

namespace ISpanShop.Repositories.Stores
{
    public interface IStoreRepository
    {
        IEnumerable<StoreDto> GetAllStores();
        StoreDetailDto? GetStoreById(int storeId);
        bool ToggleVerified(int storeId, bool isVerified);
        bool ToggleBlacklist(int userId, bool isBlacklisted);
    }
}
