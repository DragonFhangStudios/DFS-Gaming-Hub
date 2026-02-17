# Technical Specifications: DFS.JobEngine Math

This document outlines the formulas and logic used by the "Black Box" engine to calculate job rewards and penalties.

## Formulas

### 1. Final Payout (Credits)
The credit reward for a job is calculated based on its base reward and risk level.

**Formula:**
`Final Payout = BaseReward * (1 + (RiskLevel * 0.2))`

*   **BaseReward:** The starting credit value defined in the job template.
*   **RiskLevel:** Integer (1-5) representing the difficulty.
*   **Multiplier:** Each level of risk adds a 20% bonus to the base reward.

### 2. XP Gain
Experience points gained upon successful completion.

**Formula:**
`XP Gain = RiskLevel * 100`

*   **RiskLevel:** Integer (1-5).
*   **Base XP:** 100 XP per risk level.

### 3. Fail Penalty (Reputation Impact)
The impact on reputation if the job is aborted or failed.

**Formula:**
`Reputation Impact = RiskLevel * -5`

*   **RiskLevel:** Integer (1-5).
*   **Penalty:** -5 Reputation points per risk level.
