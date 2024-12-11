using MongoDB.Driver;

namespace DataValidation.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDb"));
            _database = client.GetDatabase("DatabaseName");

            // Create unique index on RuleName
            var indexKeysDefinition = Builders<DocumentModel>.IndexKeys.Ascending(rule => rule.Payload.RuleName);
            var indexOptions = new CreateIndexOptions { Unique = true };
            var indexModel = new CreateIndexModel<DocumentModel>(indexKeysDefinition, indexOptions);
            ValidationRules.Indexes.CreateOne(indexModel);
        }

        public IMongoCollection<DocumentModel> ValidationRules => _database.GetCollection<DocumentModel>("ValidationRules");
    }
}
