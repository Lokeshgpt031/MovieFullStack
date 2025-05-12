// No service for type 'MongoDB.Driver.IMongoClient' has been registered.

using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MovieApi.Models; // Assuming your settings model is in MovieApi.Models

namespace MovieApi.Service
{
    public class MongoDbService
    {
        private readonly IMongoDatabase _database;

        public MongoDbService(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var client = new MongoClient(mongoDbSettings.Value.ConnectionString);
            _database = client.GetDatabase(mongoDbSettings.Value.DatabaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }
    }
}