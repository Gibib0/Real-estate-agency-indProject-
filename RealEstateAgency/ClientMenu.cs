using RealEstateAgency.Models;
using RealEstateAgency.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RealEstateAgency
{
    public class ClientMenu
    {
        private readonly Client _currentClient;
        private readonly PropertyService _propertyService;
        private readonly DealService _dealService;
        private readonly SavedSearchService _savedSearchService;

        public ClientMenu(Client currentClient, PropertyService propertyService, DealService dealService, SavedSearchService savedSearchService)
        {
            _currentClient = currentClient;
            _propertyService = propertyService;
            _dealService = dealService;
            _savedSearchService = savedSearchService;
        }

        public void Run()
        {
            bool isRunning = true;
            while (isRunning)
            {
                Console.Clear();
                Console.WriteLine($"=== Client ({_currentClient.FullName}) menu ===");
                Console.WriteLine("1. Property search and save");
                Console.WriteLine("2. Get all saved seraches");
                Console.WriteLine("3. Get my deal history");
                Console.WriteLine("0. Back");

                string choice = ConsoleHelpers.GetString("Your choice:").ToUpper();
                switch (choice)
                {
                    case "1":
                        SearchProperties();
                        break;
                    case "2":
                        ViewMySavedSearches();
                        break;
                    case "3":
                        ViewMyDeals();
                        break;
                    case "0":
                        isRunning = false;
                        break;
                    default:
                        Console.WriteLine("Wrong choice.");
                        break;
                }
                if (isRunning) ConsoleHelpers.PressAnyKeyToContinue();
            }
        }

        private void ViewMyDeals()
        {
            Console.WriteLine($"--- ({_currentClient.FullName}) deal history ---");
            var myDeals = _dealService.GetDeals().Where(d => d.Client.Id == _currentClient.Id).ToList();
            if (!myDeals.Any())
            {
                Console.WriteLine("There are not any deals.");
                return;
            }
            foreach (var d in myDeals)
            {
                Console.WriteLine($" - [{d.Date.ToShortDateString()}] {d.Type} - {d.Property.Address}");
                Console.WriteLine($"   Agent: {d.Agent.FullName}, Price: ${d.FinalPrice}");
            }
        }

        private void ViewMySavedSearches()
        {
            Console.WriteLine($"--- ({_currentClient.FullName}) saved searches ---");
            var searches = _savedSearchService.GetSavedSearches(_currentClient.Id);
            if (!searches.Any())
            {
                Console.WriteLine("There are not any saved searches.");
                return;
            }
            foreach (var s in searches)
            {
                Console.WriteLine($" - [{s.DateSaved.ToShortDateString()}] {s.Description}");
                Console.WriteLine($"   Filter: (Type: {s.Filter.PropertyType}, Price: {s.Filter.MinPrice}-{s.Filter.MaxPrice}, District: {s.Filter.District})");
            }
        }

        private void SearchProperties()
        {
            Console.WriteLine("--- Property search ---");
            Console.WriteLine("(Press Enter for skip)");

            var filter = new PropertyFilter
            {
                PropertyType = ConsoleHelpers.GetString("Type ('Flat', 'House' etc.):"),
                District = ConsoleHelpers.GetString("District:"),
                Landmark = ConsoleHelpers.GetString("Landmark:"),
                MinPrice = ConsoleHelpers.GetNullableDecimal("Min. price ($):"),
                MaxPrice = ConsoleHelpers.GetNullableDecimal("Max. price ($):"),
                MinArea = ConsoleHelpers.GetNullableDecimal("Min. area (м2):"),
                MaxArea = ConsoleHelpers.GetNullableDecimal("Max. area (м2):")
            };
            var minRoomsStr = ConsoleHelpers.GetString("Min. rooms:");
            filter.MinRooms = string.IsNullOrWhiteSpace(minRoomsStr) ? null : (int?)int.Parse(minRoomsStr);
            var maxRoomsStr = ConsoleHelpers.GetString("Max. rooms:");
            filter.MaxRooms = string.IsNullOrWhiteSpace(maxRoomsStr) ? null : (int?)int.Parse(maxRoomsStr);

            var results = _propertyService.GetPropertiesByFilters(filter).ToList();

            Console.WriteLine($"\n--- Find: {results.Count} ---");
            foreach (var p in results)
            {
                var landmarksString = string.Join(", ", (p.Landmarks ?? Enumerable.Empty<LandmarkInfo>()).Select(lm => $"{lm.Name} ({lm.TravelTimeMinutes} min)"));
                Console.WriteLine($" - {p.PropertyType} on {p.Address} ({p.Square} m2) for ${p.Price}. Status: {p.CurrentStatus}");
                Console.WriteLine($"   Landmarks: [{landmarksString}]");
            }

            if (results.Any())
            {
                string save = ConsoleHelpers.GetString("\nSave this search? (Y/N):").ToUpper();
                if (save == "Y")
                {
                    string desc = ConsoleHelpers.GetString("Enter description for this search:");
                    _savedSearchService.AddSavedSearch(_currentClient.Id, filter, desc);
                    Console.WriteLine("Search saved.");
                }
            }
        }
    }
}