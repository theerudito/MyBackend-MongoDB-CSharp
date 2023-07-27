using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using MyBackend_MongoDB_CSharp.Data;
using MyBackend_MongoDB_CSharp.Helpers;
using MyBackend_MongoDB_CSharp.Models;
using MyBackend_MongoDB_CSharp.Models.DTOs;

namespace MyBackend_MongoDB_CSharp.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IMapper mapper;
        private readonly IMongoCollection<Auth> authCollections;
        private readonly IConfiguration config;

        public AuthController(
            IConfiguration config,
            IMapper mapper,
            IOptions<DataBaseSetting> databaseSettings
        )
        {
            this.mapper = mapper;
            this.config = config;

            var client = new MongoClient(databaseSettings.Value.ConnectionString);
            var database = client.GetDatabase(databaseSettings.Value.MongoDB_Name);
            authCollections = database.GetCollection<Auth>(
                databaseSettings.Value.MongoDB_Collection_Two
            );
        }

        [HttpGet]
        public async Task<ActionResult> GET_USERS()
        {
            var result = await authCollections.Find(auth => true).ToListAsync();

            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(new { message = "The users does not exist" });
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult> GetById(string id)
        {
            var result = await authCollections.Find(user => user.Id == id).FirstOrDefaultAsync();

            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(new { message = "The user does not exist" });
            }
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult> PUT_USER([FromBody] Auth auth, string id)
        {
            var result = await authCollections.Find(user => user.Id == id).FirstOrDefaultAsync();

            if (result != null)
            {
                var myAvatar = CreateAvatar.avatar;

                await authCollections.UpdateOneAsync(
                    user => user.Id == id,
                    Builders<Auth>.Update
                        .Set(user => user.Pic, myAvatar + auth.Username)
                        .Set(user => user.Username, auth.Username)
                        .Set(user => user.Email, auth.Email)
                        .Set(user => user.Password, BCrypt.Net.BCrypt.HashPassword(auth.Password))
                );

                return Ok(new { message = "The user has been updated" });
            }
            else
            {
                return BadRequest(new { message = "The user does not exist" });
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> DELETE_USER(string id)
        {
            await authCollections.DeleteOneAsync(user => user.Id == id);

            if (id != null)
            {
                return Ok(new { message = "The user has been deleted" });
            }
            else
            {
                return BadRequest(new { message = "The user does not exist" });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] AuthDTO userDTO)
        {
            var result = await authCollections
                .Find(user => user.Email == userDTO.Email)
                .FirstOrDefaultAsync();

            if (result != null)
            {
                // compare password
                if (BCrypt.Net.BCrypt.Verify(userDTO.Password, result.Password) == false)
                {
                    return BadRequest(new { message = "Password or email is incorrect" });
                }
                else
                {
                    // map userDTO to user
                    var user = mapper.Map<Auth>(userDTO);

                    // generate token
                    var token = GenerateToken(user);

                    var data = new
                    {
                        pic = result.Pic,
                        user = result.Username,
                        token
                    };

                    return Ok(data);
                }
            }
            else
            {
                return BadRequest(new { message = "The user does not exist" });
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] Auth auth)
        {
            var result = await authCollections
                .Find(user => user.Email == auth.Email)
                .FirstOrDefaultAsync();

            if (result is null)
            {
                var myAvatar = CreateAvatar.avatar;

                var neUser = new Auth
                {
                    Pic = myAvatar + auth.Username,
                    Username = auth.Username,
                    Email = auth.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(auth.Password),
                };

                await authCollections.InsertOneAsync(neUser);

                var token = GenerateToken(auth);

                var data = new
                {
                    pic = neUser.Pic,
                    user = auth.Username,
                    token = token
                };

                return Ok(data);
            }
            else
            {
                return BadRequest(new { message = "Email already exists" });
            }
        }

        private string GenerateToken(Auth auth)
        {
            var _secretKey = config.GetSection("Jwt").GetChildren().ToString()!;

            var keyByte = Encoding.ASCII.GetBytes(_secretKey);

            var clains = new ClaimsIdentity();

            clains.AddClaim(new Claim(ClaimTypes.NameIdentifier, auth.Username));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = clains,
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(keyByte),
                    SecurityAlgorithms.HmacSha512Signature
                )
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(tokenConfig);
        }
    }
}
