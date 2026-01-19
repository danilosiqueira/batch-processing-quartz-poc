namespace BatchProcessingQuartzPoc.Application.Abstractions;

public interface IActionHandler
{
    string ActionType { get; }

    Task HandleAsync(ActionExecutionContext context, CancellationToken cancellationToken);
}
