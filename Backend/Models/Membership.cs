using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GymSystemAPI.Models
{
    public class Membership
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Name { get; set; } = null!; // Ej. Mensualidad Básico
        public decimal Price { get; set; }
        public int DurationInDays { get; set; } // 30, 180, 365
        public bool IsActive { get; set; } = true;
    }
}
