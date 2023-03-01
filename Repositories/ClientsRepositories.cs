using MongoDB.Bson;
using MongoDB.Driver;
using MyBackend_MongoDB_CSharp.Data;
using MyBackend_MongoDB_CSharp.Models;
using MyBackend_MongoDB_CSharp.Service;

namespace MyBackend_MongoDB_CSharp.Repositories
{
  public class ClientsRepositories : IClientsRepositories
  {
    MongoDbSettings mongoDbSettings = new MongoDbSettings();

    public async Task CreateClient(Clients client)
    {
      await mongoDbSettings.collectionClient.InsertOneAsync(client);
    }

    public async Task DeleteClient(string id)
    {
      await mongoDbSettings.collectionClient.DeleteOneAsync(client => client.Id == id);
    }

    public async Task<IEnumerable<Clients>> GetAllClients()
    {
      return await mongoDbSettings.collectionClient.Find(new BsonDocument()).ToListAsync();
    }

    public async Task<Clients> GetClientById(string id)
    {
      return await mongoDbSettings.collectionClient.FindSync(client => client.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Clients> UpdateClient(string id, Clients client)
    {
      await mongoDbSettings.collectionClient.ReplaceOneAsync(client => client.Id == id, client);
      return client;
    }
  }
}