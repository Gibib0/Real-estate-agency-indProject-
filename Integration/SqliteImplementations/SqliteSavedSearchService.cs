using BusinessLogic.Models;
using BusinessLogic.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Integration.SqliteImplementations
{
    public class SqliteSavedSearchService : ISavedSearchService
    {
        private readonly RealEstateDbContext _context;

        public SqliteSavedSearchService(RealEstateDbContext context)
        {
            _context = context;
        }

        public void AddSavedSearch(Guid clientId, PropertyFilter filter, string description)
        {
            var newSearch = new SaveSearch
            {
                ClientId = clientId,
                Filter = filter,
                Description = description
            };

            var clientSearches = _context.SavedSearches.Where(s => s.ClientId == clientId).ToList();

            if (clientSearches.Count >= 10)
            {
                var oldest = clientSearches.OrderBy(s => s.DateSaved).First();
                _context.SavedSearches.Remove(oldest);
            }

            _context.SavedSearches.Add(newSearch);
            _context.SaveChanges();
        }

        public List<SaveSearch> GetSavedSearches(Guid clientId)
        {
            return _context.SavedSearches
                .Where(s => s.ClientId == clientId)
                .OrderByDescending(s => s.DateSaved)
                .ToList();
        }
    }
}