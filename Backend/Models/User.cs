using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace GymSystemAPI.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Role { get; set; } = "member"; // member, admin, trainer

        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Status { get; set; } = "active"; // active, inactive
        
        public string PasswordHash { get; set; } = null!; // Almacenará la contraseña cifrada

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
