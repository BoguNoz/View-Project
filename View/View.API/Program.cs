using Microsoft.EntityFrameworkCore;
using View.Model;
using System;
using View.Model.Enteties;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("AppDbContext");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

// Adding Authentication
builder.Services.AddAuthentication();
builder.Services.AddIdentityApiEndpoints<ApplicationUserModel>().AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddControllers();
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

app.MapIdentityApi<ApplicationUserModel>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
