using Microsoft.EntityFrameworkCore;
using View.Model;
using System;
using View.Model.Enteties;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using View.Repository.Databases;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IDatabaseRepository, MsDatabaseRepository>();

var connectionString = builder.Configuration.GetConnectionString("AppDbContext");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

// Adding Authentication
builder.Services.AddAuthentication();
builder.Services.AddIdentityApiEndpoints<ApplicationUserModel>().AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Ading Idendity User Model
app.MapIdentityApi<ApplicationUserModel>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
