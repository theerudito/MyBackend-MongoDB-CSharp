using MongoDB.Driver;
using MyBackend_MongoDB_CSharp.Models;

namespace MyBackend_MongoDB_CSharp.Data
{
  public class MongoDbSettings
  {
    public MongoClient client;
    public IMongoDatabase database;
    public IMongoCollection<Clients> collectionClient;
    public IMongoCollection<Auth> collectionAuth;

    public MongoDbSettings()
    {
      client = new MongoClient("mongodb://localhost:27017");
      database = client.GetDatabase("MyBackend");
      collectionClient = database.GetCollection<Clients>("Clients");
      collectionAuth = database.GetCollection<Auth>("Auth");
    }
  }
}

///mongodb://root:erudito@localhost:27017/?authMechanism=DEFAULT