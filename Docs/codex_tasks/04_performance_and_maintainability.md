# Milestone 4: Performance And Maintainability

## Priority
Medium

## Purpose
Make the active game code easier to extend and less expensive to run, while keeping the old reference material intact.

This milestone prepares the project for future changes like more levels, new entity types, tutorials, and gameplay tuning.

## Main Goals
- Reduce unnecessary runtime overhead in active systems.
- Improve code organization and readability.
- Lower the risk of future changes causing unintended regressions.
- Modernize old patterns where doing so helps maintainability.

## What I Will Work On
- Inspect hot-path gameplay code for avoidable overhead, especially:
  - repeated global lookups
  - unnecessary per-frame allocations
  - repeated overlap checks
  - repeated material reassignments
  - expensive update logic in large scripts
- Improve code structure where it materially helps:
  - naming
  - inspector organization
  - field visibility and serialization
  - helper method extraction
  - reducing oversized responsibilities in single classes
- Review active asset organization if needed for clarity, while preserving legacy reference assets.
- Keep modernization grounded in the current game needs rather than rewriting everything.

## Expected Deliverables
- Cleaner active gameplay code.
- Lower-risk systems for future content work.
- Better understanding of the game's runtime cost in busy scenes.

## Testing Approach
- Use Unity Profiler on at least one busy gameplay level.
- Compare behavior before and after cleanup work.
- Re-run the core gameplay regression checklist after each cleanup batch.
- Verify there is no behavioral regression in:
  - recruitment
  - enemy AI
  - follower combat
  - scene flow

## Exit Criteria
- The active codebase is easier to work with.
- Obvious hot-path inefficiencies have been reduced.
- Cleanup work does not introduce gameplay regressions.

## Notes
- This milestone should support future-proofing, not rewrite the entire project architecture just because the code is old.
