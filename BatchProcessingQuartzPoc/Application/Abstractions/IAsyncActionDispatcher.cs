namespace BatchProcessingQuartzPoc.Application.Abstractions;

public interface IAsyncActionDispatcher
{
    Task DispatchItemAsync(string actionType, Guid batchActionId, Guid batchActionItemId, string itemReference, CancellationToken cancellationToken = default);
}
