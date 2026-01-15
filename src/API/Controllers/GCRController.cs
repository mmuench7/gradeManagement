using API.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.GradeChangeRequest;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/GradeChangeRequest")]
[Authorize]
public class GCRController : ControllerBase
{
    private readonly IGCRService _gcrService;

    public GCRController(IGCRService gcrService)
    {
        _gcrService = gcrService;
    }

    [HttpPost]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> Create([FromBody] CreateGCRDTO dto)
    {
        int? teacherId = GetUserIdFromJwt();
        if (teacherId == null)
        {
            return Unauthorized("Missing user id in token.");
        }

        int? requestId = await _gcrService.CreateAsync(teacherId.Value, dto);

        if (requestId == null)
        {
            return BadRequest("Could not create grade change request.");
        }

        return CreatedAtAction(nameof(GetById), new { id = requestId.Value }, new { id = requestId.Value });
    }

    [HttpGet("mine")]
    [Authorize(Roles = "Teacher")]
    public async Task<ActionResult<List<GCRDTO>>> GetMine()
    {
        int? teacherId = GetUserIdFromJwt();
        if (teacherId == null)
        {
            return Unauthorized("Missing user id in token.");
        }

        List<GCRDTO> items = await _gcrService.GetMineAsync(teacherId.Value);
        return Ok(items);
    }

    [HttpGet("pending")]
    [Authorize(Roles = "Principal")]
    public async Task<ActionResult<List<GCRDTO>>> GetPending()
    {
        int? principalId = GetUserIdFromJwt();
        if (principalId == null)
        {
            return Unauthorized("Missing user id in token.");
        }

        List<GCRDTO> items = await _gcrService.GetPendingForPrincipalAsync(principalId.Value);
        return Ok(items);
    }

    [HttpPost("{id:int}/approve")]
    [Authorize(Roles = "Principal")]
    public async Task<IActionResult> Approve([FromRoute] int id)
    {
        int? principalId = GetUserIdFromJwt();
        if (principalId == null)
        {
            return Unauthorized("Missing user id in token.");
        }

        bool ok = await _gcrService.ApproveAsync(principalId.Value, id);

        if (!ok)
        {
            return Conflict("Could not approve this request.");
        }

        return NoContent();
    }

    [HttpPost("{id:int}/reject")]
    [Authorize(Roles = "Principal")]
    public async Task<IActionResult> Reject(int id, [FromBody] RejectGCRDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        int? principalId = GetUserIdFromJwt();
        if (principalId == null)
        {
            return Unauthorized();
        }

        bool ok = await _gcrService.RejectAsync(principalId.Value, id, dto.RejectionReason);
        return ok ? NoContent() : Conflict("Could not reject this request.");
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Teacher,Principal")]
    public async Task<ActionResult<GCRDTO>> GetById([FromRoute] int id)
    {
        int? userId = GetUserIdFromJwt();
        if (userId == null)
        {
            return Unauthorized("Missing user id in token.");
        }

        GCRDTO? dto = await _gcrService.GetByIdAsync(id);
        if (dto == null)
        {
            return NotFound();
        }

        if (User.IsInRole("Teacher"))
        {
            if (dto.TeacherId != userId.Value)
            {
                return Forbid();
            }
        }
        else if (User.IsInRole("Principal"))
        {
            if (dto.PrincipalId != userId.Value)
            {
                return Forbid();
            }
        }
        else
        {
            return Forbid();
        }

        return Ok(dto);
    }

    [HttpGet("history")]
    [Authorize(Roles = "Principal")]
    public async Task<ActionResult<List<GCRDTO>>> GetHistory()
    {
        int? principalId = GetUserIdFromJwt();
        if (principalId == null)
        {
            return Unauthorized();
        }

        var items = await _gcrService.GetHistoryForPrincipalAsync(principalId.Value);
        return Ok(items);
    }

    private int? GetUserIdFromJwt()
    {
        string? sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (int.TryParse(sub, out int id))
        {
            return id;
        }

        return null;
    }
}
