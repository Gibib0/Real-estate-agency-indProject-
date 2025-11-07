using RealEstateAgency.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace RealEstateAgency.Services
{
    public class MatchingService
    {
        private readonly PropertyService _propertyService;
        private readonly SavedSearchService _savedSearchService;

        public MatchingService(PropertyService propertyService, SavedSearchService savedSearchService)
        {
            _propertyService = propertyService;
            _savedSearchService = savedSearchService;
        }

        public List<Property> GetMatchingPropertiesForClient(Guid clientId)
        {
            var clientSearches = _savedSearchService.GetSavedSearches(clientId);
            if (!clientSearches.Any())
            {
                return new List<Property>();
            }

            var matchedProperties = new Dictionary<Guid, Property>();

            foreach (var search in clientSearches)
            {
                var properties = _propertyService.GetPropertiesByFilters(search.Filter);

                foreach (var prop in properties)
                {
                    if (prop.CurrentStatus == Property.Status.OnSale || prop.CurrentStatus == Property.Status.Rent)
                    {
                        matchedProperties[prop.Id] = prop;
                    }
                }
            }

            return matchedProperties.Values.ToList();
        }
    }
}