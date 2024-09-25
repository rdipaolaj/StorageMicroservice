using MediatR;
using ssptb.pe.tdlt.storage.command.Upload;
using ssptb.pe.tdlt.storage.common.Responses;
using ssptb.pe.tdlt.storage.internalservices.Cloudinary;

namespace ssptb.pe.tdlt.storage.commandhandler.Upload;
public class DownloadFileQueryHandler : IRequestHandler<DownloadFileQuery, ApiResponse<string>>
{
    private readonly ICloudinaryService _cloudinaryService;

    public DownloadFileQueryHandler(ICloudinaryService cloudinaryService)
    {
        _cloudinaryService = cloudinaryService;
    }

    public async Task<ApiResponse<string>> Handle(DownloadFileQuery request, CancellationToken cancellationToken)
    {
        return await _cloudinaryService.DownloadFileAsync(request.FileId);
    }
}
