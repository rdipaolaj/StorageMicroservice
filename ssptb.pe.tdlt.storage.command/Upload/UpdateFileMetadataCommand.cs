using MediatR;
using ssptb.pe.tdlt.storage.common.Responses;

namespace ssptb.pe.tdlt.storage.command.Upload;
public class UpdateFileMetadataCommand : IRequest<ApiResponse<bool>>
{
    public string FileId { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string PublicUrl { get; set; } = string.Empty;
    public string JsonContent { get; set; } = string.Empty;
}
