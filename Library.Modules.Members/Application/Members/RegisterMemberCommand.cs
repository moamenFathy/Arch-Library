using Library.Modules.Members.Domain.Enums;

namespace Library.Modules.Members.Application.Members;

public record RegisterMemberCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string? PhoneNumber = null,
    MembershipType Type = MembershipType.Standard);
