using Microsoft.EntityFrameworkCore;
using BlackJackWebAPI.Models;
using LibShared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
	options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
		new MySqlServerVersion(new Version(8, 0, 21))); 
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();