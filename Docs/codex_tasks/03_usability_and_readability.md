# Milestone 3: Usability And Readability

## Priority
High

## Purpose
Make Shepherd easier to understand and fairer to play without removing its intended uncertainty.

The game is built around hidden influence ranges and indirect control, so the key question here is:
"Can the player understand what happened, even if exact numbers and ranges remain invisible?"

## Main Goals
- Improve player understanding of recruitment, threat, and combat states.
- Make cause and effect more readable.
- Reduce confusion that feels like a bug instead of a design choice.
- Improve menu and gameplay feedback where needed.

## What I Will Work On
- Review how the game communicates:
  - recruitment
  - enemy threat
  - follower aggression
  - follower return to player
  - level completion and failure states
- Improve feedback through the existing systems where appropriate:
  - animations
  - particles
  - sounds
  - UI prompts
  - cursor behavior
  - indirect visual cues
- Evaluate whether hidden aggro / influence ranges feel fair in practice.
- Identify areas where readability can improve without exposing exact values.

## Expected Deliverables
- A clearer gameplay experience with better cause-and-effect feedback.
- Reduced player confusion during core interactions.
- Improved menus and gameplay state readability where needed.

## Testing Approach
- Play each level without relying on debug information.
- Note moments where behavior is confusing rather than tactically interesting.
- Verify the player can understand:
  - why followers joined
  - why enemies attacked
  - why followers left follow mode and entered combat
  - when a dangerous situation is developing
- If possible, gather lightweight outside playtest feedback from someone unfamiliar with the code.

## Exit Criteria
- The game communicates its rules more clearly.
- Hidden ranges still feel intentional rather than arbitrary.
- The player can usually explain why important interactions happened.

## Notes
- This milestone should preserve the game's identity.
- The goal is not to turn Shepherd into an explicit numbers-heavy tactics game.
