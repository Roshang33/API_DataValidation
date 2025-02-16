using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataValidation
{
    public class ValidationRule
    {
        [BsonElement("RuleName")]
        public string RuleName { get; set; }

        [BsonElement("RuleDescription")]
        public RuleDescription RuleDescription { get; set; }
        // Add other properties as needed
    }

    public class RuleDescription
    {
        public string Description { get; set; }

        [BsonElement("Details")]
        public BsonDocument Details { get; set; }  // ✅ Stores JSON as BsonDocument
    }


}
