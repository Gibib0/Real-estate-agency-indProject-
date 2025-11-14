using BusinessLogic.Models;
using BusinessLogic.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Integration.SqliteImplementations
{
    public class SqlitePropertyService : IPropertyService
    {
        private readonly RealEstateDbContext _context;

        public SqlitePropertyService(RealEstateDbContext context)
        {
            _context = context;
        }

        public void AddProperty(Property property)
        {
            string upperAddress = property.Address.ToUpper();
            if (!_context.Properties.Any(p => p.Address.ToUpper() == upperAddress))
            {
                if (property.Landmarks == null) property.Landmarks = new List<LandmarkInfo>();
                if (property.StatusHistory == null) property.StatusHistory = new List<StatusChange>();

                _context.Properties.Add(property);
                _context.SaveChanges();
            }
        }

        public List<Property> GetProperties()
        {
            return _context.Properties
                .Include(p => p.Landmarks)
                .Include(p => p.StatusHistory)
                .ToList();
        }

        public bool UpdateProperty(Property updatedProperty)
        {
            _context.Properties.Update(updatedProperty);
            _context.SaveChanges();
            return true;
        }

        public bool DeleteProperty(Guid id)
        {
            var property = _context.Properties.Find(id);
            if (property == null) return false;

            _context.Properties.Remove(property);
            _context.SaveChanges();
            return true;
        }

        public bool ChangePropertyStatus(Guid propertyId, Property.Status newStatus, string changeBy = null)
        {
            var property = _context.Properties.Find(propertyId);
            if (property == null || property.CurrentStatus == newStatus) return false;

            var change = new StatusChange
            {
                Id = Guid.NewGuid(),
                Time = DateTime.Now,
                OldStatus = property.CurrentStatus,
                NewStatus = newStatus,
                ChangeBy = changeBy,
                PropertyId = property.Id
            };

            _context.StatusChanges.Add(change);
            property.CurrentStatus = newStatus;

            _context.SaveChanges();
            return true;
        }

        public List<StatusChange> GetStatusHistory(Guid propertyId)
        {
            return _context.StatusChanges
                .Where(h => h.PropertyId == propertyId)
                .OrderBy(h => h.Time)
                .ToList();
        }

        public IEnumerable<Property> GetPropertiesByFilters(PropertyFilter filter)
        {
            IQueryable<Property> query = _context.Properties
                .Include(p => p.Landmarks)
                .AsNoTracking();

            if (filter == null) return query.ToList();

            if (!string.IsNullOrWhiteSpace(filter.PropertyType))
            {
                string upperType = filter.PropertyType.ToUpper();
                query = query.Where(p => !string.IsNullOrWhiteSpace(p.PropertyType) && p.PropertyType.ToUpper() == upperType);
            }

            if (!string.IsNullOrWhiteSpace(filter.District))
            {
                string upperDistrict = filter.District.ToUpper();
                query = query.Where(p => !string.IsNullOrWhiteSpace(p.District) && p.District.ToUpper() == upperDistrict);
            }

            if (!string.IsNullOrWhiteSpace(filter.Landmark))
            {
                string upperLandmark = filter.Landmark.ToUpper();
                query = query.Where(p => p.Landmarks.Any(lm => lm.Name.ToUpper() == upperLandmark &&
                    (!filter.MaxTravelTime.HasValue || lm.TravelTimeMinutes <= filter.MaxTravelTime.Value)));
            }

            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.Price >= filter.MinPrice.Value);
            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);
            if (filter.MinArea.HasValue)
                query = query.Where(p => p.Square >= filter.MinArea.Value);
            if (filter.MaxArea.HasValue)
                query = query.Where(p => p.Square <= filter.MaxArea.Value);
            if (filter.MinRooms.HasValue)
                query = query.Where(p => p.Rooms >= filter.MinRooms.Value);
            if (filter.MaxRooms.HasValue)
                query = query.Where(p => p.Rooms <= filter.MaxRooms.Value);

            return query.ToList();
        }
    }
}