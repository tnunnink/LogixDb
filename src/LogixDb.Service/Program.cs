using LogixDb.Service.Common;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Channels;
using LogixConverter.Abstractions;
using LogixDb.Data;
using LogixDb.Service.Workers;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseWindowsService(o => o.ServiceName = "LogixDb");

// Register the app settings configuration to drive further registration.
builder.Services.Configure<LogixConfig>(builder.Configuration.GetSection(nameof(LogixConfig)));

// Add message channels for queuing work to be processed by background services.
builder.Services.AddSingleton(Channel.CreateUnbounded<Import>());
builder.Services.AddSingleton(Channel.CreateUnbounded<AssetInfo>());

// Register the local import converter wrapper with the options ACD executable from the configuration settings.
builder.Services.AddSingleton<ILogixFileConverter>(p =>
    new ImportConverter(p.GetRequiredService<IOptions<LogixConfig>>().Value.AcdConverter)
);

// Register API background services
builder.Services.AddSingleton<SourceUploadService>();
builder.Services.AddHostedService<SourceIngestionService>();

// Add the appropriate ILogixProvider based on the connection string
builder.Services.AddLogixDb(builder.Configuration.GetSection(nameof(LogixConfig)).Get<LogixConfig>());

// Add the FTAC services if enabled in configuration. By default, it is disabled. Users need to opt in.
if (builder.Configuration.GetSection(nameof(LogixConfig)).Get<LogixConfig>()?.FtacMonitor is true)
{
    builder.Services.AddHostedService<FtacMonitorService>();
    builder.Services.AddHostedService<FtacDownloadService>();
}

var app = builder.Build();


// Defines a simple health check endpoint to verify communication with service.
app.MapGet("/health",
    () => Results.Ok(new { status = "ok", time = DateTimeOffset.UtcNow })
);

// Defines the primary endpoint for ingesting L5X and ACD files.
// This lets external tools or scripts upload logix files to the local server.
// The service hosts a background service that will ingest these uploaded files to the configured LogixDb database.
app.MapPost("/ingest", async (
    [FromForm] IFormFile file,
    HttpRequest request,
    SourceUploadService uploadService,
    ILogger<Program> logger
) =>
{
    if (file.Length == 0)
        return Results.BadRequest("No file content provided.");

    if (!file.FileName.IsLogixFile())
        return Results.BadRequest("Invalid file type. Only .L5X and .ACD files are supported.");

    // Extract custom metadata from Request Headers.
    // This will let users associate custom info with a given source upload.
    const string metadataHeaderPrefix = "Logix-";
    var metadata = new Dictionary<string, string>();
    foreach (var header in request.Headers)
    {
        if (header.Key.StartsWith(metadataHeaderPrefix, StringComparison.OrdinalIgnoreCase))
        {
            var key = header.Key[metadataHeaderPrefix.Length..];
            metadata[key] = header.Value!;
        }
    }

    try
    {
        var source = await uploadService.UploadAsync(file, metadata);

        return Results.Accepted(uri: string.Empty, value: new
        {
            importId = source.ImportId,
            received = source.FileName,
            status = "Queued"
        });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while processing the upload for {FileName}", file.FileName);
        return Results.Problem("An internal error occurred while processing the upload. Please check server logs.");
    }
}).DisableAntiforgery();

await app.RunAsync();