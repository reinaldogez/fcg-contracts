using Fcg.Contracts.Enums;

namespace Fcg.Contracts.Events;

public record PaymentProcessedEvent
{
    public int EventVersion { get; init; } = 1;
    public DateTimeOffset OccurredAt { get; init; }
    public Guid PaymentId { get; init; }
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
    public string UserEmail { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public Guid GameId { get; init; }
    public string GameName { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public PaymentStatus Status { get; init; }
    public string? RejectionReason { get; init; }
}
