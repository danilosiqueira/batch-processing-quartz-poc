using BatchProcessingQuartzPoc.Domain;

namespace BatchProcessingQuartzPoc.Application.Responses;

public record BatchActionItemDto(Guid Id, string ItemReference, BatchActionItemStatus Status, int AttemptCount, string? LastError, DateTime CreatedAtUtc, DateTime? StartedAtUtc, DateTime? CompletedAtUtc);
