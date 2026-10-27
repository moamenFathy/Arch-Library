using Library_Api.Common;
using Library.Modules.Members.Application.Members;
using Microsoft.AspNetCore.Mvc;

namespace Library_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MembersController(IMemberService memberService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterMemberCommand command, CancellationToken ct)
    {
        var member = await memberService.RegisterAsync(command, ct);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<MemberResponseDto>.Success(member, "Member registered successfully."));
    }
}
