using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MyBackend_MongoDB_CSharp.Data;
using MyBackend_MongoDB_CSharp.Repositories;
using MyBackend_MongoDB_CSharp.Service;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<IClientsRepositories, ClientsRepositories>();


builder.Services.Configure<DataBaseSetting>(options =>
{
    options.ConnectionString = builder.Configuration.GetSection("MongoDatabase:ConnectionString").Value!;
    options.MongoDB_Name = builder.Configuration.GetSection("MongoDatabase:MongoDB_Name").Value!;
    options.MongoDB_Collection_One = builder.Configuration.GetSection("MongoDatabase:MongoDB_Collection_One").Value!;
    options.MongoDB_Collection_Two = builder.Configuration.GetSection("MongoDatabase:MongoDB_Collection_Two").Value!;
});

var fontendUrl = builder.Configuration.GetSection("MyFrontend").Value;

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins(fontendUrl!).AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// config de jwt
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
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
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Token"]!)
            )
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
