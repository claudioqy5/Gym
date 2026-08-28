using GymSystemAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GymSystemAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMongoCollection<User> _usersCollection;

        public UsersController(IMongoClient mongoClient, IOptions<GymDatabaseSettings> settings)
        {
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _usersCollection = database.GetCollection<User>("Users");
        }

        [HttpGet]
        public async Task<List<User>> Get() =>
            await _usersCollection.Find(_ => true).ToListAsync();

        [HttpPost]
        public async Task<IActionResult> Post(User newUser)
        {
            newUser.CreatedAt = DateTime.UtcNow;
            
            // Si viene una contraseña en texto plano en el campo PasswordHash, la hasheamos
            if (!string.IsNullOrEmpty(newUser.PasswordHash))
            {
                newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUser.PasswordHash);
            }

            await _usersCollection.InsertOneAsync(newUser);
            return CreatedAtAction(nameof(Get), new { id = newUser.Id }, newUser);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, User updatedUser)
        {
            var user = await _usersCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
            if (user == null) return NotFound();

            updatedUser.Id = user.Id;
            updatedUser.PasswordHash = user.PasswordHash; // No cambiar contraseña por aquí
            updatedUser.CreatedAt = user.CreatedAt;

            await _usersCollection.ReplaceOneAsync(u => u.Id == id, updatedUser);
            return NoContent();
        }

        [HttpPut("{id}/suspend")]
        public async Task<IActionResult> Suspend(string id)
        {
            var update = Builders<User>.Update.Set(u => u.Status, "inactive");
            var result = await _usersCollection.UpdateOneAsync(u => u.Id == id, update);
            if (result.MatchedCount == 0) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _usersCollection.DeleteOneAsync(u => u.Id == id);
            if (result.DeletedCount == 0) return NotFound();
            return NoContent();
        }
    }
}
