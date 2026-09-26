using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Playwright;

public static class GrabApi
{
    public static List<string> logs = new();
    public static async Task<string> DownloadLevelAsync(
        string userId,
        string timestamp,
        string outputDirectory)
    {
        const string iteration = "1";

        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException(
                "User ID is required.",
                nameof(userId));

        if (!ulong.TryParse(timestamp, out _))
            throw new ArgumentException(
                "Timestamp must contain only numbers.",
                nameof(timestamp));

        Directory.CreateDirectory(outputDirectory);

        string outputFile = Path.Combine(
            outputDirectory,
            $"{timestamp}.level");

        logs.Add("Downloading level...");
        logs.Add($"User ID: {userId}");
        logs.Add($"Timestamp: {timestamp}");

        using IPlaywright playwright =
            await Playwright.CreateAsync();

        await using IBrowser browser =
            await playwright.Chromium.LaunchAsync(
                new BrowserTypeLaunchOptions
                {
                    Headless = true
                });

        await using IBrowserContext context =
            await browser.NewContextAsync(
                new BrowserNewContextOptions
                {
                    UserAgent =
                        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
                        "AppleWebKit/537.36 (KHTML, like Gecko) " +
                        "Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0"
                });

        IPage page = await context.NewPageAsync();

        page.Request += (_, request) =>
        {
            if (request.Url.Contains("api.slin.dev"))
            {
                foreach (var header in request.Headers)
                    Console.WriteLine($"{header.Key}: {header.Value}");

            }
        };

        await page.GotoAsync(
            "https://grabvr.quest/",
            new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded
            });

        string url =
            $"https://api.slin.dev/grab/v1/download/" +
            $"{userId}/{timestamp}/{iteration}";

        string[] data = await page.EvaluateAsync<string[]>(
            """
            async ({ url }) =>
            {
                const response = await fetch(url, {
                    method: "GET",

                    headers: {
                        "Accept": "*/*",
                        "Prefer": "safe"
                    },

                    referrer: "https://grabvr.quest/",
                    credentials: "omit"
                });

                if (!response.ok)
                {
                    throw new Error(
                        `HTTP ${response.status}: ` +
                        await response.text()
                    );
                }

                const buffer =
                    await response.arrayBuffer();

                return Array.from(
                    new Uint8Array(buffer),
                    byte => byte.toString()
                );
            }
            """,
            new { url });

        byte[] bytes = new byte[data.Length];

        for (int i = 0; i < data.Length; i++)
            bytes[i] = byte.Parse(data[i]);

        await File.WriteAllBytesAsync(
            outputFile,
            bytes);

        logs.Add("Success!");
        logs.Add($"Downloaded: {bytes.Length} bytes");
        logs.Add($"Saved: {outputFile}");

        return outputFile;
    }

    public static string[] getLevelDataWithLink(string link)
    {
        string data = link.Split("?level=")[1];
        return data.Split(":");
    }
}