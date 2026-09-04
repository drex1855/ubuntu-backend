using WebcamStudio.Application.Common;
using WebcamStudio.Application.Interfaces.Persistence;
using WebcamStudio.Application.Interfaces.Services;
using WebcamStudio.Domain.Entities;
using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Application.Checklists;

/// <summary>
/// Implementa el modulo "Checklist de habitaciones" (F) del diagrama:
/// F1 Seleccionar habitacion -> F2 Cargar checklist -> F3 Revisar estado ->
/// F4 Elemento en buen estado? -> F5 Marcar bueno / F6-F7-F8 Marcar malo + observacion +
/// solicitud de mantenimiento -> F9 Registrar materiales disponibles -> F10 Guardar checklist -> Z.
/// </summary>
public class ChecklistService : IChecklistService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditService _auditService;

    public ChecklistService(IUnitOfWork unitOfWork, IAuditService auditService)
    {
        _unitOfWork = unitOfWork;
        _auditService = auditService;
    }

    public async Task<List<RoomDto>> GetRoomsAsync(CancellationToken ct = default)
    {
        var rooms = await _unitOfWork.Rooms.GetActiveAsync(ct);
        return rooms.Select(r => new RoomDto(r.Id, r.Name, r.Description, r.IsActive)).ToList();
    }

    public async Task<Result<RoomDto>> CreateRoomAsync(CreateRoomRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return Result<RoomDto>.Failure("El nombre de la habitacion es obligatorio.");

        var room = new Room { Name = request.Name, Description = request.Description, IsActive = true };
        await _unitOfWork.Rooms.AddAsync(room, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<RoomDto>.Success(new RoomDto(room.Id, room.Name, room.Description, room.IsActive));
    }

    public async Task<Result<ChecklistTemplateItemDto>> AddTemplateItemAsync(
        Guid roomId, CreateChecklistTemplateItemRequest request, CancellationToken ct = default)
    {
        var room = await _unitOfWork.Rooms.GetByIdAsync(roomId, ct);
        if (room is null)
            return Result<ChecklistTemplateItemDto>.Failure("Habitacion no encontrada.");

        var item = new ChecklistTemplateItem
        {
            RoomId = room.Id,
            Name = request.Name,
            Description = request.Description,
            DisplayOrder = request.DisplayOrder,
            IsActive = true
        };
        await _unitOfWork.ChecklistTemplateItems.AddAsync(item, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<ChecklistTemplateItemDto>.Success(
            new ChecklistTemplateItemDto(item.Id, item.Name, item.Description, item.DisplayOrder));
    }

    public async Task<Result<RoomChecklistTemplateDto>> GetRoomTemplateAsync(Guid roomId, CancellationToken ct = default)
    {
        // F1 + F2: Seleccionar habitacion / Cargar checklist.
        var room = await _unitOfWork.Rooms.GetWithTemplateItemsAsync(roomId, ct);
        if (room is null)
            return Result<RoomChecklistTemplateDto>.Failure("Habitacion no encontrada.");

        var items = room.TemplateItems
            .Where(i => i.IsActive)
            .OrderBy(i => i.DisplayOrder)
            .Select(i => new ChecklistTemplateItemDto(i.Id, i.Name, i.Description, i.DisplayOrder))
            .ToList();

        return Result<RoomChecklistTemplateDto>.Success(
            new RoomChecklistTemplateDto(new RoomDto(room.Id, room.Name, room.Description, room.IsActive), items));
    }

    public async Task<Result<ChecklistRunDto>> SubmitChecklistAsync(
        Guid performedByAccountId, SubmitChecklistRequest request, CancellationToken ct = default)
    {
        if (request.Items.Count == 0)
            return Result<ChecklistRunDto>.Failure("El checklist debe incluir al menos un elemento revisado.");

        var room = await _unitOfWork.Rooms.GetWithTemplateItemsAsync(request.RoomId, ct);
        if (room is null)
            return Result<ChecklistRunDto>.Failure("Habitacion no encontrada.");

        var validTemplateItemIds = room.TemplateItems.Select(i => i.Id).ToHashSet();
        foreach (var item in request.Items)
        {
            if (!validTemplateItemIds.Contains(item.TemplateItemId))
                return Result<ChecklistRunDto>.Failure($"El elemento {item.TemplateItemId} no pertenece a esta habitacion.");
        }

        var run = new ChecklistRun
        {
            RoomId = room.Id,
            PerformedByAccountId = performedByAccountId,
            PerformedAt = DateTime.UtcNow,
            // F9: Registrar materiales disponibles.
            AvailableMaterialsNotes = request.AvailableMaterialsNotes,
            MaterialsAttachmentFileName = request.MaterialsAttachmentFileName,
            MaterialsAttachmentContentType = request.MaterialsAttachmentContentType
        };

        foreach (var itemRequest in request.Items)
        {
            // F3: Revisar estado de equipos y materiales.
            var result = new ChecklistItemResult
            {
                ChecklistRun = run,
                TemplateItemId = itemRequest.TemplateItemId,
                Status = itemRequest.Status,
                Observation = itemRequest.Observation,
                AttachmentFileName = itemRequest.AttachmentFileName,
                AttachmentContentType = itemRequest.AttachmentContentType
            };
            run.ItemResults.Add(result);

            // F4: Elemento en buen estado?
            if (itemRequest.Status == ChecklistItemStatus.Malo)
            {
                // F6 + F7: Marcar como malo + registrar observacion (ya esta en el result).
                // F8: Crear solicitud de mantenimiento.
                var templateItem = room.TemplateItems.First(i => i.Id == itemRequest.TemplateItemId);
                run.MaintenanceRequests.Add(new MaintenanceRequest
                {
                    ChecklistRun = run,
                    ChecklistItemResult = result,
                    RoomId = room.Id,
                    Description = $"Elemento '{templateItem.Name}' marcado como malo." +
                                  (string.IsNullOrWhiteSpace(itemRequest.Observation) ? "" : $" Observacion: {itemRequest.Observation}"),
                    Status = MaintenanceRequestStatus.Pendiente
                });
            }
            // F5: Marcar como bueno -> no requiere accion adicional, el Status ya lo refleja.
        }

        // F10: Guardar checklist.
        await _unitOfWork.ChecklistRuns.AddAsync(run, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        // Z: Registrar auditoria.
        await _auditService.LogAsync("Checklist", "ChecklistGuardado", nameof(ChecklistRun), run.Id,
            new { room.Id, MaintenanceRequestsCreated = run.MaintenanceRequests.Count }, ct);

        return Result<ChecklistRunDto>.Success(ToDto(run, room.Name));
    }

    private static ChecklistRunDto ToDto(ChecklistRun run, string roomName) => new(
        run.Id,
        run.RoomId,
        roomName,
        run.PerformedByAccountId,
        run.PerformedAt,
        run.AvailableMaterialsNotes,
        run.MaterialsAttachmentFileName,
        run.ItemResults.Select(r => new ChecklistItemResultDto(
            r.TemplateItemId, r.TemplateItem?.Name ?? string.Empty, r.Status, r.Observation, r.AttachmentFileName)).ToList(),
        run.MaintenanceRequests.Select(m => new MaintenanceRequestDto(m.Id, m.Description, m.Status, m.CreatedAt)).ToList());
}
