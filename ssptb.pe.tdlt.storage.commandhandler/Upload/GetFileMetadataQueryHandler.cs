using MediatR;
using ssptb.pe.tdlt.storage.command.Upload;
using ssptb.pe.tdlt.storage.common.Responses;
using ssptb.pe.tdlt.storage.data.Repositories.Interfaces;
using ssptb.pe.tdlt.storage.entities.Storage;

namespace ssptb.pe.tdlt.storage.commandhandler.Upload;
public class GetFileMetadataQueryHandler : IRequestHandler<GetFileMetadataQuery, ApiResponse<FileMetadata>>
{
    private readonly IFileRepository _fileRepository;

    public GetFileMetadataQueryHandler(IFileRepository fileRepository)
    {
        _fileRepository = fileRepository;
    }

    public async Task<ApiResponse<FileMetadata>> Handle(GetFileMetadataQuery request, CancellationToken cancellationToken)
    {
        return await _fileRepository.GetFileMetadataAsync(request.FileId);
    }
}
