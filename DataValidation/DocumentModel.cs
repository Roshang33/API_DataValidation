using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DataValidation
{
    public class DocumentModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("Payload")]
        public ValidationRule Payload { get; set; } = new ValidationRule();
    }
}
