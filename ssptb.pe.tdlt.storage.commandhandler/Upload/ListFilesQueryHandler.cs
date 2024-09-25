using MediatR;
using ssptb.pe.tdlt.storage.command.Upload;
using ssptb.pe.tdlt.storage.common.Responses;
using ssptb.pe.tdlt.storage.data.Repositories.Interfaces;
using ssptb.pe.tdlt.storage.entities.Storage;

namespace ssptb.pe.tdlt.storage.commandhandler.Upload;
public class ListFilesQueryHandler : IRequestHandler<ListFilesQuery, ApiResponse<List<FileMetadata>>>
{
    private readonly IFileRepository _fileRepository;

    public ListFilesQueryHandler(IFileRepository fileRepository)
    {
        _fileRepository = fileRepository;
    }

    public async Task<ApiResponse<List<FileMetadata>>> Handle(ListFilesQuery request, CancellationToken cancellationToken)
    {
        return await _fileRepository.ListAllFilesAsync();
    }
}
