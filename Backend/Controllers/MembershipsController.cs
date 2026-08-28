using GymSystemAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GymSystemAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MembershipsController : ControllerBase
    {
        private readonly IMongoCollection<Membership> _collection;

        public MembershipsController(IMongoClient mongoClient, IOptions<GymDatabaseSettings> settings)
        {
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _collection = database.GetCollection<Membership>("Memberships");
        }

        [HttpGet]
        public async Task<List<Membership>> Get() =>
            await _collection.Find(_ => true).ToListAsync();

        [HttpPost]
        public async Task<IActionResult> Post(Membership newMembership)
        {
            await _collection.InsertOneAsync(newMembership);
            return CreatedAtAction(nameof(Get), new { id = newMembership.Id }, newMembership);
        }
    }
}
