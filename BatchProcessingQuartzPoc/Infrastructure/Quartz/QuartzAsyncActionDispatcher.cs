using BatchProcessingQuartzPoc.Application.Abstractions;
using Quartz;

namespace BatchProcessingQuartzPoc.Infrastructure.Quartz;

public class QuartzAsyncActionDispatcher : IAsyncActionDispatcher
{
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly ILogger<QuartzAsyncActionDispatcher> _logger;

    public QuartzAsyncActionDispatcher(ISchedulerFactory schedulerFactory, ILogger<QuartzAsyncActionDispatcher> logger)
    {
        _schedulerFactory = schedulerFactory;
        _logger = logger;
    }

    public async Task DispatchItemAsync(string actionType, Guid batchActionId, Guid batchActionItemId, string itemReference, CancellationToken cancellationToken = default)
    {
        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

        var jobData = new JobDataMap
        {
            { JobDataKeys.ActionType, actionType },
            { JobDataKeys.BatchActionId, batchActionId.ToString() },
            { JobDataKeys.BatchActionItemId, batchActionItemId.ToString() },
            { JobDataKeys.ItemReference, itemReference }
        };

        var trigger = TriggerBuilder.Create()
            .WithIdentity($"batch-item-{batchActionItemId}")
            .StartNow()
            .UsingJobData(jobData)
            .ForJob(ProcessItemJob.Key)
            .Build();

        await scheduler.ScheduleJob(trigger, cancellationToken);

        _logger.LogInformation("Scheduled item {ItemId} for batch {BatchId}", batchActionItemId, batchActionId);
    }
}
