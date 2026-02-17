using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using DFS.JobEngine.Models;

namespace DFS.JobEngine.Services;

public record CompletedJobEntry(GeneratedJob Job, DateTime Timestamp);

public class JobHistoryService
{
    private readonly string _logFilePath;

    public JobHistoryService()
    {
        // Save in the executable directory
        _logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "completed_jobs.json");
    }

    public void LogCompletedJob(GeneratedJob job)
    {
        var entry = new CompletedJobEntry(job, DateTime.UtcNow);
        List<CompletedJobEntry> history = new List<CompletedJobEntry>();

        if (File.Exists(_logFilePath))
        {
            try
            {
                string json = File.ReadAllText(_logFilePath);
                var loadedHistory = JsonSerializer.Deserialize<List<CompletedJobEntry>>(json);
                if (loadedHistory != null)
                {
                    history = loadedHistory;
                }
            }
            catch (JsonException)
            {
                // If file is corrupted, we start fresh (or could backup/log error)
            }
        }

        history.Add(entry);

        string updatedJson = JsonSerializer.Serialize(history, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_logFilePath, updatedJson);
    }
}
