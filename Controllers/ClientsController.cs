using Microsoft.AspNetCore.Mvc;
using MyBackend_MongoDB_CSharp.Models;
using MyBackend_MongoDB_CSharp.Repositories;
using MyBackend_MongoDB_CSharp.Service;

namespace MyBackend_MongoDB_CSharp.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class ClientsController : Controller
  {

    IClientsRepositories clientsRepositories = new ClientsRepositories();

    [HttpGet]
    public async Task<ActionResult<List<Clients>>> Get()
    {
      var clients = await clientsRepositories.GetAllClients();
      return Ok(clients);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Clients>> Get(string id)
    {
      var client = await clientsRepositories.GetClientById(id);
      return Ok(client);
    }

    [HttpPost]
    public async Task<ActionResult<Clients>> Post([FromBody] Clients client)
    {
      await clientsRepositories.CreateClient(client);
      return Ok(new { message = "Client created" });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Clients>> Put(Clients client, string id)
    {
      await clientsRepositories.UpdateClient(id, client);
      return Ok(new { message = "Client updated" });
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
      await clientsRepositories.DeleteClient(id);
      return Ok(new { message = "Client deleted" });
    }
  }
}