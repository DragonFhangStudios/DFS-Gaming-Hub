using System;
using DFS.JobEngine.Models;

namespace DFS.JobEngine.Services;

public class JobGenerationService
{
    public JobData GenerateJob(int playerLevel)
    {
        return new JobData(
            ID: Guid.NewGuid().ToString(),
            Title: "Placeholder Job",
            Description: "This is a placeholder job generated for testing.",
            ClientName: "Unknown Client",
            DifficultyLevel: playerLevel
        );
    }
}
