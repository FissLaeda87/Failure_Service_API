using System.Text.Json.Nodes;
using Failure_Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OpenApi;
using Newtonsoft.Json;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<EventsDb>();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Welcome to API for checking the health of your web services!\nPlease enter in the address bar: \".../swagger\" to check the service");

//Получает информацию обо всех событиях
app.MapGet("/events", async (EventsDb db) => await db.Events.ToListAsync());

//Получает информацию о событиях, ушедших в архив
app.MapGet("/events/archived", async (EventsDb db) =>
{
    foreach (Event ev in db.Events)
    {
        ev.IsArchived = ev.Timestamp < DateTime.Now.AddMonths(-1);
        await db.SaveChangesAsync();
    }
    await db.Events.Where(t => t.IsArchived).ToListAsync();
});
    

//Получает информацию о событии по id
app.MapGet("/events/{id}", async (int id, EventsDb db) =>
    await db.Events.FindAsync(id)
        is Event ev
            ? Results.Ok(ev)
            : Results.NotFound());

//Заносит информацию о сервисе
app.MapPost("/events/register", async (HttpContext query, EventsDb db) =>
{
    using var reader = new StreamReader(query.Request.Body);
    var requestBody = await reader.ReadToEndAsync();
    var text = JsonConvert.SerializeObject(requestBody);
    var jsonObj = JsonObject.Parse(text);
    string? errorMessage = jsonObj["errorMessage"]?.ToString();
    var statusCode = (int)jsonObj["statusCode"];
    var host = query.Request.Host.Host;
    var ev = new Event
    {
        Status = statusCode.ToString(),
        Timestamp = DateTime.Now,
        IsArchived = false,
        Name = host,
        Message = errorMessage
        
    };

    db.Events.Add(ev);
    await db.SaveChangesAsync();
    return Results.Created($"/events/register/{ev.Id}", ev);
});

app.Run();
