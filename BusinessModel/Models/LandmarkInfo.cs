using System;

namespace BusinessLogic.Models
{
    public class LandmarkInfo
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public int TravelTimeMinutes { get; set; }
        public Guid PropertyId { get; set; }
        public Property Property { get; set; }
        public LandmarkInfo() { }
        public LandmarkInfo(string name, int travelTimeMinutes)
        {
            Name = name;
            TravelTimeMinutes = travelTimeMinutes;
        }
    }
}