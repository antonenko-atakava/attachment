using AttachmentApi.DTO;
using AttachmentApi.Request;
using AttachmentApi.Service.Abstracts;
using Database.Database.Repository.Abstracts;
using Microsoft.AspNetCore.Mvc;
using SystemFile = System.IO.File;

namespace AttachmentApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttachmentController : ControllerBase
{
    private readonly IAttachmentService _service;
    private readonly IAttachmentRepository _repository;

    public AttachmentController(IAttachmentService service, IAttachmentRepository repository)
    {
        _service = service;
        _repository = repository;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _service.GetById(id);
        return Ok(result);
    }

    [HttpGet("{id}/dowload")]
    public async Task<IActionResult> DownloadFile(string id)
    {
        var attachment = await _service.GetById(id);
        var filePath = attachment.FilePath;

        var fileBytes = await SystemFile.ReadAllBytesAsync(filePath);
        var filename = attachment.Extension;

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
        using (var transaction = await _repository.BeginTransactionAsync())
        {
            try
            {
                var attachments = new List<AttachmentDto>();

                foreach (var fileId in requests.FileIdList)
                    attachments.Add(await _service.Create(fileId, transaction));

                await _repository.CommitTransactionAsync(transaction);
                return Ok(attachments);
            }
            catch (Exception e)
            {
                await _repository.RollbackTransactionAsync(transaction);
                return BadRequest("Произошла ошибка при сохранении файлов.");
            }
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _service.Delete(id);
        return NoContent();
    }
}