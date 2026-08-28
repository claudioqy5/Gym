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
    }
}
