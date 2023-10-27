using API.Data;
using API.Dtos;
using API.Model;
using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var sqlConnectionBuilder = new SqlConnectionStringBuilder
{
    ConnectionString = builder.Configuration.GetConnectionString("SQLDbConnection"),
    UserID = builder.Configuration["UserId"],
    Password = builder.Configuration["Password"]
};

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(sqlConnectionBuilder.ConnectionString));
builder.Services.AddScoped<ICommandRepo, CommandRepo>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.Map("/", () => "Hello World!");

app.MapGet("/api/v1/commands", async (ICommandRepo repo, IMapper mapper) =>
{
    var commands = await repo.GetAllCommands();
    return Results.Ok(mapper.Map<IEnumerable<CommandReadDto>>(commands));
});

app.MapGet("/api/v1/command/{id}", async (ICommandRepo repo, IMapper mapper, int id) =>
{
    var command = await repo.GetCommandById(id);
    if (command != null)
    {
        return Results.Ok(mapper.Map<CommandReadDto>(command));
    }
    return Results.NotFound();
});

app.MapPost("/api/v1/command", async (ICommandRepo repo, IMapper mapper, CommandCreateDto createDto) =>
{
    var model = mapper.Map<Command>(createDto);
    await repo.CreateCommand(model);
    await repo.SaveChanges();
    var responseModel = mapper.Map<CommandReadDto>(model);
    return Results.Created($"api/v1/command/{model.Id}", responseModel);
});

app.MapPut("/api/v1/command/{id}", async (ICommandRepo repo, IMapper mapper, int id, CommandUpdateDto updateDto) =>
{
    var command = await repo.GetCommandById(id);
    if (command != null)
    {
        mapper.Map(updateDto, command);
        await repo.SaveChanges();
        return Results.NoContent();
    }
    return Results.NotFound();
});

app.MapDelete("/api/v1/command/{id}", async (ICommandRepo repo, IMapper mapper, int id) =>
{
    var command = await repo.GetCommandById(id);
    if (command != null)
    {
        repo.DeleteCommand(command);
        await repo.SaveChanges();
        return Results.NoContent();
    }
    return Results.NotFound();
});
app.Run();
