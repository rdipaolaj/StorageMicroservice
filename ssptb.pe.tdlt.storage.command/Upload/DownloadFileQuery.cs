using MediatR;
using ssptb.pe.tdlt.storage.common.Responses;

namespace ssptb.pe.tdlt.storage.command.Upload;
public class DownloadFileQuery : IRequest<ApiResponse<string>>
{
    public string FileId { get; set; } = string.Empty;
}
