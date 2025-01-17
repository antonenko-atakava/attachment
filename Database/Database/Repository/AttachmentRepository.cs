using Database.Database.Entity;
using Database.Database.Repository.Abstracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Database.Database.Repository;

public class AttachmentRepository : IAttachmentRepository
{
    private readonly DatabaseContext _db;

    public AttachmentRepository(DatabaseContext db)
    {
        _db = db;
    }

    public async Task<Attachment?> GetById(string id)
    {
        return await _db.Attachments
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<ICollection<Attachment>> Pagination(uint size, uint page)
    {
        var attacments = await _db.Attachments.AsNoTracking()
            .Skip((int)size * ((int)page - 1))
            .Take((int)size)
            .ToListAsync();

        if (attacments.Count <= 0)
            return new List<Attachment>();

        return attacments;
    }

    public async Task<Attachment> Create(Attachment entity)
    {
        await _db.Attachments.AddAsync(entity);

        return entity;
    }

    public Task<bool> Delete(Attachment entity)
    {
        _db.Attachments.Remove(entity);

        return Task.FromResult(true);
    }

    public async Task SaveAsync()
    {
        await _db.SaveChangesAsync();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _db.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync(IDbContextTransaction transaction)
    {
        await transaction.CommitAsync();
    }

    public async Task RollbackTransactionAsync(IDbContextTransaction transaction)
    {
        await transaction.RollbackAsync();
    }
}