namespace Library.Modules.Members.Domain;

public class RefreshToken
{
    public Guid Id { get; private set; }
    public Guid MemberId { get; private set; }
    public string Token { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt != null;
    public bool IsActive => !IsRevoked && !IsExpired;

    private RefreshToken() { } // Required by EF Core

    public static RefreshToken Create(Guid memberId, string token, int daysValid = 7)
    {
        if (memberId == Guid.Empty) throw new ArgumentException("MemberId cannot be empty.", nameof(memberId));
        if (string.IsNullOrWhiteSpace(token)) throw new ArgumentException("Token cannot be empty.", nameof(token));
        if (daysValid <= 0) throw new ArgumentOutOfRangeException(nameof(daysValid), "Days valid must be greater than zero.");

        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            MemberId = memberId,
            Token = token,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(daysValid)
        };
    }

    public void Revoke()
    {
        if (!IsRevoked)
        {
            RevokedAt = DateTime.UtcNow;
        }
    }
}
