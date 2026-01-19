namespace BatchProcessingQuartzPoc.Application;

public record ActionExecutionContext(Guid BatchActionId, Guid BatchActionItemId, string ItemReference, string ActionType);
