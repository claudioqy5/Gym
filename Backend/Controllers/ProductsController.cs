using GymSystemAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GymSystemAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IMongoCollection<Product> _collection;

        public ProductsController(IMongoClient mongoClient, IOptions<GymDatabaseSettings> settings)
        {
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _collection = database.GetCollection<Product>("Products");
        }

        [HttpGet]
        public async Task<List<Product>> Get() =>
            await _collection.Find(_ => true).ToListAsync();

        [HttpPost]
        public async Task<IActionResult> Post(Product newProduct)
        {
            newProduct.CreatedAt = DateTime.UtcNow;
            await _collection.InsertOneAsync(newProduct);
            return CreatedAtAction(nameof(Get), new { id = newProduct.Id }, newProduct);
        }
    }
}
