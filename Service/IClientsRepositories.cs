using MyBackend_MongoDB_CSharp.Models;

namespace MyBackend_MongoDB_CSharp.Service
{
  public interface IClientsRepositories
  {
    Task<List<Clients>> GetAllClients();
    Task<Clients> GetClientById(string id);
    Task CreateClient(Clients client);
    Task<Clients> UpdateClient(string id, Clients client);
    Task DeleteClient(string id);
  }
}