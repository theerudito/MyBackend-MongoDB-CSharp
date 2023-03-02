using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MyBackend_MongoDB_CSharp.Data;
using MyBackend_MongoDB_CSharp.Models;
using MyBackend_MongoDB_CSharp.Service;

namespace MyBackend_MongoDB_CSharp.Repositories
{
  public class ClientsRepositories : IClientsRepositories
  {
    private readonly IMongoCollection<Clients> usersCollections;


    public ClientsRepositories(IOptions<DataBaseSetting> databaseSettings)
    {
      var client = new MongoClient(databaseSettings.Value.ConnectionString);
      var database = client.GetDatabase(databaseSettings.Value.MongoDB_Name);
      usersCollections = database.GetCollection<Clients>(databaseSettings.Value.MongoDB_Collection_One);
    }

    public async Task CreateClient(Clients client)
    {
      await usersCollections.InsertOneAsync(client);
    }

    public async Task DeleteClient(string id)
    {
      await usersCollections.DeleteOneAsync(client => client.Id == id);
    }

    public async Task<IEnumerable<Clients>> GetAllClients()
    {
      return await usersCollections.Find(client => true).ToListAsync();
    }

    public async Task<Clients> GetClientById(string id)
    {
      return await usersCollections.Find(client => client.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Clients> UpdateClient(string id, Clients client)
    {
      await usersCollections.ReplaceOneAsync(client => client.Id == id, client);
      return client;
    }
  }
}