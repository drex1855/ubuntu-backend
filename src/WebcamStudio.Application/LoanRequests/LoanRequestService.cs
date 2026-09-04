using WebcamStudio.Application.Common;
using WebcamStudio.Application.Interfaces.Persistence;
using WebcamStudio.Application.Interfaces.Services;
using WebcamStudio.Domain.Entities;
using WebcamStudio.Domain.Enums;

namespace WebcamStudio.Application.LoanRequests;

/// <summary>
/// Modulo "Prestamos": una modelo o monitor solicita un prestamo, el dueno recibe un
/// correo de aviso, y un Admin aprueba o rechaza la solicitud desde el historial.
/// </summary>
public class LoanRequestService : ILoanRequestService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditService _auditService;
    private readonly IEmailSender _emailSender;
    private readonly IWhatsAppNotifier _whatsAppNotifier;
    private readonly OwnerSettings _ownerSettings;

    public LoanRequestService(
        IUnitOfWork unitOfWork,
        IAuditService auditService,
        IEmailSender emailSender,
        IWhatsAppNotifier whatsAppNotifier,
        OwnerSettings ownerSettings)
    {
        _unitOfWork = unitOfWork;
        _auditService = auditService;
        _emailSender = emailSender;
        _whatsAppNotifier = whatsAppNotifier;
        _ownerSettings = ownerSettings;
    }

    public async Task<Result<LoanRequestDto>> CreateAsync(
        Guid requestedByAccountId, CreateLoanRequestRequest request, CancellationToken ct = default)
    {
        if (request.Amount <= 0)
            return Result<LoanRequestDto>.Failure("El monto debe ser mayor a cero.");

        if (string.IsNullOrWhiteSpace(request.Reason))
            return Result<LoanRequestDto>.Failure("El motivo del prestamo es obligatorio.");

        var account = await _unitOfWork.ModelAccounts.GetByIdAsync(requestedByAccountId, ct);
        if (account is null)
            return Result<LoanRequestDto>.Failure("Cuenta no encontrada.");

        var loanRequest = new LoanRequest
        {
            RequestedByAccountId = account.Id,
            Amount = request.Amount,
            Reason = request.Reason.Trim(),
            Status = LoanRequestStatus.Pendiente,
            RequestedAt = DateTime.UtcNow
        };

        await _unitOfWork.LoanRequests.AddAsync(loanRequest, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        await _auditService.LogAsync("Prestamos", "SolicitudCreada", nameof(LoanRequest), loanRequest.Id,
            new { request.Amount }, ct);

        // IEmailSender/IWhatsAppNotifier nunca lanzan excepcion (ver SmtpEmailSender /
        // WhatsAppCloudApiNotifier): si el envio falla o las credenciales todavia no
        // estan configuradas, la solicitud ya quedo guardada igual.
        var subject = $"Nueva solicitud de prestamo - {account.FullName}";
        var body =
            $"Se registro una nueva solicitud de prestamo.\n\n" +
            $"Solicitante: {account.FullName}\n" +
            $"Monto: {loanRequest.Amount:C}\n" +
            $"Motivo: {loanRequest.Reason}\n" +
            $"Fecha: {loanRequest.RequestedAt:dd/MM/yyyy HH:mm} UTC";
        await _emailSender.SendAsync(_ownerSettings.Email, subject, body, ct);
        await _whatsAppNotifier.SendAsync(_ownerSettings.WhatsAppNumber, body, ct);

        return Result<LoanRequestDto>.Success(ToDto(loanRequest, account.FullName));
    }

    public async Task<List<LoanRequestDto>> SearchAsync(LoanRequestStatus? status, CancellationToken ct = default)
    {
        var requests = await _unitOfWork.LoanRequests.SearchAsync(status, ct);
        return requests.Select(r => ToDto(r, r.RequestedByAccount.FullName)).ToList();
    }

    public async Task<Result<LoanRequestDto>> SetStatusAsync(
        Guid id, UpdateLoanRequestStatusRequest request, CancellationToken ct = default)
    {
        var loanRequest = await _unitOfWork.LoanRequests.GetByIdAsync(id, ct);
        if (loanRequest is null)
            return Result<LoanRequestDto>.Failure("Solicitud no encontrada.");

        var account = await _unitOfWork.ModelAccounts.GetByIdAsync(loanRequest.RequestedByAccountId, ct);

        loanRequest.Status = request.Status;
        loanRequest.ResolvedAt = DateTime.UtcNow;

        _unitOfWork.LoanRequests.Update(loanRequest);
        await _unitOfWork.SaveChangesAsync(ct);

        await _auditService.LogAsync("Prestamos", $"EstadoCambiadoA{request.Status}", nameof(LoanRequest),
            loanRequest.Id, null, ct);

        // Notificar a quien pidio el prestamo el resultado de la decision -- mismo criterio
        // de "mejor esfuerzo" que el aviso al dueno: si la cuenta no tiene telefono
        // registrado, el WhatsApp simplemente se omite sin marcar error.
        if (account is not null)
        {
            var decisionText = request.Status == LoanRequestStatus.Aprobada ? "aprobada" : "rechazada";
            var message = $"Tu solicitud de prestamo por {loanRequest.Amount:C} fue {decisionText}.";
            await _emailSender.SendAsync(account.Email, $"Tu solicitud de prestamo fue {decisionText}", message, ct);
            if (!string.IsNullOrWhiteSpace(account.PhoneNumber))
                await _whatsAppNotifier.SendAsync(account.PhoneNumber, message, ct);
        }

        return Result<LoanRequestDto>.Success(ToDto(loanRequest, account?.FullName ?? string.Empty));
    }

    private static LoanRequestDto ToDto(LoanRequest r, string requestedByFullName) => new(
        r.Id, r.RequestedByAccountId, requestedByFullName, r.Amount, r.Reason, r.Status, r.RequestedAt, r.ResolvedAt);
}
