using Microsoft.AspNetCore.Mvc;
using MyBackend_MongoDB_CSharp.Models;
using MyBackend_MongoDB_CSharp.Service;

namespace MyBackend_MongoDB_CSharp.Controllers
{
  [Route("api/v1/[controller]")]
  [ApiController]
  public class ClientsController : Controller
  {

    private readonly IClientsRepositories _clientsRepositories;

    public ClientsController(IClientsRepositories clientsRepositories)
    {
      _clientsRepositories = clientsRepositories;
    }

    [HttpGet]
    public async Task<ActionResult<List<Clients>>> GetAll()
    {
      var clients = await _clientsRepositories.GetAllClients();
      return Ok(clients);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Clients>> Get(string id)
    {
      var client = await _clientsRepositories.GetClientById(id);
      return Ok(client);
    }

    [HttpPost]
    public async Task<ActionResult<Clients>> Post([FromBody] Clients client)
    {
      await _clientsRepositories.CreateClient(client);
      return Ok(new { message = "Client created" });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Clients>> Put(Clients client, string id)
    {
      await _clientsRepositories.UpdateClient(id, client);
      return Ok(new { message = "Client updated" });
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
      await _clientsRepositories.DeleteClient(id);
      return Ok(new { message = "Client deleted" });
    }
  }
}