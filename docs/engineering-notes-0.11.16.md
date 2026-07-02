# Engineering Notes - 0.11.16

## Legacy Search System Restoration - Milestone 2

Milestone 2 wires the restored Search workspace to executable search logic.

The legacy tree payload does not include race or skill values, so the restored search uses the existing certified character-detail retrieval path to load each character's detailed record. Retrieved details are cached in memory for the current session and cleared whenever the tree is refreshed.

Implemented behavior:

- Rank dropdown filter.
- Race dropdown filter.
- Skill minimum grid filters.
- Combined filter intersection.
- Results grid population with Name, Level, Rank, and Race.
- Reset clears Rank, Race, all skill minimums, and the results list.

No parser, serializer, tree icon, current-player-function, character-dialog, or legacy protocol definitions were changed.

## Certification Notes

This milestone restores behavior only. Modern search enhancements such as column sorting, double-click open, export, saved searches, and live filtering remain deferred to Phase 0.12.
