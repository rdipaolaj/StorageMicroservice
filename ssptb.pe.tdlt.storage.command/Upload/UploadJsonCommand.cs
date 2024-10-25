using MediatR;
using ssptb.pe.tdlt.storage.common.Responses;
using ssptb.pe.tdlt.storage.dto.File;

namespace ssptb.pe.tdlt.storage.command.Upload;
public class UploadJsonCommand : IRequest<ApiResponse<FileUploadDataResponse>>
{
    public string FileName { get; set; } = string.Empty;
    public string JsonContent { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
}