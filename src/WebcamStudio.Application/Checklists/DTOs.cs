using System.ComponentModel.DataAnnotations;
using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Application.Checklists;

public record RoomDto(Guid Id, string Name, string? Description, bool IsActive);

public record CreateRoomRequest([Required, MaxLength(200)] string Name, [MaxLength(1000)] string? Description);

public record CreateChecklistTemplateItemRequest(
    [Required, MaxLength(200)] string Name,
    [MaxLength(1000)] string? Description,
    [Range(0, 10_000)] int DisplayOrder);

public record ChecklistTemplateItemDto(Guid Id, string Name, string? Description, int DisplayOrder);


public record RoomChecklistTemplateDto(RoomDto Room, List<ChecklistTemplateItemDto> Items);

public record SubmitChecklistItemResultRequest(
    Guid TemplateItemId,
    ChecklistItemStatus Status,
    [MaxLength(2000)] string? Observation,
    string? AttachmentFileName,
    string? AttachmentContentType);

public record SubmitChecklistRequest(
    Guid RoomId,
    [Required, MinLength(1)] List<SubmitChecklistItemResultRequest> Items,
    [MaxLength(2000)] string? AvailableMaterialsNotes,
    string? MaterialsAttachmentFileName,
    string? MaterialsAttachmentContentType);

public record ChecklistItemResultDto(
    Guid TemplateItemId,
    string TemplateItemName,
    ChecklistItemStatus Status,
    string? Observation,
    string? AttachmentFileName);

public record MaintenanceRequestDto(Guid Id, string Description, MaintenanceRequestStatus Status, DateTime CreatedAt);

public record ChecklistRunDto(
    Guid Id,
    Guid RoomId,
    string RoomName,
    Guid PerformedByAccountId,
    DateTime PerformedAt,
    string? AvailableMaterialsNotes,
    string? MaterialsAttachmentFileName,
    List<ChecklistItemResultDto> Items,
    List<MaintenanceRequestDto> MaintenanceRequests);

public record UploadAttachmentResponse(string FileName);
