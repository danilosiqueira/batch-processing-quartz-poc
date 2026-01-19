using BatchProcessingQuartzPoc.Application.Responses;
using BatchProcessingQuartzPoc.Domain;

namespace BatchProcessingQuartzPoc.Application.Mappers;

public static class BatchActionMapping
{
    public static BatchActionDto ToDto(this BatchAction action)
    {
        var items = action.Items
            .Select(i => new BatchActionItemDto(i.Id, i.ItemReference, i.Status, i.AttemptCount, i.LastError, i.CreatedAtUtc, i.StartedAtUtc, i.CompletedAtUtc))
            .ToList();

        return new BatchActionDto(action.Id, action.ActionType, action.Status, action.CreatedAtUtc, action.CompletedAtUtc, items);
    }
}
