using BusinessLogic.Models;
using System;
using System.Collections.Generic;

namespace BusinessLogic.Services
{
    public interface IPropertyService
    {
        void AddProperty(Property property);
        List<Property> GetProperties();
        bool UpdateProperty(Property updatedProperty);
        bool DeleteProperty(Guid id);
        bool ChangePropertyStatus(Guid propertyId, Property.Status newStatus, string changeBy = null);
        List<StatusChange> GetStatusHistory(Guid propertyId);
        IEnumerable<Property> GetPropertiesByFilters(PropertyFilter filter);
    }
}