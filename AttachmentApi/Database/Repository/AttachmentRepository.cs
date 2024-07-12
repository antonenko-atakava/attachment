using AttachmentApi.Database.Models;
using AttachmentApi.Database.Repository.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace AttachmentApi.Database.Repository;

public class AttachmentRepository : IAttachmentRepository
{
    private readonly DatabaseContext _db;

    public AttachmentRepository(DatabaseContext db)
    {
        _db = db;
    }

    public async Task<Attachment?> GetById(int id)
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
}