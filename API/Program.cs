using Microsoft.EntityFrameworkCore;
using TME1.Abstractions.Repositories;
using TME1.Abstractions.Services;
using TME1.Core;
using TME1.Core.DataTransferObjects;
using TME1.Core.Repositories;
using TME1.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
  .AddNdjson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<RobotContext>(config 
  => config.UseSqlServer(builder.Configuration["TME1"], options 
    => options.MigrationsAssembly(typeof(RobotContext).Assembly.GetName().Name)), ServiceLifetime.Singleton);

builder.Services
  .AddSingleton<IRobotRepository<int, RobotDTO>, RobotRepository>()
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
