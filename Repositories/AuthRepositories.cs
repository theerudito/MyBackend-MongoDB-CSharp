using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using MyBackend_MongoDB_CSharp.Data;
using MyBackend_MongoDB_CSharp.Models;
using MyBackend_MongoDB_CSharp.Models.DTOs;
using MyBackend_MongoDB_CSharp.Service;

namespace MyBackend_MongoDB_CSharp.Repositories
{
  public class AuthRepositories : IAuthRepositories
  {
    MongoDbSettings mongoDbSettings = new MongoDbSettings();
    private readonly IConfiguration config;

    public AuthRepositories(IConfiguration config)
    {
      this.config = config;
    }


    public Task DeleteUser(string id)
    {
      throw new NotImplementedException();
    }

    public Task<Auth> GetUserById(string id)
    {
      throw new NotImplementedException();
    }

    public async Task<IEnumerable<Auth>> Get_All_Users()
    {
      return await mongoDbSettings.collectionAuth.Find(new BsonDocument()).ToListAsync();
    }

    public Task Login(AuthDTO userDTO)
    {
      // search if email exists
      var filter = Builders<Auth>.Filter.Eq("email", userDTO.Email);

      var user = mongoDbSettings.collectionAuth.Find(filter).FirstOrDefault();

      if (user == null)
      {
        return Task.FromResult(false);
      }

      // compare password
      if (!BCrypt.Net.BCrypt.Verify(userDTO.Password, user.Password))
      {
        return Task.FromResult(false);
      }

      // generate token
      var tokenHandler = new JwtSecurityTokenHandler();

      var key = Encoding.ASCII.GetBytes(config["Jwt:Token"]);

      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(new Claim[]
        {
            //new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.Username),
            //new Claim(ClaimTypes.Email, user.Email)
        }),
        Expires = DateTime.UtcNow.AddDays(7),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
      };

      var token = tokenHandler.CreateToken(tokenDescriptor);

      return Task.FromResult(token);
    }

    public async Task Register(Auth user)
    {
      // search if email already exists
      var filter = Builders<Auth>.Filter.Eq("email", user.Email);

      var result = await mongoDbSettings.collectionAuth.Find(filter).FirstOrDefaultAsync();

      if (result != null)
      {
        throw new Exception("Email already exists");
      }

      // encriptar password
      user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

      // save user

      await mongoDbSettings.collectionAuth.InsertOneAsync(user);
    }

    public Task<Auth> UpdateUser(string id, Auth user)
    {
      throw new NotImplementedException();
    }
  }
}