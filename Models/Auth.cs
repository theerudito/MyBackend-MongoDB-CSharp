using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyBackend_MongoDB_CSharp.Models
{
    public class Auth
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        // [BsonElement("id")]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("username")]
        public string Username { get; set; } = string.Empty;

        [BsonElement("password")]
        public string Password { get; set; } = string.Empty;

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("pic")]
        public string Pic { get; set; } = string.Empty;
    }
}
