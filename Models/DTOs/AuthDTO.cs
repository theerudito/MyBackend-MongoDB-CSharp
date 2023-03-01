using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyBackend_MongoDB_CSharp.Models.DTOs
{
  public class AuthDTO
  {
    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;
    [BsonElement("password")]
    public string Password { get; set; } = string.Empty;
  }
}