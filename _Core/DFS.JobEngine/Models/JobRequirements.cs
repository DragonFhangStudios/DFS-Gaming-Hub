using System.Collections.Generic;

namespace DFS.JobEngine.Models;

public record JobRequirements(
    int MinLevel,
    List<string> RequiredItems,
    List<string> RequiredSkills
);
