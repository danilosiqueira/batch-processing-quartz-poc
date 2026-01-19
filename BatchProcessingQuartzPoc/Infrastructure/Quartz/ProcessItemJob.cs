using BatchProcessingQuartzPoc.Application;
using BatchProcessingQuartzPoc.Application.Abstractions;
using Quartz;

namespace BatchProcessingQuartzPoc.Infrastructure.Quartz;

public class ProcessItemJob : IJob
{
    public static readonly JobKey Key = new("ProcessItemJob");

    private readonly IEnumerable<IActionHandler> _handlers;
    private readonly ILogger<ProcessItemJob> _logger;

    public ProcessItemJob(IEnumerable<IActionHandler> handlers, ILogger<ProcessItemJob> logger)
    {
        _handlers = handlers;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var jobData = context.MergedJobDataMap;

        var actionType = jobData.GetString(JobDataKeys.ActionType);
        var actionIdRaw = jobData.GetString(JobDataKeys.BatchActionId);
        var itemIdRaw = jobData.GetString(JobDataKeys.BatchActionItemId);
        var itemReference = jobData.GetString(JobDataKeys.ItemReference) ?? string.Empty;

        if (!Guid.TryParse(actionIdRaw, out var batchActionId) || !Guid.TryParse(itemIdRaw, out var batchActionItemId) || string.IsNullOrWhiteSpace(actionType))
        {
            _logger.LogWarning("Invalid job payload. actionType={ActionType} batchActionId={BatchActionId} itemId={ItemId}", actionType, actionIdRaw, itemIdRaw);
            return;
        }

        var handler = _handlers.FirstOrDefault(h => string.Equals(h.ActionType, actionType, StringComparison.OrdinalIgnoreCase));

        if (handler == null)
        {
            _logger.LogWarning("No handler registered for action type {ActionType}", actionType);
            return;
        }

        var executionContext = new ActionExecutionContext(batchActionId, batchActionItemId, itemReference, actionType);

        await handler.HandleAsync(executionContext, context.CancellationToken);
    }
}
