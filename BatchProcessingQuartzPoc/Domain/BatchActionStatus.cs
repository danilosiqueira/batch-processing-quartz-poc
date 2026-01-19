namespace BatchProcessingQuartzPoc.Domain;

public enum BatchActionStatus
{
    Created = 0,
    InProgress = 1,
    Completed = 2,
    CompletedWithErrors = 3
}
