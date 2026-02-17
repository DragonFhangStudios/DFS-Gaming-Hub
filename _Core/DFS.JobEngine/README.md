# DFS.JobEngine

The `DFS.JobEngine` is a "Black Box" library responsible for generating procedural jobs, calculating rewards, and managing job history.

## Initialization & Usage

### 1. Requesting a Job (The Bridge)
The easiest way to get a job is via the `JobEngineBridge`. This static helper handles the service instantiation and returns a JSON string ready for consumption by Unity or other frontends.

```csharp
using DFS.JobEngine;

// Request a job for a Player Level 1
string jobJson = JobEngineBridge.GetJobJson(1);
```

### 2. Logging Completed Jobs
To log a completed job to the local history file (`completed_jobs.json`), use the `JobHistoryService`.

```csharp
using DFS.JobEngine.Services;

var historyService = new JobHistoryService();
historyService.LogCompletedJob(generatedJobObject);
```

## Technical Specifications (Math Logic)

The engine uses the following formulas to calculate rewards based on the job's Risk Level (Difficulty).

### Final Payout (Credits)
`Final Payout = BaseReward * (1 + (RiskLevel * 0.2))`
*   Each level of risk adds a **20% bonus** to the base reward.

### XP Gain
`XP Gain = RiskLevel * 100`
*   **100 XP** per risk level.

### Fail Penalty (Reputation)
`Reputation Impact = RiskLevel * -5`
*   **-5 Reputation** per risk level if the job is aborted or failed.
