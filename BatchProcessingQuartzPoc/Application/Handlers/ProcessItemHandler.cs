using BatchProcessingQuartzPoc.Application.Abstractions;
using BatchProcessingQuartzPoc.Domain;
using BatchProcessingQuartzPoc.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BatchProcessingQuartzPoc.Application.Handlers;

public class ProcessItemHandler : IActionHandler
{
    private readonly BatchDbContext _dbContext;
    private readonly ILogger<ProcessItemHandler> _logger;

    public ProcessItemHandler(BatchDbContext dbContext, ILogger<ProcessItemHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public string ActionType => ActionTypes.ProcessItem;

    public async Task HandleAsync(ActionExecutionContext context, CancellationToken cancellationToken)
    {
        var item = await _dbContext.BatchActionItems
            .Include(i => i.BatchAction)
            .FirstOrDefaultAsync(i => i.Id == context.BatchActionItemId, cancellationToken);

        if (item == null)
        {
            _logger.LogWarning("Item {ItemId} not found for batch {BatchId}", context.BatchActionItemId, context.BatchActionId);
            return;
        }

        item.MarkProcessing();
        item.BatchAction?.UpdateStatusFromItems();
        await _dbContext.SaveChangesAsync(cancellationToken);

        try
        {
            await Task.Delay(TimeSpan.FromMilliseconds(300), cancellationToken);
            item.MarkCompleted();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            item.MarkFailed(ex.Message);
            _logger.LogError(ex, "Processing failed for item {ItemId}", context.BatchActionItemId);
        }

        item.BatchAction?.UpdateStatusFromItems();
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
