using BusinessLogic.Models;
using System;
using System.Collections.Generic;

namespace BusinessLogic.Services
{
    public interface ISavedSearchService
    {
        void AddSavedSearch(Guid clientId, PropertyFilter filter, string description);
        List<SaveSearch> GetSavedSearches(Guid clientId);
    }
}