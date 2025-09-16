using FluentValidation;
using Identity.Core.Configurations;
using Identity.Core.Interfaces;
using Identity.Core.Models.User;
using Identity.Core.Services;
using Identity.Core.Validators.Client;
using Identity.Core.Services.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureIdentityCore(builder.Configuration);
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
