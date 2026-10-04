using WebcamStudio.Application.Common;

namespace WebcamStudio.Application.Checklists;

public interface IChecklistService
{
    Task<List<RoomDto>> GetRoomsAsync(CancellationToken ct = default);
    Task<Result<RoomDto>> CreateRoomAsync(CreateRoomRequest request, CancellationToken ct = default);
    Task<Result<ChecklistTemplateItemDto>> AddTemplateItemAsync(
        Guid roomId, CreateChecklistTemplateItemRequest request, CancellationToken ct = default);
    Task<Result<RoomChecklistTemplateDto>> GetRoomTemplateAsync(Guid roomId, CancellationToken ct = default);
    Task<Result<ChecklistRunDto>> SubmitChecklistAsync(
        Guid performedByAccountId, SubmitChecklistRequest request, CancellationToken ct = default);
}
