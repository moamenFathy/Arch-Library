using Microsoft.AspNetCore.Identity;
using Library.Modules.Members.Domain;
using Library.Shared.Exceptions;

namespace Library.Modules.Members.Application.Members;

public class MemberService(UserManager<Member> userManager) : IMemberService
{
    public async Task<MemberResponseDto> RegisterAsync(RegisterMemberCommand command, CancellationToken ct = default)
    {
        var existingUser = await userManager.FindByEmailAsync(command.Email);
        if (existingUser != null)
        {
            throw AppException.Conflict("A member with this email already exists.");
        }

        var member = Member.Create(
            command.FirstName,
            command.LastName,
            command.Email,
            command.PhoneNumber,
            command.Type);

        var result = await userManager.CreateAsync(member, command.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw AppException.BadRequest($"Failed to register member: {errors}");
        }

        return new MemberResponseDto(
            member.Id,
            member.FullName,
            member.Email!,
            member.PhoneNumber,
            member.Status.ToString(),
            member.Type.ToString(),
            member.MaxAllowedLoans,
            member.CreatedAt);
    }
}
