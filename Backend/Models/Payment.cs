using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace GymSystemAPI.Models
{
    public class Payment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } = null!;

        [BsonRepresentation(BsonType.ObjectId)]
        public string MembershipId { get; set; } = null!;

        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string PaymentMethod { get; set; } = "cash"; // cash, transfer
        public string Status { get; set; } = "completed"; // pending, completed
        public string Notes { get; set; } = string.Empty;
    }
}
