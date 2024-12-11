using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataValidation
{
    public class ValidationRule
    {
        [BsonElement("RuleName")]
        public string RuleName { get; set; }

        [BsonElement("RuleDescription")]
        public BsonDocument RuleDescription { get; set; } = new BsonDocument();
        // Add other properties as needed
    }
}
