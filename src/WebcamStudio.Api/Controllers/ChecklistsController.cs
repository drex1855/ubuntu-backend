using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebcamStudio.Application.Checklists;
using WebcamStudio.Application.Common;
using WebcamStudio.Application.Interfaces.Services;
using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Api.Controllers;


[Authorize]
public class ChecklistsController : ApiControllerBase
{
    private readonly IChecklistService _service;
    private readonly IFileStorageService _fileStorage;

    public ChecklistsController(IChecklistService service, IFileStorageService fileStorage)
    {
        _service = service;
        _fileStorage = fileStorage;
    }

    private const long MaxUploadBytes = 5 * 1024 * 1024;
    private const string StaffRoles = $"{nameof(AccountRole.Admin)},{nameof(AccountRole.Monitor)}";

    [RequestSizeLimit(MaxUploadBytes)]
    [HttpPost("attachments")]
    public async Task<IActionResult> UploadAttachment(IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest(ApiResponse<UploadAttachmentResponse>.Fail("No se recibio ningun archivo."));

        await using var stream = file.OpenReadStream();
        var result = await _fileStorage.SaveImageAsync(stream, file.ContentType, ct);
        if (!result.Succeeded)
            return BadRequest(ApiResponse<UploadAttachmentResponse>.Fail(result.Error ?? "No se pudo guardar el archivo."));

        return Ok(ApiResponse<UploadAttachmentResponse>.Ok(new UploadAttachmentResponse(result.Value!.FileName)));
    }

    [Authorize(Roles = StaffRoles)]
    [HttpGet("attachments/{fileName}")]
    public async Task<IActionResult> GetAttachment(string fileName, CancellationToken ct)
    {
        var file = await _fileStorage.OpenReadAsync(fileName, ct);
        if (file is null)
            return NotFound();

        return File(file.Value.Content, file.Value.ContentType);
    }

    [HttpGet("rooms")]
    public async Task<IActionResult> GetRooms(CancellationToken ct)
    {
        var rooms = await _service.GetRoomsAsync(ct);
        return Ok(ApiResponse<List<RoomDto>>.Ok(rooms));
    }

    [Authorize(Roles = StaffRoles)]
    [HttpPost("rooms")]
    public async Task<IActionResult> CreateRoom([FromBody] CreateRoomRequest request, CancellationToken ct) =>
        HandleResult(await _service.CreateRoomAsync(request, ct));

    [Authorize(Roles = StaffRoles)]
    [HttpPost("rooms/{roomId:guid}/items")]
    public async Task<IActionResult> AddTemplateItem(
        Guid roomId, [FromBody] CreateChecklistTemplateItemRequest request, CancellationToken ct) =>
        HandleResult(await _service.AddTemplateItemAsync(roomId, request, ct));

    [HttpGet("rooms/{roomId:guid}/template")]
    public async Task<IActionResult> GetTemplate(Guid roomId, CancellationToken ct) =>
        HandleResult(await _service.GetRoomTemplateAsync(roomId, ct));

    [HttpPost("submit")]
    public async Task<IActionResult> Submit([FromBody] SubmitChecklistRequest request, CancellationToken ct) =>
        HandleResult(await _service.SubmitChecklistAsync(CurrentAccountId, request, ct));
}
