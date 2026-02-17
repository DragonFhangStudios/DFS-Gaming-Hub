namespace DFS.JobEngine.Models;

public record JobData(
    string ID,
    string Title,
    string Description,
    string ClientName,
    int DifficultyLevel
);
