using GiornaleOnline.DataContext;
using GiornaleOnline.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer; // AGGIUNTA QUESTA USING
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Error()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddSerilog();

builder.Services.AddDbContext<GOContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("default"),b=>b.MigrationsAssembly("GiornaleOnline")));
builder.Services.AddControllers();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            // andiamo a prenderci delle configurazioni custom se prima bene o male è standard questa parte è a se
            ValidAudience = builder.Configuration["Jwt:Audience"], 
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
builder.Services.AddAuthorization();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); //questa è la pipeline per l'autenticazione: quando arribva una richiesta http e prima di entrare nei controller controlla se c'è un token valido
app.UseAuthorization();

app.MapControllers();

app.MigrateDatabase();

app.Run();
