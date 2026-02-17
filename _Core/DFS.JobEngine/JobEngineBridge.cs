using System.Text.Json;
using DFS.JobEngine.Services;

namespace DFS.JobEngine;

public static class JobEngineBridge
{
    private static readonly JobGenerationService _service = new JobGenerationService();

    public static string GetJobJson(int playerLevel)
    {
        var job = _service.GenerateJob(playerLevel);
        return JsonSerializer.Serialize(job, new JsonSerializerOptions { WriteIndented = true });
    }
}
