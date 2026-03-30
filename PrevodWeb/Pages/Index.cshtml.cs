using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using System.Text;
using PrevodCore;

namespace PrevodWeb.Pages;

public class IndexModel : PageModel
{
    private readonly IMemoryCache _cache;

    public IndexModel(IMemoryCache cache) => _cache = cache;

    [TempData] public string? Log          { get; set; }
    [TempData] public string? DownloadId   { get; set; }
    [TempData] public string? DownloadName { get; set; }
    [TempData] public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnPostAsync(
        IFormFile subtitleFile,
        double offsetSeconds)
    {
        if (subtitleFile is null || subtitleFile.Length == 0)
        {
            ErrorMessage = "Please select a subtitle file.";
            return RedirectToPage();
        }

        string ext = Path.GetExtension(subtitleFile.FileName).ToLowerInvariant();
        if (ext is not (".srt" or ".sub"))
        {
            ErrorMessage = "Only .srt and .sub files are supported.";
            return RedirectToPage();
        }

        try
        {
            await using var stream = subtitleFile.OpenReadStream();
            var encoding = SubtitleProcessor.DetectEncoding(stream);
            using var reader = new StreamReader(stream, encoding);
            string rawText = await reader.ReadToEndAsync();

            var (fixedText, report) = SubtitleProcessor.Process(rawText, offsetSeconds);

            var bytes = new UTF8Encoding(true).GetBytes(fixedText);
            string id = Guid.NewGuid().ToString("N");
            _cache.Set(id, bytes, TimeSpan.FromMinutes(30));

            DownloadId   = id;
            DownloadName = Path.GetFileNameWithoutExtension(subtitleFile.FileName) + "_fixed" + ext;
            Log          = report;
        }
        catch (Exception ex)
        {
            ErrorMessage = "Processing failed: " + ex.Message;
        }

        return RedirectToPage();
    }
}
