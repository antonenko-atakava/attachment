using System.IO;
using System.Threading.Tasks;
using AttachmentApi.Service.Abstracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SystemFile = System.IO.File;

namespace AttachmentApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttachmentController : ControllerBase
{
    private readonly IAttachmentService _service;

    public AttachmentController(IAttachmentService service)
    {
        _service = service;
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

        var fileBytes = await SystemFile.ReadAllBytesAsync(filePath);
        var filename = Path.GetFileName(filePath);

        return File(fileBytes, "application/octet-stream", filename);
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