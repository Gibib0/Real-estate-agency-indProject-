using System;

namespace BusinessLogic.Models
{
    public class LandmarkInfo
    {
        public string Name { get; set; }

        public int TravelTimeMinutes { get; set; }

        public LandmarkInfo() { }

        public LandmarkInfo(string name, int travelTimeMinutes)
        {
            Name = name;
            TravelTimeMinutes = travelTimeMinutes;
        }
    }
}