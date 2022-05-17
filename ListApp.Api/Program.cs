using AutoMapper;
using ListApp.Api.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiModel = ListApp.Api.Models;
using DatabaseModel = ListApp.Api.Database.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDbContext<ListContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAutoMapper(config =>
{
    config.CreateMap<DatabaseModel.List, ApiModel.List>().ReverseMap();
    config.CreateMap<DatabaseModel.ListItem, ApiModel.ListItem>().ReverseMap();
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ListContext>();
    context.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", () => "List Freak API");

app.MapGet("/lists", async (ListContext db, [FromServices] IMapper mapper) =>
      mapper.Map<List<ApiModel.List>>(
          await db.Lists
            .Include(x => x.ListItems)
            .ToListAsync()));

app.MapGet("/lists/{id}", async (long id, ListContext db, [FromServices] IMapper mapper) =>
    await db.Lists.Include(x => x.ListItems).FirstOrDefaultAsync(x => x.Id == id)
        is DatabaseModel.List list
            ? Results.Ok(mapper.Map<ApiModel.List>(list))
            : Results.NotFound());

app.MapGet("/lists/guid/{guid}", async (string guid, ListContext db, [FromServices] IMapper mapper) =>
    await db.Lists.Include(x => x.ListItems).FirstOrDefaultAsync(x => x.Guid == guid)
        is DatabaseModel.List list
            ? Results.Ok(mapper.Map<ApiModel.List>(list))
            : Results.NotFound());

app.MapGet("/lists/ownerEmail/{email}", async (string email, ListContext db, [FromServices] IMapper mapper) =>
    mapper.Map<List<ApiModel.List>>(
        await db.Lists
            .Include(x => x.ListItems)
            .Where(x => x.OwnerEmail == email)
            .ToListAsync()));

app.MapPost("/lists", async (ApiModel.List list, ListContext db, [FromServices] IMapper mapper) =>
     {
         await db.Lists.AddAsync(mapper.Map<DatabaseModel.List>(list));
         await db.SaveChangesAsync();

         return Results.Created($"/lists/{list.Id}", list);
     });

app.MapPut("/lists/{id}", async (long id, ApiModel.List list, ListContext db, [FromServices] IMapper mapper) =>
{
    var oldList = await db.Lists.FindAsync(id);

    if (oldList is null) return Results.NotFound();

    mapper.Map(list, oldList);

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/lists/{id}", async (long id, ListContext db, [FromServices] IMapper mapper) =>
{
    if (await db.Lists.FindAsync(id) is DatabaseModel.List list)
    {
        db.Lists.Remove(list);

        await db.SaveChangesAsync();

        return Results.Ok(mapper.Map<ApiModel.List>(list));
    }

    return Results.NotFound();
});

app.Run();