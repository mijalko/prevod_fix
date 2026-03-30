using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;

namespace PrevodWeb.Pages;

public class DownloadModel : PageModel
{
    private readonly IMemoryCache _cache;

    public DownloadModel(IMemoryCache cache) => _cache = cache;

    public IActionResult OnGet(string id, string filename)
    {
        if (string.IsNullOrWhiteSpace(id) || !_cache.TryGetValue<byte[]>(id, out var data) || data is null)
            return NotFound("File not found or expired.");

        _cache.Remove(id);
        return File(data, "text/plain; charset=utf-8", filename);
    }
}
