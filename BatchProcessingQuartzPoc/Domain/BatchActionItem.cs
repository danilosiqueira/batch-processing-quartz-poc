using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BatchProcessingQuartzPoc.Domain;

public class BatchActionItem
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid BatchActionId { get; set; }

    [ForeignKey(nameof(BatchActionId))]
    public BatchAction? BatchAction { get; set; }

    [Required]
    [MaxLength(200)]
    public string ItemReference { get; set; } = string.Empty;

    public BatchActionItemStatus Status { get; set; } = BatchActionItemStatus.Pending;

    public int AttemptCount { get; set; } = 0;

    [MaxLength(1000)]
    public string? LastError { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? StartedAtUtc { get; set; }

    public DateTime? CompletedAtUtc { get; set; }

    public void MarkProcessing()
    {
        Status = BatchActionItemStatus.Processing;
        AttemptCount++;
        StartedAtUtc = DateTime.UtcNow;
    }

    public void MarkCompleted()
    {
        Status = BatchActionItemStatus.Completed;
        CompletedAtUtc = DateTime.UtcNow;
        LastError = null;
    }

    public void MarkFailed(string error)
    {
        Status = BatchActionItemStatus.Failed;
        LastError = error;
        CompletedAtUtc = DateTime.UtcNow;
    }
}
