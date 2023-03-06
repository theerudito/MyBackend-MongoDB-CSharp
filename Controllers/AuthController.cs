using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using MyBackend_MongoDB_CSharp.Data;
using MyBackend_MongoDB_CSharp.Models;
using MyBackend_MongoDB_CSharp.Models.DTOs;


namespace MyBackend_MongoDB_CSharp.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class AuthController : Controller
  {

    private readonly IConfiguration config;
    private readonly IMongoCollection<Auth> authCollections;
    public AuthController(IConfiguration config, IOptions<DataBaseSetting> databaseSettings)
    {
      this.config = config;
      var client = new MongoClient(databaseSettings.Value.ConnectionString);
      var database = client.GetDatabase(databaseSettings.Value.MongoDB_Name);
      authCollections = database.GetCollection<Auth>(databaseSettings.Value.MongoDB_Collection_Two);
    }

    [HttpGet]
    public async Task<ActionResult> Get()
    {
      var result = await authCollections.Find(auth => true).ToListAsync();
      return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] AuthDTO userDTO)
    {
      // search if email exists
      var filter = Builders<Auth>.Filter.Eq("email", userDTO.Email);

      var result = await authCollections.Find(filter).FirstOrDefaultAsync();

      if (result == null)
      {
        return BadRequest("The user does not exist");
      }

      //if (!BCrypt.Net.BCrypt.Verify(userDTO.Password, result.Password))
      // compare password
      if (!BCrypt.Net.BCrypt.Verify(userDTO.Password, result.Password))
      {
        return BadRequest("Password or email is incorrect");
      }

      // generate token
      var tokenHandler = new JwtSecurityTokenHandler();

      var key = Encoding.ASCII.GetBytes(config["Jwt:Token"]);

      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, result.Id)
        }),
        Expires = DateTime.UtcNow.AddDays(7),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
      };

      var token = tokenHandler.CreateToken(tokenDescriptor);


      return Ok(new { token = tokenHandler.WriteToken(token) });
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] Auth auth)
    {
      var filter = Builders<Auth>.Filter.Eq("email", auth.Email);

      var result = await authCollections.Find(filter).FirstOrDefaultAsync();

      if (result != null)
      {
        return BadRequest(new { message = "Email already exists" });
      }

      // encriptar password
      auth.Password = BCrypt.Net.BCrypt.HashPassword(auth.Password);

      // save user

      await authCollections.InsertOneAsync(auth);

      return Ok(new { message = "User created" });
    }
  }
}
