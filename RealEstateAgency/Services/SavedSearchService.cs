using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RealEstateAgency.Models;

namespace RealEstateAgency.Services
{
    public class SavedSearchService
    {
        private List<SaveSearch> _savedSearches;
        private JSONDataService _jsonDataService;
        private string _filePath = @"Data/savedSearchesData.json";

        public SavedSearchService()
        {
            _jsonDataService = new JSONDataService();
            _savedSearches = _jsonDataService.LoadFromFile<SaveSearch>(_filePath);
        }

        public void AddSavedSearch(Guid clientId, PropertyFilter filter, string description)
        {
            var newSearch = new SaveSearch {
                ClientId = clientId,
                Filter = filter,
                Description = description
            };

            var clientSearches = _savedSearches.Where(s => s.ClientId == clientId).ToList();

            if (clientSearches.Count >= 10)
            {
                var oldest = clientSearches.OrderBy(s => s.DateSaved).First();
                _savedSearches.Remove(oldest);
            }

            _savedSearches.Add(newSearch);
            _jsonDataService.SaveToFile(_savedSearches, _filePath);
        }

        public List<SaveSearch> GetSavedSearches(Guid clientId)
        {
            return _savedSearches
                .Where(s => s.ClientId == clientId)
                .OrderByDescending(s => s.DateSaved)
                .ToList();
        }
    }
}
