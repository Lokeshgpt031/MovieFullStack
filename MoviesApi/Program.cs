using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApi.Middleware;
using MovieApi.Models; // For MongoDbSettings
using MovieApi.Service;
// Ensure the "FileCache" folder is deleted on app initialization
var cacheFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "FileCache");
if (Directory.Exists(cacheFolderPath))
{
    Directory.Delete(cacheFolderPath, true);
}




var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddSingleton<IMoviesService, MoviesService>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(config =>
{
    config.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "http://localhost:5075")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Ensure CORS is applied early in the pipeline
app.UseCors("AllowAll");

app.UseMiddleware<CacheMiddleware>();

// Define endpoints

app.UseHttpsRedirection();
// app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () => Results.Ok("Healthy"));
app.MapGet("/error", () => Results.Problem("An error occurred"));


app.MapGet("/", () => Results.Redirect("/swagger/index.html"));

app.Run();
