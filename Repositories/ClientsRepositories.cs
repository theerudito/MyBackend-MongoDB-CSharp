using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MyBackend_MongoDB_CSharp.Data;
using MyBackend_MongoDB_CSharp.Helpers;
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

            usersCollections = database.GetCollection<Clients>(
                databaseSettings.Value.MongoDB_Collection_One
            );
        }

        public async Task CreateClient(Clients client)
        {
            var newAvatar = CreateAvatar.avatar;

            var newClient = new Clients
            {
                Name = client.Name,
                Email = client.Email,
                Phone = client.Phone,
                Message = client.Message,
                Pic = newAvatar + client.Name
            };

            await usersCollections.InsertOneAsync(newClient);
        }

        public async Task DeleteClient(string id)
        {
            await usersCollections.DeleteOneAsync(client => client.Id == id);
        }

        public async Task<List<Clients>> GetAllClients()
        {
            return await usersCollections.Find(client => true).ToListAsync();
        }

        public async Task<Clients> GetClientById(string id)
        {
            return await usersCollections.Find(client => client.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Clients> UpdateClient(string id, Clients client)
        {
            var user = await usersCollections
                .Find<Clients>(client => client.Id == id)
                .FirstOrDefaultAsync();

            var newAvatar = CreateAvatar.avatar;

            await usersCollections.UpdateOneAsync(
                user => user.Id == id,
                Builders<Clients>.Update
                    .Set(client => client.Name, client.Name)
                    .Set(client => client.Email, client.Email)
                    .Set(client => client.Phone, client.Phone)
                    .Set(client => client.Message, client.Message)
                    .Set(client => client.Pic, newAvatar + client.Name)
            );

            return client;
        }
    }
}
