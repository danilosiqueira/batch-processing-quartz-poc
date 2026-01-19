using BatchProcessingQuartzPoc.Domain;

namespace BatchProcessingQuartzPoc.Application.Responses;

public record BatchActionDto(Guid Id, string ActionType, BatchActionStatus Status, DateTime CreatedAtUtc, DateTime? CompletedAtUtc, IReadOnlyCollection<BatchActionItemDto> Items);
