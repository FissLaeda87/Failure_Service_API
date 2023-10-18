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
    foreach (Events ev in db.Events)
    {
        ev.isArchived = ev.timestamp < DateTime.Now.AddMonths(-1);
        await db.SaveChangesAsync();
    }
    await db.Events.Where(t => t.isArchived).ToListAsync();
});
    

//Получает информацию о событии по id
app.MapGet("/events/{id}", async (int id, EventsDb db) =>
    await db.Events.FindAsync(id)
        is Events ev
            ? Results.Ok(ev)
            : Results.NotFound());

//Заносит информацию о сервисе
app.MapPost("/events/register", async (Events ev, EventsDb db) =>
{
    ev.isArchived = ev.timestamp < DateTime.Now.AddMonths(-1);
    db.Events.Add(ev);
    await db.SaveChangesAsync();
    return Results.Created($"/events/register/{ev.id}", ev);
});

app.Run();
