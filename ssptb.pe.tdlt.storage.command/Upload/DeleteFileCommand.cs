using MediatR;
using ssptb.pe.tdlt.storage.common.Responses;

namespace ssptb.pe.tdlt.storage.command.Upload;
public class DeleteFileCommand : IRequest<ApiResponse<bool>>
{
    public string FileId { get; set; } = string.Empty;
}
