using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using simpleApi.Data;
using simpleApi.Models;

namespace simpleApi.Routes;

public static class PersonRoute
{
    public static void PersonRoutes(this WebApplication app)
    {
        var route = app.MapGroup("person");
        route.MapPost("", async (PersonRequest req, PersonContext context) =>
            {
                Person person = new Person(req.name);
                await context.AddAsync(person);
                await context.SaveChangesAsync();
            })
            .WithName("PostPerson").WithOpenApi();

        route.MapGet("", async (PersonContext context) =>
        {
            var persons = await context.People.ToListAsync();
            return Results.Ok(persons);
        }).WithName("GetPerson").WithOpenApi();

        route.MapPut("{id:guid}", async (Guid id, PersonRequest req,
            PersonContext context) =>
        {
            var person = await context.People.FirstOrDefaultAsync(x => x.Id == id);
            if (person == null)
            {
                return Results.NotFound();
            }

            person.ChangeName(req.name);
            await context.SaveChangesAsync();
            return Results.Ok(person);
        }).WithName("PutPerson").WithOpenApi();

        route.MapDelete("{id:guid}", async (Guid id,
            PersonContext context) =>
        {
            var person = await context.People.FirstOrDefaultAsync(x => x.Id == id);
            if (person == null)
            {
                return Results.NotFound();
            }

            await context.People.Where(p => p.Id == id).ExecuteDeleteAsync();
            return Results.Ok(person);
        }).WithName("DeletePerson").WithOpenApi();
    }
}