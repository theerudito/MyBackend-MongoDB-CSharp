using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using MyBackend_MongoDB_CSharp.Data;
using MyBackend_MongoDB_CSharp.Models;
using MyBackend_MongoDB_CSharp.Models.DTOs;
using MyBackend_MongoDB_CSharp.Repositories;
using MyBackend_MongoDB_CSharp.Service;

namespace MyBackend_MongoDB_CSharp.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class AuthController : Controller
  {

    private readonly IConfiguration config;
    MongoDbSettings mongoDbSettings = new MongoDbSettings();
    public AuthController(IConfiguration config)
    {
      this.config = config;
    }


    [HttpGet]
    public async Task<ActionResult> Get()
    {
      var result = await mongoDbSettings.collectionAuth.Find(new BsonDocument()).ToListAsync();
      return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] AuthDTO userDTO)
    {
      // search if email exists
      var filter = Builders<Auth>.Filter.Eq("email", userDTO.Email);

      var result = await mongoDbSettings.collectionAuth.Find(filter).FirstOrDefaultAsync();

      if (result == null)
      {
        return BadRequest("The user does not exist");
      }

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

      var result = await mongoDbSettings.collectionAuth.Find(filter).FirstOrDefaultAsync();

      if (result != null)
      {
        return BadRequest(new { message = "Email already exists" });
      }

      // encriptar password
      auth.Password = BCrypt.Net.BCrypt.HashPassword(auth.Password);

      // save user

      await mongoDbSettings.collectionAuth.InsertOneAsync(auth);

      return Ok(new { message = "User created" });
    }


  }
}