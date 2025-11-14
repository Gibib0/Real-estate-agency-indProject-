using BusinessLogic.Models;
using BusinessLogic.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Presentation
{
    public class ClientMenu
    {
        private readonly Client _currentClient;
        private readonly IPropertyService _propertyService;
        private readonly IDealService _dealService;
        private readonly ISavedSearchService _savedSearchService;
        private readonly MatchingService _matchingService;

        public ClientMenu(Client currentClient, IPropertyService propertyService, IDealService dealService, ISavedSearchService savedSearchService, MatchingService matchingService)
        {
            _currentClient = currentClient;
            _propertyService = propertyService;
            _dealService = dealService;
            _savedSearchService = savedSearchService;
            _matchingService = matchingService;
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
                Console.WriteLine("4. Get matched properties for me");
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
                    case "4":
                        ViewMyMatchedProperties();
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
            var myDeals = _dealService.GetDeals().Where(d => d.ClientId == _currentClient.Id).ToList();
            if (!myDeals.Any())
            {
                Console.WriteLine("There are not any deals.");
                return;
            }
            foreach (var d in myDeals)
            {
                string agentName = d.Agent?.FullName ?? "N/A";
                string propertyAddress = d.Property?.Address ?? "N/A";

                Console.WriteLine($" - [{d.Date.ToShortDateString()}] {d.Type} - {propertyAddress}");
                Console.WriteLine($"   Agent: {agentName}, Total Price Paid: ${d.FinalPrice:F2}");
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

        private void ViewMyMatchedProperties()
        {
            Console.WriteLine("--- Matched properties for you ---");

            var matchedProperties = _matchingService.GetMatchingPropertiesForClient(_currentClient.Id);

            if (!matchedProperties.Any())
            {
                Console.WriteLine("There are not any matched properties for your saved searches.");
                return;
            }

            Console.WriteLine($"\n--- Found {matchedProperties.Count} properties ---");
            foreach (var p in matchedProperties)
            {
                var landmarksString = string.Join(", ", (p.Landmarks ?? Enumerable.Empty<LandmarkInfo>()).Select(lm => $"{lm.Name} ({lm.TravelTimeMinutes} min)"));
                Console.WriteLine($" - {p.PropertyType} on {p.Address} (${p.Price}) [Status: {p.CurrentStatus}]");
                Console.WriteLine($"   Landmarks: [{landmarksString}]");
            }
        }
    }
}