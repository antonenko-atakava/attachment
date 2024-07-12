using System.Text;
using AttachmentApi.Database.DTO;
using AttachmentApi.Service.Abstracts;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AttachmentApi.Controllers;

[ApiController]
[Route("api/attachment")]
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

    [HttpGet("get/name/{name}")]
    public async Task<IActionResult> GetByName(string name)
    {
        var result = await _service.GetByName(name);
        return Ok(result);
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        var upload = await _service.Upload(file);

        var client = new HttpClient();

        var json = JsonConvert.SerializeObject(upload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response =
            await client.PostAsync($"{Request.Scheme}://{Request.Host}{Request.PathBase}/api/Attachment/create",
                content);

        var responseData = await response.Content.ReadAsStringAsync();
        var createdAttachment = JsonConvert.DeserializeObject<AttachmentDto>(responseData);
        
        return Ok(createdAttachment);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] AttachmentDto requset)
    {
        var attachment = await _service.Create(requset);
        return Ok(attachment);
    }

    [HttpDelete("delete/id/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.Delete(id);
        return NoContent();
    }
}