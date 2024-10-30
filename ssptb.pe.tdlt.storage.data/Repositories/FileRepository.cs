using Couchbase.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using ssptb.pe.tdlt.storage.common.Responses;
using ssptb.pe.tdlt.storage.data.Helpers;
using ssptb.pe.tdlt.storage.data.Repositories.Interfaces;
using ssptb.pe.tdlt.storage.entities.Storage;

namespace ssptb.pe.tdlt.storage.data.Repositories;
public class FileRepository : IFileRepository
{
    private readonly IMongoDBHelper _mongoDBHelper;
    private readonly ILogger<FileRepository> _logger;

    public FileRepository(IMongoDBHelper mongoDBHelper, ILogger<FileRepository> logger)
    {
        _mongoDBHelper = mongoDBHelper;
        _logger = logger;
    }

    public async Task<ApiResponse<bool>> SaveFileMetadataAsync(string id, FileMetadata metadata)
    {
        try
        {
            var collection = _mongoDBHelper.GetCollection<FileMetadata>("file_metadata");
            metadata.SchemaVersion = 2;

            var filter = Builders<FileMetadata>.Filter.Eq(f => f.Id, id);
            var options = new ReplaceOptions { IsUpsert = true };

            await collection.ReplaceOneAsync(filter, metadata, options);

            _logger.LogInformation($"File metadata saved to MongoDB with ID: {id}");

            return ApiResponseHelper.CreateSuccessResponse(true, "File metadata saved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error saving file metadata to MongoDB: {ex.Message}");
            return ApiResponseHelper.CreateErrorResponse<bool>($"Failed to save file metadata: {ex.Message}", 500);
        }
    }

    public async Task<ApiResponse<List<FileMetadata>>> ListFilesByUserAsync(string userId)
    {
        try
        {
            var collection = _mongoDBHelper.GetCollection<FileMetadata>("file_metadata");
            var filter = Builders<FileMetadata>.Filter.Eq(f => f.UploadedBy, userId) &
                         Builders<FileMetadata>.Filter.Eq(f => f.SchemaVersion, 2);

            var files = await collection.Find(filter).ToListAsync();

            if (files.Count == 0)
            {
                return ApiResponseHelper.CreateSuccessResponse(files, "No files found for the specified user.");
            }

            return ApiResponseHelper.CreateSuccessResponse(files, "Files retrieved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error listing files by user from MongoDB: {ex.Message}");
            return ApiResponseHelper.CreateErrorResponse<List<FileMetadata>>($"Failed to list files: {ex.Message}", 500);
        }
    }

    public async Task<ApiResponse<FileMetadata>> GetFileMetadataAsync(string id)
    {
        try
        {
            var collection = _mongoDBHelper.GetCollection<FileMetadata>("file_metadata");
            var filter = Builders<FileMetadata>.Filter.Eq(f => f.Id, id);
            var metadata = await collection.Find(filter).FirstOrDefaultAsync();

            if (metadata == null)
            {
                return ApiResponseHelper.CreateErrorResponse<FileMetadata>("File metadata not found.", 404);
            }

            // Migrar el documento si está en una versión de esquema anterior
            if (metadata.SchemaVersion < 2)
            {
                metadata = MigrateToLatestVersion(metadata);
                await collection.ReplaceOneAsync(filter, metadata);
            }

            _logger.LogInformation($"File metadata with ID {id} retrieved from MongoDB.");

            return ApiResponseHelper.CreateSuccessResponse(metadata, "File metadata retrieved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving file metadata from MongoDB: {ex.Message}");
            return ApiResponseHelper.CreateErrorResponse<FileMetadata>($"Failed to retrieve file metadata: {ex.Message}", 500);
        }
    }

    public async Task<ApiResponse<bool>> DeleteFileAsync(string id)
    {
        try
        {
            var collection = _mongoDBHelper.GetCollection<FileMetadata>("file_metadata");
            var filter = Builders<FileMetadata>.Filter.Eq(f => f.Id, id);
            var result = await collection.DeleteOneAsync(filter);

            if (result.DeletedCount == 0)
            {
                return ApiResponseHelper.CreateErrorResponse<bool>("File metadata not found.", 404);
            }

            _logger.LogInformation($"File metadata with ID {id} deleted from MongoDB.");

            return ApiResponseHelper.CreateSuccessResponse(true, "File deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting file metadata from MongoDB: {ex.Message}");
            return ApiResponseHelper.CreateErrorResponse<bool>($"Failed to delete file metadata: {ex.Message}", 500);
        }
    }

    public async Task<ApiResponse<List<FileMetadata>>> ListAllFilesAsync()
    {
        try
        {
            var collection = _mongoDBHelper.GetCollection<FileMetadata>("file_metadata");
            var filter = Builders<FileMetadata>.Filter.Eq(f => f.SchemaVersion, 2);
            var files = await collection.Find(filter).ToListAsync();

            return ApiResponseHelper.CreateSuccessResponse(files, "Files retrieved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error listing files from MongoDB: {ex.Message}");
            return ApiResponseHelper.CreateErrorResponse<List<FileMetadata>>($"Failed to list files: {ex.Message}", 500);
        }
    }

    public async Task<ApiResponse<bool>> UpdateFileMetadataAsync(string id, FileMetadata metadata)
    {
        try
        {
            var collection = _mongoDBHelper.GetCollection<FileMetadata>("file_metadata");
            var filter = Builders<FileMetadata>.Filter.Eq(f => f.Id, id);
            var options = new ReplaceOptions { IsUpsert = false };
            var result = await collection.ReplaceOneAsync(filter, metadata, options);

            if (result.MatchedCount == 0)
            {
                return ApiResponseHelper.CreateErrorResponse<bool>("File metadata not found.", 404);
            }

            _logger.LogInformation($"File metadata with ID {id} updated in MongoDB.");

            return ApiResponseHelper.CreateSuccessResponse(true, "File metadata updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating file metadata in MongoDB: {ex.Message}");
            return ApiResponseHelper.CreateErrorResponse<bool>($"Failed to update file metadata: {ex.Message}", 500);
        }
    }

    private FileMetadata MigrateToLatestVersion(FileMetadata oldMetadata)
    {
        if (oldMetadata.SchemaVersion == 1)
        {
            oldMetadata.UploadedBy = "unknown"; // Agrega el campo faltante, por ejemplo
            oldMetadata.SchemaVersion = 2;
        }
        return oldMetadata;
    }
}
