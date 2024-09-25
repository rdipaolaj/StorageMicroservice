using Couchbase.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ssptb.pe.tdlt.storage.common.Responses;
using ssptb.pe.tdlt.storage.data.Helpers;
using ssptb.pe.tdlt.storage.data.Repositories.Interfaces;
using ssptb.pe.tdlt.storage.entities.Storage;

namespace ssptb.pe.tdlt.storage.data.Repositories;
public class FileRepository : IFileRepository
{
    private readonly ICouchbaseHelper _couchbaseHelper;
    private readonly ILogger<FileRepository> _logger;

    public FileRepository(ICouchbaseHelper couchbaseHelper, ILogger<FileRepository> logger)
    {
        _couchbaseHelper = couchbaseHelper;
        _logger = logger;
    }

    public async Task<ApiResponse<bool>> SaveFileMetadataAsync(string id, FileMetadata metadata)
    {
        try
        {
            // Usa CouchbaseHelper para obtener la collection
            var collection = await _couchbaseHelper.GetCollectionAsync("storage_app", "file_metadata");

            // Asigna la versión de esquema actual al documento
            metadata.SchemaVersion = 2;

            // Inserta o actualiza los metadatos en la collection
            await collection.UpsertAsync(id, metadata);

            _logger.LogInformation($"File metadata saved to Couchbase with ID: {id}");

            return ApiResponseHelper.CreateSuccessResponse(true, "File metadata saved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error saving file metadata to Couchbase: {ex.Message}");
            return ApiResponseHelper.CreateErrorResponse<bool>($"Failed to save file metadata: {ex.Message}", 500);
        }
    }

    public async Task<ApiResponse<List<FileMetadata>>> ListFilesByUserAsync(string userId)
    {
        try
        {
            var query = $"SELECT file_metadata.* FROM `travel-sample`.`storage_app`.`file_metadata` WHERE `uploadedBy` = '{userId}' AND file_metadata.schemaVersion = 2";
            var cluster = await _couchbaseHelper.GetClusterAsync(); // Obtenemos el cluster usando CouchbaseHelper
            var result = await cluster.QueryAsync<FileMetadata>(query);

            var files = await result.Rows.ToListAsync();

            if (files.Count == 0)
            {
                return ApiResponseHelper.CreateSuccessResponse(files, "No files found for the specified user.");
            }

            return ApiResponseHelper.CreateSuccessResponse(files, "Files retrieved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error listing files by user from Couchbase: {ex.Message}");
            return ApiResponseHelper.CreateErrorResponse<List<FileMetadata>>($"Failed to list files: {ex.Message}", 500);
        }
    }

    public async Task<ApiResponse<FileMetadata>> GetFileMetadataAsync(string id)
    {
        try
        {
            var collection = await _couchbaseHelper.GetCollectionAsync("storage_app", "file_metadata");
            var result = await collection.GetAsync(id);
            var metadata = result.ContentAs<FileMetadata>();

            // Migrar el documento si está en una versión de esquema anterior
            if (metadata.SchemaVersion < 2)
            {
                metadata = MigrateToLatestVersion(metadata);
                await collection.UpsertAsync(id, metadata);
            }

            _logger.LogInformation($"File metadata with ID {id} retrieved from Couchbase.");

            return ApiResponseHelper.CreateSuccessResponse(metadata, "File metadata retrieved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving file metadata from Couchbase: {ex.Message}");
            return ApiResponseHelper.CreateErrorResponse<FileMetadata>($"Failed to retrieve file metadata: {ex.Message}", 500);
        }
    }

    public async Task<ApiResponse<bool>> DeleteFileAsync(string id)
    {
        try
        {
            var collection = await _couchbaseHelper.GetCollectionAsync("storage_app", "file_metadata");

            await collection.RemoveAsync(id);
            _logger.LogInformation($"File metadata with ID {id} deleted from Couchbase.");

            return ApiResponseHelper.CreateSuccessResponse(true, "File deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting file metadata from Couchbase: {ex.Message}");
            return ApiResponseHelper.CreateErrorResponse<bool>($"Failed to delete file metadata: {ex.Message}", 500);
        }
    }

    public async Task<ApiResponse<List<FileMetadata>>> ListAllFilesAsync()
    {
        try
        {
            var query = "SELECT file_metadata.* FROM `travel-sample`.`storage_app`.`file_metadata` " +
                    "WHERE file_metadata.schemaVersion = 2";
            var cluster = await _couchbaseHelper.GetClusterAsync(); // Obtenemos el cluster usando CouchbaseHelper
            var result = await cluster.QueryAsync<FileMetadata>(query);

            var files = await result.Rows.ToListAsync();
            return ApiResponseHelper.CreateSuccessResponse(files, "Files retrieved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error listing files from Couchbase: {ex.Message}");
            return ApiResponseHelper.CreateErrorResponse<List<FileMetadata>>($"Failed to list files: {ex.Message}", 500);
        }
    }

    public async Task<ApiResponse<bool>> UpdateFileMetadataAsync(string id, FileMetadata metadata)
    {
        try
        {
            var collection = await _couchbaseHelper.GetCollectionAsync("storage_app", "file_metadata");

            await collection.UpsertAsync(id, metadata);
            _logger.LogInformation($"File metadata with ID {id} updated in Couchbase.");

            return ApiResponseHelper.CreateSuccessResponse(true, "File metadata updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating file metadata in Couchbase: {ex.Message}");
            return ApiResponseHelper.CreateErrorResponse<bool>($"Failed to update file metadata: {ex.Message}", 500);
        }
    }

    private FileMetadata MigrateToLatestVersion(FileMetadata oldMetadata)
    {
        // Lógica para actualizar el documento al nuevo esquema
        if (oldMetadata.SchemaVersion == 1)
        {
            oldMetadata.UploadedBy = "unknown"; // Agrega el campo faltante, por ejemplo
            oldMetadata.SchemaVersion = 2;
        }
        return oldMetadata;
    }
}
