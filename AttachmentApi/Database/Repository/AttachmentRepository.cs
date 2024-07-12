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
        return await _db.Attachments.FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<Attachment?> GetByName(string name)
    {
        return await _db.Attachments.FirstOrDefaultAsync(n => n.Extension == name);
    }

    public async Task<List<Attachment>> Select()
    {
        return await _db.Attachments.ToListAsync();
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