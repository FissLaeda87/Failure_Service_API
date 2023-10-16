using Failure_Service;
using Microsoft.EntityFrameworkCore;


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

app.MapGet("/", () => "Hello World!");

//������ ��� ������������������ �������
app.MapGet("/events", async (EventsDb db) => await db.Events.ToListAsync());

//������ �������, ���������� � �����
app.MapGet("/events/archived", async (EventsDb db) => await db.Events.Where(t => t.IsArchived).ToListAsync());

//������� ������� �� ��� id
app.MapGet("/events/{id}", async (int id, EventsDb db) =>
    await db.Events.FindAsync(id)
        is Event ev
            ? Results.Ok(ev)
            : Results.NotFound());

//��������� ����� �������
app.MapPost("/events/register", async (Event ev, EventsDb db) =>
{
    ev.IsArchived = ev.Timestamp < DateTime.Now.AddMonths(-1);
    db.Events.Add(ev);
    await db.SaveChangesAsync();
    return Results.Created($"/events/register/{ev.Id}", ev);
});

app.Run();
