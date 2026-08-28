using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace GymSystemAPI.Models
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Name { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string Barcode { get; set; } = string.Empty;
        
        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int StockQuantity { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
