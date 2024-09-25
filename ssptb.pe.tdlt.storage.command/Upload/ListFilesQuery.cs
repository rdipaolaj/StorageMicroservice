using MediatR;
using ssptb.pe.tdlt.storage.common.Responses;
using ssptb.pe.tdlt.storage.entities.Storage;

namespace ssptb.pe.tdlt.storage.command.Upload;
public class ListFilesQuery : IRequest<ApiResponse<List<FileMetadata>>>
{
}
