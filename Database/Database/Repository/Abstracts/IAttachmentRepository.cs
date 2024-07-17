using Database.Database.Entity;

namespace Database.Database.Repository.Abstracts;

public interface IAttachmentRepository
{
    Task<Attachment?> GetById(int id);
    Task<ICollection<Attachment>> Pagination(uint size, uint page);
    Task<Attachment> Create(Attachment entity);
    Task<bool> Delete(Attachment entity);
    Task SaveAsync();
}