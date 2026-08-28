using GymSystemAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;

namespace GymSystemAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IMongoCollection<Payment> _collection;

        public PaymentsController(IMongoClient mongoClient, IOptions<GymDatabaseSettings> settings)
        {
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _collection = database.GetCollection<Payment>("Payments");
        }

        [HttpGet]
        public async Task<List<Payment>> Get() =>
            await _collection.Find(_ => true).SortByDescending(p => p.PaymentDate).ToListAsync();

        [HttpPost]
        public async Task<IActionResult> Post(Payment newPayment)
        {
            newPayment.PaymentDate = DateTime.UtcNow;
            await _collection.InsertOneAsync(newPayment);
            return CreatedAtAction(nameof(Get), new { id = newPayment.Id }, newPayment);
        }
    }
}
