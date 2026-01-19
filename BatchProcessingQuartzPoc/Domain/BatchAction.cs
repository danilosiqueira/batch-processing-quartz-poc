using System.ComponentModel.DataAnnotations;

namespace BatchProcessingQuartzPoc.Domain;

public class BatchAction
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string ActionType { get; set; } = string.Empty;

    public BatchActionStatus Status { get; set; } = BatchActionStatus.Created;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedAtUtc { get; set; }

    public List<BatchActionItem> Items { get; set; } = new();

    public void UpdateStatusFromItems()
    {
        if (Items.Count == 0)
        {
            Status = BatchActionStatus.Created;
            CompletedAtUtc = null;
            return;
        }

        var hasFailures = Items.Any(i => i.Status == BatchActionItemStatus.Failed);
        var allCompleted = Items.All(i => i.Status == BatchActionItemStatus.Completed);
        var anyProcessing = Items.Any(i => i.Status == BatchActionItemStatus.Processing);
        var anyPending = Items.Any(i => i.Status == BatchActionItemStatus.Pending);

        if (allCompleted && !hasFailures)
        {
            Status = BatchActionStatus.Completed;
            CompletedAtUtc = DateTime.UtcNow;
            return;
        }

        if (allCompleted && hasFailures)
        {
            Status = BatchActionStatus.CompletedWithErrors;
            CompletedAtUtc = DateTime.UtcNow;
            return;
        }

        if (hasFailures && !anyProcessing && !anyPending)
        {
            Status = BatchActionStatus.CompletedWithErrors;
            CompletedAtUtc = DateTime.UtcNow;
            return;
        }

        Status = anyProcessing || !anyPending ? BatchActionStatus.InProgress : BatchActionStatus.Created;
        CompletedAtUtc = null;
    }
}
