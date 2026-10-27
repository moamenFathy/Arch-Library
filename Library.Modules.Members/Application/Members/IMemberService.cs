namespace Library.Modules.Members.Application.Members;

public interface IMemberService
{
    Task<MemberResponseDto> RegisterAsync(RegisterMemberCommand command, CancellationToken ct = default);
}
