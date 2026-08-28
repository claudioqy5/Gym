using GymSystemAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Linq;

namespace GymSystemAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassesController : ControllerBase
    {
        private readonly IMongoCollection<GymClass> _collection;

        public ClassesController(IMongoClient mongoClient, IOptions<GymDatabaseSettings> settings)
        {
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _collection = database.GetCollection<GymClass>("Classes");
        }

        [HttpGet]
        public async Task<List<GymClass>> Get() =>
            await _collection.Find(_ => true).SortBy(c => c.StartTime).ToListAsync();

        [HttpPost]
        public async Task<IActionResult> Post(GymClass newClass)
        {
            if (newClass.ReservedUserIds == null)
            {
                newClass.ReservedUserIds = new List<string>();
            }
            await _collection.InsertOneAsync(newClass);
            return CreatedAtAction(nameof(Get), new { id = newClass.Id }, newClass);
        }

        [HttpPost("{id}/reserve")]
        public async Task<IActionResult> ReserveSpot(string id, [FromBody] string userId)
        {
            var gymClass = await _collection.Find(c => c.Id == id).FirstOrDefaultAsync();
            if (gymClass == null) return NotFound("Clase no encontrada.");
            
            if (gymClass.ReservedUserIds.Count >= gymClass.MaxCapacity)
            {
                return BadRequest("La clase ya está llena.");
            }

            if (gymClass.ReservedUserIds.Contains(userId))
            {
                return BadRequest("El usuario ya está reservado en esta clase.");
            }

            var update = Builders<GymClass>.Update.Push(c => c.ReservedUserIds, userId);
            await _collection.UpdateOneAsync(c => c.Id == id, update);

            return Ok(new { message = "Reserva exitosa" });
        }
    }
}
