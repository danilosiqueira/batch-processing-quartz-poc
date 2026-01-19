using System.ComponentModel.DataAnnotations;

namespace BatchProcessingQuartzPoc.Application.Requests;

public record CreateBatchActionRequest
{
    [Required]
    [MinLength(1)]
    public List<string> Items { get; init; } = new();
}
