using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ssptb.pe.tdlt.storage.common.Responses;
using ssptb.pe.tdlt.storage.common.Settings;
using ssptb.pe.tdlt.storage.data.Repositories.Interfaces;
using ssptb.pe.tdlt.storage.internalservices.Helpers.Interfaces;
using System.Net;
using System.Text;

namespace ssptb.pe.tdlt.storage.internalservices.Cloudinary;
public class CloudinaryService : ICloudinaryService
{
    private readonly CloudinarySettings _cloudinarySecrets;
    private readonly ILogger<CloudinaryService> _logger;
    private readonly IJsonValidator _jsonValidator;
    private readonly IFileNameGenerator _fileNameGenerator;
    private readonly IFileRepository _fileRepository;

    public CloudinaryService(
        IOptions<CloudinarySettings> cloudinarySecrets,
        ILogger<CloudinaryService> logger,
        IJsonValidator jsonValidator,
        IFileNameGenerator fileNameGenerator,
        IFileRepository fileRepository)
    {
        _cloudinarySecrets = cloudinarySecrets.Value;
        _logger = logger;
        _jsonValidator = jsonValidator;
        _fileNameGenerator = fileNameGenerator;
        _fileRepository = fileRepository;
    }
    public async Task<ApiResponse<string>> UploadJsonAsync(string fileName, string jsonContent)
    {
        _logger.LogInformation("Uploading JSON content to Cloudinary");

        // Verificar la configuración de Cloudinary
        if (!IsCloudinaryConfigured())
        {
            return ApiResponseHelper.CreateErrorResponse<string>("Cloudinary configuration is missing.", 500);
        }

        // Validar el formato del JSON
        if (!_jsonValidator.IsValidJson(jsonContent))
        {
            _logger.LogError("Invalid JSON format");
            return ApiResponseHelper.CreateErrorResponse<string>("Invalid JSON format.", 400);
        }

        // Generar el nombre del archivo
        string finalFileName = _fileNameGenerator.GenerateFileName(fileName);

        // Subir el archivo a Cloudinary
        var uploadResult = await UploadFileToCloudinary(finalFileName, jsonContent);

        // Retornar la respuesta basada en el resultado de la subida
        return ProcessUploadResult(uploadResult);
    }

    public async Task<ApiResponse<string>> DownloadFileAsync(string fileId)
    {
        _logger.LogInformation($"Downloading file with ID {fileId} from Cloudinary.");

        // Supongamos que la URL pública está almacenada en Couchbase
        var fileMetadataResponse = await _fileRepository.GetFileMetadataAsync(fileId);

        if (!fileMetadataResponse.Success || fileMetadataResponse.Data == null)
        {
            _logger.LogError($"File metadata not found for file ID {fileId}");
            return ApiResponseHelper.CreateErrorResponse<string>("File metadata not found.", 404);
        }

        var fileMetadata = fileMetadataResponse.Data;

        // Recuperar la URL pública que ya está almacenada en la base de datos
        var url = fileMetadata.PublicUrl;

        return ApiResponseHelper.CreateSuccessResponse(url, "File URL retrieved successfully.");
    }

    public async Task<ApiResponse<bool>> DeleteFileFromCloudinaryAsync(string publicId)
    {
        _logger.LogInformation($"Deleting file with PublicId {publicId} from Cloudinary.");

        if (!IsCloudinaryConfigured())
        {
            return ApiResponseHelper.CreateErrorResponse<bool>("Cloudinary configuration is missing.", 500);
        }

        var cloudinaryAccount = new Account(_cloudinarySecrets.CloudName, _cloudinarySecrets.ApiKey, _cloudinarySecrets.ApiSecret);
        var cloudinaryInstance = new CloudinaryDotNet.Cloudinary(cloudinaryAccount);

        var deletionParams = new DeletionParams(publicId);
        var deletionResult = await cloudinaryInstance.DestroyAsync(deletionParams);

        if (deletionResult.Result == "ok")
        {
            _logger.LogInformation($"File with PublicId {publicId} deleted successfully from Cloudinary.");
            return ApiResponseHelper.CreateSuccessResponse(true, "File deleted successfully from Cloudinary.");
        }

        _logger.LogError($"Failed to delete file with PublicId {publicId} from Cloudinary.");
        return ApiResponseHelper.CreateErrorResponse<bool>("Failed to delete file from Cloudinary.", 500);
    }

    // Método para verificar si la configuración de Cloudinary es válida
    private bool IsCloudinaryConfigured()
    {
        if (string.IsNullOrWhiteSpace(_cloudinarySecrets.CloudName) ||
            string.IsNullOrWhiteSpace(_cloudinarySecrets.ApiKey) ||
            string.IsNullOrWhiteSpace(_cloudinarySecrets.ApiSecret))
        {
            _logger.LogError("Cloudinary configuration is missing");
            return false;
        }
        return true;
    }

    // Método privado para manejar la subida del archivo a Cloudinary
    private async Task<RawUploadResult> UploadFileToCloudinary(string fileName, string jsonContent)
    {
        var cloudinaryAccount = new Account(_cloudinarySecrets.CloudName, _cloudinarySecrets.ApiKey, _cloudinarySecrets.ApiSecret);
        var cloudinaryInstance = new CloudinaryDotNet.Cloudinary(cloudinaryAccount);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonContent));
        var uploadParams = new RawUploadParams
        {
            File = new FileDescription(fileName, stream),
            PublicId = fileName
        };

        return await cloudinaryInstance.UploadAsync(uploadParams);
    }

    // Método para procesar el resultado de la subida y devolver la URL pública
    private ApiResponse<string> ProcessUploadResult(RawUploadResult uploadResult)
    {
        if (uploadResult.StatusCode == HttpStatusCode.OK)
        {
            var publicUrl = uploadResult.Url?.ToString() ?? string.Empty;  // Obtenemos la URL pública del resultado
            return ApiResponseHelper.CreateSuccessResponse(publicUrl, "JSON file uploaded successfully.");
        }
        else
        {
            return ApiResponseHelper.CreateErrorResponse<string>("Failed to upload JSON file to Cloudinary.", (int)uploadResult.StatusCode);
        }
    }

    
}
