using AttachmentApi.Database.DTO;
using AttachmentApi.Database.Models;
using AttachmentApi.Database.Repository.Abstracts;
using AttachmentApi.Service.Abstracts;
using AutoMapper;

namespace AttachmentApi.Service;

public class AttachmentService : IAttachmentService
{
    private readonly IAttachmentRepository _repository;
    private readonly IMapper _mapper;

    public AttachmentService(IAttachmentRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<AttachmentDto> GetById(int id)
    {
        var attachment = await _repository.GetById(id);

        if (attachment == null)
            throw new Exception("[Get attachment by id]: Не удалось вернуть attachment по id. Attachment = null");

        var result = _mapper.Map<AttachmentDto>(attachment);

        return result;
    }

    public async Task<AttachmentDto> GetByName(string name)
    {
        var attachment = await _repository.GetByName(name);

        if (attachment == null)
            throw new Exception("[Get attachment by name]: Не удалось вернуть attachment по name. Attachment = null");

        var result = _mapper.Map<AttachmentDto>(attachment);

        return result;
    }

    public async Task<List<AttachmentDto>> Select()
    {
        var attachments = await _repository.Select();

        if (attachments.Count == 0)
            throw new Exception("у вас нет ни одного вложения");

        var results = new List<AttachmentDto>();

        foreach (var attachment in attachments)
            results.Add(_mapper.Map<AttachmentDto>(attachment));

        return results;
    }

    public async Task<AttachmentDto> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new Exception("[Upload file]: отправленный файл равен null");

        var allowedExtensions = new List<string>
            { ".jpg", ".png", ".txt", ".pdf", ".docx", ".xls", ".xlsx", ".pptx", ".sig", ".csv" };

        var fileName = Path.GetFileName(file.FileName);

        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
            throw new Exception("[Upload file]: недопустимый формат файла.");

        const int megabyte = 1024 * 1024;
        int maxFileSize = 100 * megabyte;

        if (file.Length > maxFileSize)
            throw new Exception("Размер файла превышает 100 Мб.");

        string folderPath = @"C:\FilesManager";
        var filePath = Path.Combine(folderPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
            await file.CopyToAsync(stream);

        var attachment = new AttachmentDto
        {
            Extension = fileName,
            FileSize = file.Length,
            FilePath = filePath,
            CreateAt = DateTime.UtcNow,
            Success = true
        };

        return attachment;
    }

    public async Task<AttachmentDto> Create(AttachmentDto entity)
    {
        var attachment = _mapper.Map<Attachment>(entity);

        if (attachment == null)
            throw new Exception("[Create attachment]: невозможно создать attachment который равен null");

        await _repository.Create(attachment);
        await _repository.SaveAsync();

        return entity;
    }

    public async Task<bool> Delete(int id)
    {
        var attachment = await _repository.GetById(id);

        if (attachment == null)
            throw new Exception("[Create attachment]: невозможно удалить attachment которого не существует");

        await _repository.Delete(attachment);
        await _repository.SaveAsync();

        return true;
    }
}