using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WebcamStudio.Application.Common;
using WebcamStudio.Application.Contacts;
using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Api.Controllers;

/// <summary>Base de datos de contactos: gente que escribio al WhatsApp del estudio,
/// normalmente desde el formulario publico del sitio.</summary>
[Authorize]
public class ContactsController : ApiControllerBase
{
    private readonly IContactService _service;

    public ContactsController(IContactService service)
    {
        _service = service;
    }

    private const string StaffRoles = $"{nameof(AccountRole.Admin)},{nameof(AccountRole.Monitor)}";

    /// <summary>Envio del formulario publico de contacto (sin sesion).</summary>
    [AllowAnonymous]
    [EnableRateLimiting("PublicContactForm")]
    [HttpPost("public")]
    public async Task<IActionResult> SubmitPublic([FromBody] CreatePublicContactRequest request, CancellationToken ct) =>
        HandleResult(await _service.SubmitPublicContactAsync(request, ct));

    [Authorize(Roles = StaffRoles)]
    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] string? search, [FromQuery] Guid? tagId, [FromQuery] bool? hasEmail, CancellationToken ct)
    {
        var contacts = await _service.SearchAsync(search, tagId, hasEmail, ct);
        return Ok(ApiResponse<List<ContactDto>>.Ok(contacts));
    }

    [Authorize(Roles = StaffRoles)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateContactRequest request, CancellationToken ct) =>
        HandleResult(await _service.CreateAsync(request, ct));

    [Authorize(Roles = StaffRoles)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateContactRequest request, CancellationToken ct) =>
        HandleResult(await _service.UpdateAsync(id, request, ct));

    [Authorize(Roles = StaffRoles)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct) =>
        HandleResult(await _service.DeleteAsync(id, ct));

    [Authorize(Roles = StaffRoles)]
    [HttpGet("tags")]
    public async Task<IActionResult> GetTags(CancellationToken ct)
    {
        var tags = await _service.GetTagsAsync(ct);
        return Ok(ApiResponse<List<TagDto>>.Ok(tags));
    }

    [Authorize(Roles = StaffRoles)]
    [HttpPost("tags")]
    public async Task<IActionResult> CreateTag([FromBody] CreateTagRequest request, CancellationToken ct) =>
        HandleResult(await _service.CreateTagAsync(request, ct));

    [Authorize(Roles = StaffRoles)]
    [HttpDelete("tags/{tagId:guid}")]
    public async Task<IActionResult> DeleteTag(Guid tagId, CancellationToken ct) =>
        HandleResult(await _service.DeleteTagAsync(tagId, ct));

    [Authorize(Roles = StaffRoles)]
    [HttpPost("{id:guid}/tags/{tagId:guid}")]
    public async Task<IActionResult> AssignTag(Guid id, Guid tagId, CancellationToken ct) =>
        HandleResult(await _service.AssignTagAsync(id, tagId, ct));

    [Authorize(Roles = StaffRoles)]
    [HttpDelete("{id:guid}/tags/{tagId:guid}")]
    public async Task<IActionResult> RemoveTag(Guid id, Guid tagId, CancellationToken ct) =>
        HandleResult(await _service.RemoveTagAsync(id, tagId, ct));

    /// <summary>Envia un correo a los contactos indicados y/o con la etiqueta indicada,
    /// que tengan correo registrado.</summary>
    [Authorize(Roles = StaffRoles)]
    [HttpPost("mass-email")]
    public async Task<IActionResult> SendMassEmail([FromBody] SendMassEmailRequest request, CancellationToken ct) =>
        HandleResult(await _service.SendMassEmailAsync(request, ct));
}
