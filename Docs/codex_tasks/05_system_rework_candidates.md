# Milestone 5: System Rework Candidates

## Priority
Medium

## Purpose
Rework systems that are known to be imperfect once the base game is stable enough to safely change them.

This milestone is intentionally later in the plan because reworking unstable systems too early would make debugging much harder.

## Main Goals
- Identify the systems that still feel wrong after stabilization.
- Define the intended behavior for each one before changing implementation.
- Rework one system at a time in a controlled way.

## What I Will Work On
- Build a shortlist of systems that still need redesign or heavier fixes after milestones 1 to 4.
- For each candidate system:
  - define current behavior
  - define intended behavior
  - list failure cases
  - choose the smallest safe redesign scope
- Likely candidates may include:
  - target acquisition and retargeting
  - follower combat resolution
  - aggression recovery
  - persistence / progression behavior
  - recruitment edge cases
  - any advanced follower behaviors that are still part of the active game

## Expected Deliverables
- A prioritized rework list.
- Clear design intent for each chosen system.
- Safer implementations of the most important weak points in the base game.

## Testing Approach
- Create focused regression tests for each reworked system.
- Test each reworked system in at least 2 active levels.
- Re-run the full base-game smoke test after each completed rework.
- Verify both correctness and feel, since these changes affect gameplay directly.

## Exit Criteria
- The known weak systems in the base game are either improved or clearly deferred.
- Reworked systems behave according to an explicit intended design.
- The project is in a good state for future content work.

## Notes
- New content should wait until this milestone is complete or intentionally paused.
- The goal is to improve the base game first, then build on a stronger foundation.
