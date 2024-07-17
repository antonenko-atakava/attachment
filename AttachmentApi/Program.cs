using AttachmentApi;
using AttachmentApi.Mapper;
using AttachmentApi.Middleware;
using AttachmentApi.Service;
using AttachmentApi.Service.Abstracts;
using BackgroundService;
using Database.Database;
using Database.Database.Repository;
using Database.Database.Repository.Abstracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

builder.Services.AddSingleton(configuration);

var connectionString = configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<DatabaseContext>(options => { options.UseNpgsql(connectionString); });

builder.Services.AddControllers();

builder.Services.AddScoped<IAttachmentRepository, AttachmentRepository>();
builder.Services.AddScoped<IAttachmentService, AttachmentService>();

builder.Services.Configure<FileCleanupServiceOptions>(builder.Configuration.GetSection("FileCleanupService"));
builder.Services.Configure<AttachmentServiceOptions>(builder.Configuration.GetSection("AttachmentService"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(MapperProfile));

builder.Services.AddAttachmentService(options =>
{
    options.TempFolder = builder.Configuration["AttachmentService:TempFolder"] ?? throw new NullReferenceException();
    options.FileManagerFolder =
        builder.Configuration["AttachmentService:FileManagerFolder"] ?? throw new NullReferenceException();
});
builder.Services.AddFileCleanupService(options =>
{
    options.TempFolder = builder.Configuration["FileCleanupService:TempFolder"] ?? throw new NullReferenceException();

    var interval = int.Parse(builder.Configuration["FileCleanupService:CleanInterval"]!);
    var intervalWorker = int.Parse(builder.Configuration["FileCleanupService:CleanIntervalWorker"]!);

    options.CleanInterval = TimeSpan.FromMilliseconds(interval);
    options.CleanIntervalWorker = TimeSpan.FromMilliseconds(intervalWorker);
});

var app = builder.Build();

var fileCleanupServiceOptions = app.Services.GetRequiredService<IOptions<FileCleanupServiceOptions>>().Value;
var attachmentServiceOptions = app.Services.GetRequiredService<IOptions<AttachmentServiceOptions>>().Value;
ConfigurationValidator.ValidateOptions(fileCleanupServiceOptions, attachmentServiceOptions);

app.UseMiddleware<ExceptionMiddleware>();
app.UseStaticFiles();

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

await app.RunAsync();