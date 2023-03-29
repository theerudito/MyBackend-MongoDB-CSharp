using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MyBackend_MongoDB_CSharp.Data;
using MyBackend_MongoDB_CSharp.Repositories;
using MyBackend_MongoDB_CSharp.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// añadir la configuración de la base de datos de mongo
builder.Services.Configure<DataBaseSetting>(builder.Configuration.GetSection("MongoDatabase"));
builder.Services.AddScoped<IClientsRepositories, ClientsRepositories>();

var proveedor = builder.Services.BuildServiceProvider();
var config = proveedor.GetRequiredService<IConfiguration>();

builder.Services.AddCors(options =>
{
  var fontendUrl = config.GetValue<string>("MyFrontend");

  options.AddDefaultPolicy(builder =>
  {
    builder.WithOrigins(fontendUrl)
    .AllowAnyMethod()
    .AllowAnyHeader();
  });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



// config de jwt
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
  options.RequireHttpsMetadata = false;
  options.SaveToken = true;
  options.TokenValidationParameters = new TokenValidationParameters()
  {
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidAudience = builder.Configuration["Jwt:Audience"],
    ValidIssuer = builder.Configuration["Jwt:Issuer"],
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Token"]))
  };
});





builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseCors();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
