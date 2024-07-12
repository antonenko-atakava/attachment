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

    public async Task<ICollection<AttachmentDto>> Pagination(uint size, uint page)
    {
        var attachments = await _repository.Pagination(size, page);

        if (attachments.Count <= 0)
            return new List<AttachmentDto>();

        var attachmentsDto = _mapper.Map<ICollection<AttachmentDto>>(attachments);

        return attachmentsDto;
    }

    public async Task<string> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new Exception("[Upload file]: отправленный файл равен null");

        var allowedExtensions = new List<string>
            { ".jpg", ".png", ".txt", ".pdf", ".docx", ".xls", ".xlsx", ".pptx", ".sig", ".csv" };

        var fileName = Path.GetFileName(file.FileName);

        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
            throw new Exception("[Upload file]: недопустимый формат файла.");

        string folderPath = @"C:\Temp";
        var filePath = Path.Combine(folderPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
            await file.CopyToAsync(stream);

        return filePath;
    }

    public async Task<AttachmentDto> Create(string filePath)
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            throw new FileNotFoundException("[Сreate file service]: временный файл не найден.");

        const string pathToFolder = @"C:\FilesManager";
        DateTime currentDate = DateTime.UtcNow;

        string currentFolder = Path.Combine(pathToFolder, currentDate.ToString("yyyy-MM-dd"));
        Directory.CreateDirectory(currentFolder);

        var fileName = Path.GetFileName(filePath);
        var newFilePath = Path.Combine(currentFolder, fileName);

        File.Move(filePath, newFilePath);

        var fileInfo = new FileInfo(newFilePath);

        var attachment = new Attachment
        {
            Extension = fileInfo.Name,
            FilePath = newFilePath,
            FileSize = fileInfo.Length,
            CreateAt = DateTime.UtcNow,
            Success = true,
        };

        await _repository.Create(attachment);
        await _repository.SaveAsync();

        var attachmentDto = _mapper.Map<AttachmentDto>(attachment);

        return attachmentDto;
    }

    public async Task<bool> Delete(int id)
    {
        var attachment = await _repository.GetById(id);

        if (attachment == null)
            throw new Exception("[Create attachment]: невозможно удалить attachment которого не существует");

        File.Delete(attachment.FilePath);

        await _repository.Delete(attachment);
        await _repository.SaveAsync();

        return true;
    }
}