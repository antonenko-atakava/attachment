using AttachmentApi.Database;
using AttachmentApi.Database.Repository;
using AttachmentApi.Database.Repository.Abstracts;
using AttachmentApi.Infrastructure;
using AttachmentApi.Mapper;
using AttachmentApi.Middleware;
using AttachmentApi.Service;
using AttachmentApi.Service.Abstracts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

builder.Services.AddSingleton(configuration);

string? connectionString = configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<DatabaseContext>(options => { options.UseNpgsql(connectionString); });

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(MapperProfile));

builder.Services.AddScoped<IAttachmentRepository, AttachmentRepository>();
builder.Services.AddScoped<IAttachmentService, AttachmentService>();

builder.Services.AddHostedService<FileCleanupService>();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(corsPolicyBuilder =>
{
    corsPolicyBuilder.WithOrigins();
    corsPolicyBuilder.AllowAnyHeader();
    corsPolicyBuilder.AllowAnyMethod();
    corsPolicyBuilder.AllowCredentials();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    await dbContext.Database.MigrateAsync();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();