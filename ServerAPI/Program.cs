using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using TME1.Abstractions.Repositories;
using TME1.Abstractions.Services;
using TME1.Core.Repositories;
using TME1.Core.Services;
using TME1.ServerCore;
using TME1.ServerCore.DataTransferObjects;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
  .AddNdjson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
  options.SwaggerDoc("v1", new OpenApiInfo
  {
    Version = "v1",
    Title = "TME1",
    Description = "Test Task REST API",
    TermsOfService = new Uri("https://github.com/Technus/TME1/"),
    Contact = new OpenApiContact
    {
      Name = "Technus",
      Url = new Uri("https://github.com/Technus"),
    },
    License = new OpenApiLicense
    {
      Name = "Attribution-NonCommercial-NoDerivatives 4.0 International",
      Url = new Uri("https://github.com/Technus/TME1/blob/master/LICENSE"),
    }
  });
  options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});

builder.Services.AddDbContext<RobotContext>(config 
  => config.UseSqlServer(builder.Configuration["TME1"], options 
    => options.MigrationsAssembly(typeof(RobotContext).Assembly.GetName().Name)), ServiceLifetime.Singleton);

builder.Services
  .AddSingleton<IRobotRepository<int, RobotDto>, RobotRepository>()
  .AddSingleton<IRobotStateService<int>, RobotStateService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
