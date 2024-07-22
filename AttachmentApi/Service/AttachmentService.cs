using AttachmentApi.DTO;
using AttachmentApi.Service.Abstracts;
using AutoMapper;
using Database.Database.Entity;
using Database.Database.Repository.Abstracts;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;

namespace AttachmentApi.Service;

public class AttachmentService : IAttachmentService
{
    private readonly IAttachmentRepository _repository;
    private readonly IMapper _mapper;
    private readonly IOptions<AttachmentServiceOptions> _options;
    private readonly ILogger<AttachmentService> _logger;

    public AttachmentService(IAttachmentRepository repository, IMapper mapper,
        IOptions<AttachmentServiceOptions> options, ILogger<AttachmentService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _options = options;
        _logger = logger;
    }

    public async Task<AttachmentDto> GetById(string id)
    {
        var attachment = await _repository.GetById(id);

        if (attachment == null)
            throw new Exception("[Get attachment by id]: Не удалось вернуть attachment по id. Attachment = null");

        var result = _mapper.Map<AttachmentDto>(attachment);

        return result;
    }

    public async Task<ICollection<AttachmentDto>> Pagination(uint size, uint page)
    {
        if (page == 0 || size == 0)
            throw new Exception("[Pagination error]:Размер елементов или страница не могут быть значением равным 0");

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

        var guid = Guid.NewGuid().ToString();

        var folderPath = Path.Combine(_options.Value.TempFolder, guid);
        Directory.CreateDirectory(folderPath);

        var filePath = Path.Combine(folderPath, fileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return guid;
    }

    
    public async Task<AttachmentDto> Create(string fileId, IDbContextTransaction transaction)
    {
        var tempFolderPath = Path.Combine(_options.Value.TempFolder, fileId);

        if (!Directory.Exists(tempFolderPath))
            throw new DirectoryNotFoundException("[Create file service]: временная директория не найдена.");

        var filePath = Directory.GetFiles(tempFolderPath).FirstOrDefault();

        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            throw new FileNotFoundException("[Create file service]: временный файл не найден.");

        var currentDate = DateTime.UtcNow;
        var currentFolder = Path.Combine(_options.Value.FileManagerFolder, currentDate.ToString("yyyy-MM-dd"));

        Directory.CreateDirectory(currentFolder);

        var fileName = Path.GetFileName(filePath);
        var fileExtension = Path.GetExtension(filePath);
        var newFileName = $"{fileId}{fileExtension}";
        var newFilePath = Path.Combine(currentFolder, newFileName);
        
        File.Move(filePath, newFilePath);

        Directory.Delete(tempFolderPath, true);

        var fileInfo = new FileInfo(newFilePath);

        var attachment = new Attachment
        {
            Id = fileId,
            Extension = fileName,
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

    public async Task<bool> Delete(string id)
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