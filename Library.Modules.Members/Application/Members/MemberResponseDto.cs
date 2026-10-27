namespace Library.Modules.Members.Application.Members;

public record MemberResponseDto(
    Guid Id,
    string FullName,
    string Email,
    string? PhoneNumber,
    string Status,
    string Type,
    int MaxAllowedLoans,
    DateTime CreatedAt);
