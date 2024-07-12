using AttachmentApi.Service.Abstracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace AttachmentApi.Controllers;

[ApiController]
[Route("api/attachment")]
public class AttachmentController : ControllerBase
{
    private readonly IAttachmentService _service;
    private readonly IFileProvider _fileProvider;

    public AttachmentController(IAttachmentService service, IFileProvider fileProvider)
    {
        _service = service;
        _fileProvider = fileProvider;
    }

    [HttpGet("get/id/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetById(id);
        return Ok(result);
    }

    [HttpGet("get/page/{page}/size/{size}")]
    public async Task<IActionResult> Pagination(uint size, uint page)
    {
        var result = await _service.Pagination(size, page);

        return Ok(result);
    }

    [HttpGet("download/id/{id}")]
    public async Task<IActionResult> DownloadFile(int id)
    {
        var attachment = await _service.GetById(id);
        var filePath = attachment.FilePath;

        return File(System.IO.File.OpenRead(filePath), "application/octet-stream", Path.GetFileName(filePath));
    }

    [HttpPost("upload")]
    [RequestSizeLimit(100 * 1024 * 1024)]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        var upload = await _service.Upload(file);
        return Ok(upload);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(string request)
    {
        var attachment = await _service.Create(request);
        return Ok(attachment);
    }

    [HttpDelete("delete/id/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.Delete(id);
        return NoContent();
    }
}