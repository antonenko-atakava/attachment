using Database.Database.Entity;
using Microsoft.EntityFrameworkCore.Storage;

namespace Database.Database.Repository.Abstracts;

public interface IAttachmentRepository
{
    Task<Attachment?> GetById(string id);
    Task<ICollection<Attachment>> Pagination(uint size, uint page);
    Task<Attachment> Create(Attachment entity);
    Task<bool> Delete(Attachment entity);
    Task SaveAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task CommitTransactionAsync(IDbContextTransaction transaction);
    Task RollbackTransactionAsync(IDbContextTransaction transaction);
}