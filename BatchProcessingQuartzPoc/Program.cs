using BatchProcessingQuartzPoc.Application;
using BatchProcessingQuartzPoc.Application.Abstractions;
using BatchProcessingQuartzPoc.Application.Mappers;
using BatchProcessingQuartzPoc.Application.Requests;
using BatchProcessingQuartzPoc.Application.Handlers;
using BatchProcessingQuartzPoc.Domain;
using BatchProcessingQuartzPoc.Infrastructure.Persistence;
using BatchProcessingQuartzPoc.Infrastructure.Quartz;
using Microsoft.EntityFrameworkCore;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("BatchDb")
    ?? builder.Configuration["POSTGRES_CONNECTION"]
    ?? "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=postgres";

builder.Services.AddDbContext<BatchDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    q.AddJob<ProcessItemJob>(opts => opts.WithIdentity(ProcessItemJob.Key).StoreDurably());
});

builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = false;
});

builder.Services.AddScoped<IActionHandler, ProcessItemHandler>();
builder.Services.AddScoped<IAsyncActionDispatcher, QuartzAsyncActionDispatcher>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

await EnsureDatabaseCreatedAsync(app.Services);

app.MapPost("/batch-actions", async (CreateBatchActionRequest request, BatchDbContext dbContext, IAsyncActionDispatcher dispatcher, CancellationToken cancellationToken) =>
{
    if (request.Items.Count == 0)
    {
        return Results.BadRequest("At least one item is required.");
    }

    var batchAction = new BatchAction
    {
        Id = Guid.NewGuid(),
        ActionType = ActionTypes.ProcessItem,
        Status = BatchActionStatus.Created,
        Items = request.Items
            .Select(itemReference => new BatchActionItem
            {
                Id = Guid.NewGuid(),
                ItemReference = itemReference,
                Status = BatchActionItemStatus.Pending
            })
            .ToList()
    };

    dbContext.BatchActions.Add(batchAction);
    await dbContext.SaveChangesAsync(cancellationToken);

    foreach (var item in batchAction.Items)
    {
        await dispatcher.DispatchItemAsync(ActionTypes.ProcessItem, batchAction.Id, item.Id, item.ItemReference, cancellationToken);
    }

    return Results.Accepted($"/batch-actions/{batchAction.Id}", new { batchAction.Id });
});

app.MapGet("/batch-actions/{id:guid}", async (Guid id, BatchDbContext dbContext, CancellationToken cancellationToken) =>
{
    var action = await dbContext.BatchActions
        .Include(a => a.Items)
        .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    if (action is null)
    {
        return Results.NotFound();
    }

    action.UpdateStatusFromItems();
    await dbContext.SaveChangesAsync(cancellationToken);

    return Results.Ok(action.ToDto());
});

app.Run();

static async Task EnsureDatabaseCreatedAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<BatchDbContext>();
    await db.Database.EnsureCreatedAsync();
}
