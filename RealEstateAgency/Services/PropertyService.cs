using RealEstateAgency.Models;
using System.Collections.Generic;

namespace RealEstateAgency.Services
{
    public class PropertyService
    {
        private List<Property> _properties;
        private JSONDataService _jsonDataService;
        private string _filePath = @"Data/propertiesData.json";

        public PropertyService()
        {
            _jsonDataService = new JSONDataService();
            _properties = _jsonDataService.LoadFromFile<Property>(_filePath);
        }
        public void AddProperty(Property property)
        {
            if (!_properties.Any(p => p.Address.Equals(property.Address, StringComparison.OrdinalIgnoreCase)))
            {
                _properties.Add(property);
                _jsonDataService.SaveToFile(_properties, _filePath);
            }
        }

        public List<Property> GetClients()
        {
            return _properties;
        }
    }
}
