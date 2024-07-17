using AttachmentApi.DTO;
using AttachmentApi.Request;
using AttachmentApi.Service.Abstracts;
using Microsoft.AspNetCore.Mvc;
using SystemFile = System.IO.File;

namespace AttachmentApi.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class AttachmentController : ControllerBase
{
    private readonly IAttachmentService _service;

    public AttachmentController(IAttachmentService service)
    {
        _service = service;
    }

    [HttpGet("get/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetById(id);
        return Ok(result);
    }

    [HttpGet("download/{id}")]
    public async Task<IActionResult> DownloadFile(int id)
    {
        var attachment = await _service.GetById(id);
        var filePath = attachment.FilePath;

        var fileBytes = await SystemFile.ReadAllBytesAsync(filePath);
        var filename = Path.GetFileName(filePath);

        return File(fileBytes, "application/octet-stream", filename);
    }

    [HttpPost("pagination")]
    public async Task<IActionResult> Pagination([FromBody] PaginationRequest request)
    {
        var result = await _service.Pagination(request.Size, request.Page);

        return Ok(result);
    }

    [HttpPost("upload")]
    [RequestSizeLimit(100 * 1024 * 1024)]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        var upload = await _service.Upload(file);
        return Ok(new
        {
            TempAttachmentId = upload
        });
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateRequest requests)
    {
        var attachments = new List<AttachmentDto>();

        foreach (var request in requests.FileIdList)
            attachments.Add(await _service.Create(request));

        return Ok(attachments);
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.Delete(id);
        return NoContent();
    }
}