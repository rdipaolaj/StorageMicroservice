using MediatR;
using ssptb.pe.tdlt.storage.command.Upload;
using ssptb.pe.tdlt.storage.common.Responses;
using ssptb.pe.tdlt.storage.data.Repositories.Interfaces;
using ssptb.pe.tdlt.storage.entities.Storage;
using ssptb.pe.tdlt.storage.internalservices.Cloudinary;

namespace ssptb.pe.tdlt.storage.commandhandler.Upload;
public class UpdateFileMetadataCommandHandler : IRequestHandler<UpdateFileMetadataCommand, ApiResponse<bool>>
{
    private readonly IFileRepository _fileRepository;
    private readonly ICloudinaryService _cloudinaryService;

    public UpdateFileMetadataCommandHandler(IFileRepository fileRepository, ICloudinaryService cloudinaryService)
    {
        _fileRepository = fileRepository;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<ApiResponse<bool>> Handle(UpdateFileMetadataCommand request, CancellationToken cancellationToken)
    {
        // 1. Obtener los metadatos del archivo actual para eliminarlo de Cloudinary si es necesario
        var fileMetadataResponse = await _fileRepository.GetFileMetadataAsync(request.FileId);

        if (!fileMetadataResponse.Success || fileMetadataResponse.Data == null)
        {
            return ApiResponseHelper.CreateErrorResponse<bool>("File not found in the database.", 404);
        }

        var fileMetadata = fileMetadataResponse.Data;

        // 2. Extraer el PublicId desde el PublicUrl
        var publicId = ExtractPublicIdFromUrl(fileMetadata.PublicUrl);

        // 3. Eliminar el archivo anterior de Cloudinary si existe
        var cloudinaryDeleteResponse = await _cloudinaryService.DeleteFileFromCloudinaryAsync(publicId);

        if (!cloudinaryDeleteResponse.Success)
        {
            return cloudinaryDeleteResponse; // Retornar error si la eliminación en Cloudinary falla
        }

        // 4. Subir el nuevo archivo a Cloudinary
        var uploadResponse = await _cloudinaryService.UploadJsonAsync(request.FileName, request.JsonContent);

        if (!uploadResponse.Success)
        {
            return ApiResponseHelper.CreateErrorResponse<bool>("Failed to upload new file to Cloudinary.", 500);
        }

        // 5. Actualizar los metadatos del archivo en Couchbase
        var metadata = new FileMetadata
        {
            FileName = request.FileName,
            PublicUrl = uploadResponse.Data,
            UploadedAt = DateTime.UtcNow,
            SchemaVersion = 2
        };

        return await _fileRepository.UpdateFileMetadataAsync(request.FileId, metadata);
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
