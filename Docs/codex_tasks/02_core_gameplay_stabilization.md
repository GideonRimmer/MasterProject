# Milestone 2: Core Gameplay Stabilization

## Priority
Highest

## Purpose
Make the current base game behave consistently and predictably without changing its core design.

This milestone focuses on reliability:
- recruitment should always work
- pursuit and combat should resolve cleanly
- state changes should not leave actors stuck in bad states

## Main Goals
- Stabilize the main gameplay loop.
- Reduce hidden coupling between core gameplay systems.
- Make follower and enemy behavior easier to reason about.
- Prevent state bugs that cause broken combat, recruitment, or recovery behavior.

## What I Will Work On
- Review and improve the active core scripts:
  - `PlayerController`
  - `SphereOfInfluence`
  - `FollowerManager`
  - `EnemyManager`
  - `HitPointsManager`
  - `GameManager`
  - `SavePlayerData`
- Add defensive checks where references or states may be invalid.
- Normalize behavior when:
  - targets die
  - leaders change
  - AI loses aggro
  - combat ends
  - player dies
- Review persistence and scene progression logic using `PlayerPrefs`.
- Clarify which mechanics are part of the current base game and which are legacy or inactive.

## Expected Deliverables
- Safer and more understandable core gameplay code.
- Reduced chances of null references and broken AI transitions.
- A documented list of behavior assumptions for the base game.

## Testing Approach
- Re-run the milestone 1 regression checklist after each meaningful change.
- Specifically test:
  - auto-recruitment in player range
  - follower follow behavior after recruitment
  - follower aggro and return-to-player behavior
  - enemy chase and return-to-origin behavior
  - player death and game-over flow
  - level completion flow
- Test edge cases:
  - target dies during pursuit
  - multiple actors target the same entity
  - player dies while followers are mid-combat
  - follower changes state near enemies

## Exit Criteria
- Core gameplay works consistently across all active levels.
- AI actors return to valid states after combat or target loss.
- The current game loop is stable enough for usability and cleanup work.

## Notes
- The goal is stabilization first, not feature expansion.
- If a mechanic is clearly broken and not essential to the current base game, it may be deferred or scoped separately.
