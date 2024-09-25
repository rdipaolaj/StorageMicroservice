using MediatR;
using ssptb.pe.tdlt.storage.common.Responses;

namespace ssptb.pe.tdlt.storage.command.Upload;
public class UploadJsonCommand : IRequest<ApiResponse<bool>>
{
    public string FileName { get; set; } = string.Empty;
    public string JsonContent { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
}