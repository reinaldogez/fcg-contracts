namespace Fcg.Contracts.Events;

public record UserCreatedEvent
{
    public int EventVersion { get; init; } = 1;
    public DateTimeOffset OccurredAt { get; init; }
    public Guid UserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}
