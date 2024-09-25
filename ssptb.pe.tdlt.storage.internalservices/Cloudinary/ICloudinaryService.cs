using ssptb.pe.tdlt.storage.common.Responses;

namespace ssptb.pe.tdlt.storage.internalservices.Cloudinary;
public interface ICloudinaryService
{
    Task<ApiResponse<string>> UploadJsonAsync(string fileName, string jsonContent);
    Task<ApiResponse<string>> DownloadFileAsync(string fileId);
    Task<ApiResponse<bool>> DeleteFileFromCloudinaryAsync(string publicId);
}
