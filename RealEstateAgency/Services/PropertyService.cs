using RealEstateAgency.Models;
using System.Collections.Generic;
using System.Linq;

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
            if (!_properties.Any(p =>
            p.Address.Equals(property.Address, StringComparison.OrdinalIgnoreCase) &&
            p.PropertyType.Equals(property.PropertyType, StringComparison.OrdinalIgnoreCase) &&
            p.Square == property.Square))
            {
                _properties.Add(property);
                _jsonDataService.SaveToFile(_properties, _filePath);
            }
        }

        public List<Property> GetProperties()
        {
            return _properties;
        }

        public bool UpdateProperty(Property updatedProperty)
        {
            int index = _properties.FindIndex(p => p.Id == updatedProperty.Id);

            if (index != -1)
            {
                _properties[index] = updatedProperty;
                _jsonDataService.SaveToFile(_properties, _filePath);
                return true;
            }

            return false;
        }

        public bool DeleteProperty(Guid id)
        {
            var propertyToRemove = _properties.FirstOrDefault(p => p.Id == id);

            if (propertyToRemove != null)
            {
                _properties.Remove(propertyToRemove);
                _jsonDataService.SaveToFile(_properties, _filePath);
                return true;
            }

            return false;
        }

        public bool ChangePropertyStatus(Guid propertyId, Property.Status newStatus, string changeBy = null)
        {
            var property = _properties.FirstOrDefault(p => p.Id.Equals(propertyId));
            if (property == null)
            {
                return false;
            }

            if (property.CurrentStatus == newStatus)
            {
                return false;
            }

            var change = new StatusChange
            {
                Time = DateTime.Now,
                OldStatus = property.CurrentStatus,
                NewStatus = newStatus,
                ChangeBy = changeBy,
            };

            property.StatusHistory.Add(change);
            property.CurrentStatus = newStatus;

            _jsonDataService.SaveToFile(_properties, _filePath);
            return true;
        }

        public List<StatusChange> GetStatusHistory(Guid propertyId)
        {
            var property = _properties.FirstOrDefault(p => p.Id.Equals(propertyId));
            return property?.StatusHistory ?? new List<StatusChange>();
        }

        public IEnumerable<Property> GetPropertiesByFilters(PropertyFilter filter)
        {
            IEnumerable<Property> query = _properties;

            if (filter == null)
            {
                return query;
            }

            if (!string.IsNullOrWhiteSpace(filter.PropertyType))
            {
                query = query.Where(p => !string.IsNullOrWhiteSpace(p.PropertyType) && p.PropertyType.Equals(filter.PropertyType, StringComparison.OrdinalIgnoreCase));
            }

            if (filter.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);
            }

            if (filter.MinArea.HasValue)
            {
                query = query.Where(p => p.Square >= filter.MinArea.Value);
            }

            if (filter.MaxArea.HasValue)
            {
                query = query.Where(p => p.Square <= filter.MaxArea.Value);
            }

            if (filter.MinRooms.HasValue)
            {
                query = query.Where(p => p.Rooms >= filter.MinRooms.Value);
            }

            if (filter.MaxRooms.HasValue)
            {
                query = query.Where(p => p.Rooms <= filter.MaxRooms.Value);
            }

            return query;
        }
    }
}
