using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using DFS.JobEngine.Models;

namespace DFS.JobEngine.Services;

public class JobGenerationService
{
    private List<JobData> _jobTemplates = new();

    public JobGenerationService()
    {
        LoadJobTemplates();
    }

    private void LoadJobTemplates()
    {
        try
        {
            var assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var directoryPath = Path.GetDirectoryName(assemblyLocation);

            if (string.IsNullOrEmpty(directoryPath))
            {
                // Fallback or log
                return;
            }

            var manifestPath = Path.Combine(directoryPath, "JobManifest.json");

            if (File.Exists(manifestPath))
            {
                var jsonContent = File.ReadAllText(manifestPath);
                var loadedTemplates = JsonSerializer.Deserialize<List<JobData>>(jsonContent);
                if (loadedTemplates != null)
                {
                    _jobTemplates = loadedTemplates;
                }
            }
        }
        catch (Exception)
        {
            // Log error
            // _jobTemplates remains empty
        }
    }

    public GeneratedJob GenerateJob(int playerLevel)
    {
        if (_jobTemplates.Count == 0)
        {
            return CreateFallbackJob();
        }

        var random = new Random();
        var template = _jobTemplates[random.Next(_jobTemplates.Count)];

        // Create a new instance with a unique ID
        var jobData = template with { ID = Guid.NewGuid().ToString() };

        return CalculateJob(jobData);
    }

    private GeneratedJob CalculateJob(JobData jobData)
    {
        // 1. Final Payout Calculation
        // Formula: BaseReward * (1 + (RiskLevel * 0.2))
        double multiplier = 1 + (jobData.DifficultyLevel * 0.2);
        int finalPayout = (int)(jobData.BaseReward * multiplier);

        // 2. XP Gain Calculation
        // Formula: RiskLevel * 100
        int xpGain = jobData.DifficultyLevel * 100;

        // 3. Fail Penalty (Reputation Impact)
        // Formula: RiskLevel * -5
        int reputationImpact = jobData.DifficultyLevel * -5;

        var rewards = new JobRewards(
            CreditPayout: finalPayout,
            XPGain: xpGain,
            ReputationImpact: reputationImpact
        );

        // Placeholder Requirements logic
        var requirements = new JobRequirements(
            MinLevel: jobData.DifficultyLevel,
            RequiredItems: new List<string>(),
            RequiredSkills: new List<string>()
        );

        return new GeneratedJob(
            Data: jobData,
            Rewards: rewards,
            Requirements: requirements
        );
    }

    private GeneratedJob CreateFallbackJob()
    {
        var fallbackData = new JobData(
            ID: Guid.NewGuid().ToString(),
            Title: "Fallback Job",
            Description: "No job templates available.",
            ClientName: "System",
            DifficultyLevel: 1,
            BaseReward: 100,
            FlavorText: "System error.",
            Niche: "General"
        );

        var rewards = new JobRewards(
            CreditPayout: 100,
            XPGain: 10,
            ReputationImpact: -1
        );

        var requirements = new JobRequirements(
            MinLevel: 1,
            RequiredItems: new List<string>(),
            RequiredSkills: new List<string>()
        );

        return new GeneratedJob(fallbackData, rewards, requirements);
    }
}
