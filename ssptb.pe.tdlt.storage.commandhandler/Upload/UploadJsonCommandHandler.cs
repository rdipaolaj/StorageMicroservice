using MediatR;
using ssptb.pe.tdlt.storage.command.Upload;
using ssptb.pe.tdlt.storage.common.Responses;
using ssptb.pe.tdlt.storage.data.Repositories.Interfaces;
using ssptb.pe.tdlt.storage.dto.File;
using ssptb.pe.tdlt.storage.entities.Storage;
using ssptb.pe.tdlt.storage.internalservices.Cloudinary;

namespace ssptb.pe.tdlt.storage.commandhandler.Upload;
public class UploadJsonCommandHandler : IRequestHandler<UploadJsonCommand, ApiResponse<FileUploadDataResponse>>
{
    private readonly ICloudinaryService _cloudinaryService;
    private readonly IFileRepository _fileRepository;

    public UploadJsonCommandHandler(ICloudinaryService cloudinaryService, IFileRepository fileRepository)
    {
        _cloudinaryService = cloudinaryService;
        _fileRepository = fileRepository;
    }

    public async Task<ApiResponse<FileUploadDataResponse>> Handle(UploadJsonCommand request, CancellationToken cancellationToken)
    {
        // Validar que los datos del comando no sean nulos o inválidos
        if (string.IsNullOrEmpty(request.FileName) || string.IsNullOrEmpty(request.JsonContent) || string.IsNullOrEmpty(request.UserId))
        {
            return ApiResponseHelper.CreateErrorResponse<FileUploadDataResponse>("FileName, JsonContent, or UserId is missing.", 400);
        }

        var uploadResult = await _cloudinaryService.UploadJsonAsync(request.FileName, request.JsonContent);

        // Si la subida a Cloudinary falla, retornar el error
        if (!uploadResult.Success)
        {
            return ApiResponseHelper.CreateErrorResponse<FileUploadDataResponse>($"Cloudinary upload failed: {uploadResult.Message}", uploadResult.StatusCode);
        }

        var fileId = Guid.NewGuid().ToString();

        // Si la subida fue exitosa, guardamos los metadatos en Couchbase
        var metadata = new FileMetadata
        {
            Id = fileId,
            FileName = request.FileName,
            PublicUrl = uploadResult.Data,  // Usamos la URL pública obtenida del servicio Cloudinary
            UploadedAt = DateTime.UtcNow,
            UploadedBy = request.UserId
        };

        var couchbaseResult = await _fileRepository.SaveFileMetadataAsync(fileId, metadata);

        // Si Couchbase no guarda bien los datos, devolver un error
        if (!couchbaseResult.Success)
        {
            return ApiResponseHelper.CreateErrorResponse<FileUploadDataResponse>($"Failed to save metadata in Couchbase: {couchbaseResult.Message}", 500);
        }

        var fileUploadDataResponse = new FileUploadDataResponse
        {
            FileId = fileId,
            FileName = request.FileName,
            PublicUrl = uploadResult.Data
        };

        // Todo fue exitoso, retornar una respuesta exitosa
        return ApiResponseHelper.CreateSuccessResponse(fileUploadDataResponse, "File uploaded and metadata saved successfully.");
    }
}
