using ssptb.pe.tdlt.storage.common.Responses;
using ssptb.pe.tdlt.storage.entities.Storage;

namespace ssptb.pe.tdlt.storage.data.Repositories.Interfaces;
public interface IFileRepository
{
    Task<ApiResponse<bool>> SaveFileMetadataAsync(string id, FileMetadata metadata);
    Task<ApiResponse<FileMetadata>> GetFileMetadataAsync(string id);  // Agregar para obtener metadata
    Task<ApiResponse<bool>> DeleteFileAsync(string id);  // Agregar para eliminar archivo
    Task<ApiResponse<List<FileMetadata>>> ListAllFilesAsync();  // Agregar para listar archivos
    Task<ApiResponse<bool>> UpdateFileMetadataAsync(string id, FileMetadata metadata);  // Agregar para actualizar
    Task<ApiResponse<List<FileMetadata>>> ListFilesByUserAsync(string userId);  // Agregar para listar archivos por usuario
}
