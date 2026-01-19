namespace BatchProcessingQuartzPoc.Domain;

public enum BatchActionItemStatus
{
    Pending = 0,
    Processing = 1,
    Completed = 2,
    Failed = 3
}
