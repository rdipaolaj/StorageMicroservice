using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ssptb.pe.tdlt.storage.api.Configuration;
using ssptb.pe.tdlt.storage.command.Upload;
using System.Text;

namespace ssptb.pe.tdlt.storage.api.Controllers;

[ApiVersion(1)]
[ApiController]
[Route("ssptbpetdlt/storage/api/v{v:apiVersion}/[controller]")]
public class StorageController : CustomController
{
    private readonly ILogger<StorageController> _logger;
    private readonly IMediator _mediator;

    public StorageController(IMediator mediator, ILogger<StorageController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    [Route("upload-json")]
    public async Task<IActionResult> UploadJson([FromBody] UploadJsonCommand command)
    {
        var result = await _mediator.Send(command);
        return OkorBadRequestValidationApiResponse(result);
    }

    [HttpGet]
    [Route("file/{id}")]
    public async Task<IActionResult> GetFileMetadata(string id)
    {
        var query = new GetFileMetadataQuery { FileId = id };
        var result = await _mediator.Send(query);
        return OkorBadRequestValidationApiResponse(result);
    }

    [HttpGet]
    [Route("files")]
    public async Task<IActionResult> ListFiles([FromQuery] ListFilesQuery query)
    {
        var result = await _mediator.Send(query);
        return OkorBadRequestValidationApiResponse(result);
    }

    [HttpGet]
    [Route("files/user/{userId}")]
    public async Task<IActionResult> ListFilesByUser(string userId)
    {
        var query = new ListFilesByUserQuery { UserId = userId };
        var result = await _mediator.Send(query);
        return OkorBadRequestValidationApiResponse(result);
    }

    [HttpPut]
    [Route("file/{id}")]
    public async Task<IActionResult> UpdateFileMetadata(string id, [FromBody] UpdateFileMetadataCommand command)
    {
        command.FileId = id;
        var result = await _mediator.Send(command);
        return OkorBadRequestValidationApiResponse(result);
    }

    [HttpDelete]
    [Route("file/{id}")]
    public async Task<IActionResult> DeleteFile(string id)
    {
        var command = new DeleteFileCommand { FileId = id };
        var result = await _mediator.Send(command);
        return OkorBadRequestValidationApiResponse(result);
    }

    [HttpGet]
    [Route("file/{id}/download")]
    public async Task<IActionResult> DownloadFile(string id)
    {
        var query = new DownloadFileQuery { FileId = id };
        var result = await _mediator.Send(query);
        //if (result.Success)
        //{
        //    return File(Encoding.UTF8.GetBytes(result.Data), "application/json", $"{id}.json");
        //}
        return OkorBadRequestValidationApiResponse(result);
    }
}
