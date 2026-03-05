using System.Collections.Generic;
using ISpanShop.Models.DTOs;
using ISpanShop.Repositories.Interfaces;

namespace ISpanShop.Services
{
    public class CategorySpecService
    {
        private readonly ICategorySpecRepository _categorySpecRepository;

        public CategorySpecService(ICategorySpecRepository categorySpecRepository)
        {
            _categorySpecRepository = categorySpecRepository;
        }

        public IEnumerable<CategorySpecDto> GetAll()
        {
            return _categorySpecRepository.GetAll();
        }

        public CategorySpecDto? GetById(int id)
        {
            return _categorySpecRepository.GetById(id);
        }

        public void Create(string name, string inputType, bool isRequired, int sortOrder, List<string> options)
        {
            var cleanOptions = NeedsOptions(inputType) ? options : new List<string>();
            _categorySpecRepository.Create(name, inputType, isRequired, sortOrder, cleanOptions);
        }

        public void Update(int id, string name, string inputType, bool isRequired, int sortOrder, List<string> options)
        {
            var cleanOptions = NeedsOptions(inputType) ? options : new List<string>();
            _categorySpecRepository.Update(id, name, inputType, isRequired, sortOrder, cleanOptions);
        }

        public void Delete(int id)
        {
            _categorySpecRepository.Delete(id);
        }

        public void UpdateIsActive(int id, bool isActive)
        {
            _categorySpecRepository.UpdateIsActive(id, isActive);
        }

        // ── 分類綁定相關 ──────────────────────────────

        public IEnumerable<CategoryDto> GetAllCategories()
        {
            return _categorySpecRepository.GetAllCategories();
        }

        public List<int> GetBoundAttributeIds(int categoryId)
        {
            return _categorySpecRepository.GetBoundAttributeIds(categoryId);
        }

        public List<BoundSpecItem> GetBoundSpecItems(int categoryId)
        {
            return _categorySpecRepository.GetBoundSpecItems(categoryId);
        }

        public void ToggleBinding(int categoryId, int specId, bool isBound)
        {
            _categorySpecRepository.ToggleBinding(categoryId, specId, isBound);
        }

        public void ToggleFilterable(int categoryId, int specId, bool isFilterable)
        {
            _categorySpecRepository.ToggleFilterable(categoryId, specId, isFilterable);
        }

        public List<object> GetAllBindingPairs()
        {
            return _categorySpecRepository.GetAllBindingPairs();
        }

        private static bool NeedsOptions(string inputType)
        {
            return inputType == "select"
                || inputType == "checkbox"
                || inputType == "radio";
        }
    }
}
