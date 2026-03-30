using System.Text;
using System.Text.RegularExpressions;

namespace PrevodCore;

public static class SubtitleProcessor
{
    private static readonly Regex TimingRegex = new(
        @"(\d{1,2}):(\d{2}):(\d{2})[,.](\d{3})\s*-->\s*(\d{1,2}):(\d{2}):(\d{2})[,.](\d{3})",
        RegexOptions.Compiled);

    public static Encoding DetectEncoding(string path)
    {
        byte[] bom = new byte[4];
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        _ = fs.Read(bom, 0, 4);
        return DecodeBom(bom);
    }

    public static Encoding DetectEncoding(Stream stream)
    {
        byte[] bom = new byte[4];
        _ = stream.Read(bom, 0, 4);
        stream.Position = 0;
        return DecodeBom(bom);
    }

    private static Encoding DecodeBom(byte[] bom)
    {
        if (bom[0] == 0xEF && bom[1] == 0xBB && bom[2] == 0xBF) return Encoding.UTF8;
        if (bom[0] == 0xFF && bom[1] == 0xFE) return Encoding.Unicode;
        if (bom[0] == 0xFE && bom[1] == 0xFF) return Encoding.BigEndianUnicode;
        return Encoding.UTF8;
    }

    public static (string result, string report) Process(string text, double offsetSeconds)
    {
        var sb = new StringBuilder();
        var log = new StringBuilder();
        int fixedTimingErrors = 0;
        int shiftedEntries = 0;
        int removedEntries = 0;

        text = text.Replace("\r\n", "\n").Replace("\r", "\n");

        string[] blocks = Regex.Split(text.Trim(), @"\n\s*\n");

        int renumbered = 1;
        int originalIndex = 0;
        foreach (string block in blocks)
        {
            string trimmed = block.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
                continue;

            string[] lines = trimmed.Split('\n');

            int timingLineIdx = -1;
            for (int i = 0; i < lines.Length; i++)
            {
                if (TimingRegex.IsMatch(lines[i]))
                {
                    timingLineIdx = i;
                    break;
                }
            }

            if (timingLineIdx < 0)
            {
                sb.AppendLine(trimmed);
                sb.AppendLine();
                continue;
            }

            originalIndex++;

            Match m = TimingRegex.Match(lines[timingLineIdx]);
            double startMs = ParseMs(m, 1);
            double endMs   = ParseMs(m, 5);

            startMs += offsetSeconds * 1000;
            endMs   += offsetSeconds * 1000;

            if (offsetSeconds != 0)
                shiftedEntries++;

            // Remove entries that fall entirely before 0
            if (startMs < 0)
            {
                log.AppendLine($"Entry (original #{originalIndex}): removed — start timestamp negative after shift.");
                removedEntries++;
                continue;
            }
            if (endMs < 0)
            {
                log.AppendLine($"Entry (original #{originalIndex}): removed — end timestamp negative after shift.");
                removedEntries++;
                continue;
            }

            // Fix: end < start — swap
            if (endMs < startMs)
            {
                log.AppendLine($"Entry {renumbered}: end ({FormatMs(endMs)}) was before start ({FormatMs(startMs)}) — swapped.");
                (startMs, endMs) = (endMs, startMs);
                fixedTimingErrors++;
            }

            // Fix: zero-duration — extend by 1 second
            if (endMs <= startMs)
            {
                log.AppendLine($"Entry {renumbered}: zero-duration entry — end extended by 1 second.");
                endMs = startMs + 1000;
                fixedTimingErrors++;
            }

            sb.AppendLine(renumbered.ToString());

            string newTiming = $"{FormatMs(startMs)} --> {FormatMs(endMs)}";
            string trailing  = TimingRegex.Replace(lines[timingLineIdx], "").Trim();
            if (!string.IsNullOrEmpty(trailing))
                newTiming += "  " + trailing;
            sb.AppendLine(newTiming);

            for (int i = timingLineIdx + 1; i < lines.Length; i++)
                sb.AppendLine(lines[i]);

            sb.AppendLine();
            renumbered++;
        }

        var reportSb = new StringBuilder();
        reportSb.AppendLine($"Processed {renumbered - 1} subtitle entries.");
        if (offsetSeconds != 0)
            reportSb.AppendLine($"Time shift applied: {offsetSeconds:+0.###;-0.###} seconds ({shiftedEntries} entries).");
        if (removedEntries > 0)
            reportSb.AppendLine($"Removed {removedEntries} entr{(removedEntries == 1 ? "y" : "ies")} with negative timestamps.");
        if (fixedTimingErrors > 0)
            reportSb.AppendLine($"Fixed {fixedTimingErrors} timing error(s) (end before/equal to start).");
        if (fixedTimingErrors == 0 && removedEntries == 0)
            reportSb.AppendLine("No timing errors found.");
        reportSb.AppendLine();
        if (log.Length > 0)
        {
            reportSb.AppendLine("Details:");
            reportSb.Append(log);
        }

        return (sb.ToString(), reportSb.ToString());
    }

    private static double ParseMs(Match m, int groupStart)
    {
        int h   = int.Parse(m.Groups[groupStart].Value);
        int min = int.Parse(m.Groups[groupStart + 1].Value);
        int s   = int.Parse(m.Groups[groupStart + 2].Value);
        int ms  = int.Parse(m.Groups[groupStart + 3].Value);
        return h * 3600000.0 + min * 60000.0 + s * 1000.0 + ms;
    }

    public static string FormatMs(double totalMs)
    {
        long ms  = (long)Math.Round(totalMs);
        if (ms < 0) ms = 0;
        int h    = (int)(ms / 3600000); ms %= 3600000;
        int min  = (int)(ms / 60000);   ms %= 60000;
        int sec  = (int)(ms / 1000);
        int milli = (int)(ms % 1000);
        return $"{h:D2}:{min:D2}:{sec:D2},{milli:D3}";
    }
}
