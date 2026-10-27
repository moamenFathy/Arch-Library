using Microsoft.AspNetCore.Identity;
using Library.Modules.Members.Domain.Enums;

namespace Library.Modules.Members.Domain;

public class Member : IdentityUser<Guid>
{
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public MembershipStatus Status { get; private set; }
    public MembershipType Type { get; private set; }
    public int MaxAllowedLoans { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public string? SuspensionReason { get; private set; }

    public string FullName => $"{FirstName} {LastName}".Trim();

    private Member() { } // Required by EF Core

    public static Member Create(
        string firstName,
        string lastName,
        string email,
        string? phoneNumber = null,
        MembershipType type = MembershipType.Standard)
    {
        if (string.IsNullOrWhiteSpace(firstName)) throw new ArgumentException("First name is required.", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("Last name is required.", nameof(lastName));
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required.", nameof(email));

        return new Member
        {
            Id = Guid.NewGuid(),
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            UserName = email.Trim().ToLowerInvariant(),
            Email = email.Trim().ToLowerInvariant(),
            PhoneNumber = phoneNumber?.Trim(),
            Status = MembershipStatus.Active,
            Type = type,
            MaxAllowedLoans = CalculateMaxLoans(type),
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateProfile(string firstName, string lastName, string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(firstName)) throw new ArgumentException("First name is required.", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("Last name is required.", nameof(lastName));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        PhoneNumber = phoneNumber?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeMembershipType(MembershipType newType)
    {
        Type = newType;
        MaxAllowedLoans = CalculateMaxLoans(newType);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Suspend(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("Suspension reason is required.", nameof(reason));

        Status = MembershipStatus.Suspended;
        SuspensionReason = reason.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        Status = MembershipStatus.Active;
        SuspensionReason = null;
        UpdatedAt = DateTime.UtcNow;
    }

    private static int CalculateMaxLoans(MembershipType type) => type switch
    {
        MembershipType.Student => 5,
        MembershipType.Standard => 5,
        MembershipType.Premium => 10,
        _ => 5
    };
}
