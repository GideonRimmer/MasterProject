# Milestone 1: Establish A Stable Baseline

## Priority
Highest

## Purpose
Create a trustworthy baseline for the current game before changing behavior or improving systems.

This milestone is about answering a simple question:
"What is the active version of Shepherd today, and does it work reliably from start to finish?"

## Main Goals
- Confirm the active project structure for the base game.
- Verify the 6 active gameplay levels and scene flow.
- Identify runtime errors, warnings, missing references, and broken hookups.
- Separate active gameplay assets from archived reference assets without deleting the archived ones.
- Produce a clear baseline of how the current game behaves.

## What I Will Work On
- Review the active scenes in build settings.
- Inspect the active gameplay prefabs:
  - Player
  - Follower
  - Enemy
  - Innocent
  - Game settings / UI / camera / level-end trigger
- Inspect the active gameplay scripts tied to the core loop.
- Verify tags, layers, serialized references, colliders, NavMesh usage, and scene wiring.
- Check scene transitions:
  - Main menu
  - Gameplay levels
  - Level end menu
  - Restart / pause / game over flow
- Note outdated patterns or risky assumptions that may still function but are fragile.

## Expected Deliverables
- A short baseline audit of the active game path.
- A list of active systems and assets currently used by the game.
- A ranked list of obvious issues found during inspection and testing.
- A regression checklist for future milestones.

## Testing Approach
- Play all 6 levels in sequence.
- For each level, verify:
  - scene loads correctly
  - player can move
  - followers are recruitable
  - enemies aggro and disengage correctly
  - level exit works
  - next-level flow works
- Test pause, restart, game over, and return-to-menu behavior.
- Watch the Unity console for warnings and errors.

## Exit Criteria
- We understand the active runtime path of the game.
- No critical scene-flow blockers remain unidentified.
- We have a reliable baseline to compare later changes against.

## Notes
- Archived scripts, prefabs, and old reference material stay in the project.
- This milestone does not redesign systems yet.
