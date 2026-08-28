using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace GymSystemAPI.Models
{
    public class GymClass
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Name { get; set; } = null!; // Spinning, Pilates, etc.
        public string Instructor { get; set; } = null!;
        public int MaxCapacity { get; set; }
        
        public DateTime StartTime { get; set; }
        public int DurationMinutes { get; set; }

        // Store reservations as a list of User Ids or names directly in the document for simplicity
        public List<string> ReservedUserIds { get; set; } = new List<string>();
    }
}
