using MediatR;
using ssptb.pe.tdlt.storage.command.Upload;
using ssptb.pe.tdlt.storage.common.Responses;
using ssptb.pe.tdlt.storage.data.Repositories.Interfaces;
using ssptb.pe.tdlt.storage.internalservices.Cloudinary;

namespace ssptb.pe.tdlt.storage.commandhandler.Upload;
public class DeleteFileCommandHandler : IRequestHandler<DeleteFileCommand, ApiResponse<bool>>
{
    private readonly IFileRepository _fileRepository;
    private readonly ICloudinaryService _cloudinaryService;

    public DeleteFileCommandHandler(IFileRepository fileRepository, ICloudinaryService cloudinaryService)
    {
        _fileRepository = fileRepository;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<ApiResponse<bool>> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
    {
        // 1. Obtener los metadatos del archivo para obtener el PublicUrl de Cloudinary
        var fileMetadataResponse = await _fileRepository.GetFileMetadataAsync(request.FileId);

        if (!fileMetadataResponse.Success || fileMetadataResponse.Data == null)
        {
            return ApiResponseHelper.CreateErrorResponse<bool>("File not found in the database.", 404);
        }

        var fileMetadata = fileMetadataResponse.Data;

        // 2. Extraer el PublicId desde el PublicUrl
        var publicId = ExtractPublicIdFromUrl(fileMetadata.PublicUrl);

        // 3. Eliminar el archivo de Cloudinary usando el PublicId extraído
        var cloudinaryDeleteResponse = await _cloudinaryService.DeleteFileFromCloudinaryAsync(publicId);

        if (!cloudinaryDeleteResponse.Success)
        {
            return cloudinaryDeleteResponse; // Retornar error si la eliminación en Cloudinary falla
        }

        // 4. Eliminar el archivo de Couchbase
        return await _fileRepository.DeleteFileAsync(request.FileId);
    }
    private string ExtractPublicIdFromUrl(string publicUrl)
    {
        var uri = new Uri(publicUrl);
        var segments = uri.Segments;
        var publicIdWithExtension = segments.Last();
        var publicId = Path.GetFileNameWithoutExtension(publicIdWithExtension);
        return publicId;
    }
}
