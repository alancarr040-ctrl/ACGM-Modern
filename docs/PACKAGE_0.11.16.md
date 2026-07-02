# Package

0.11.16

# Title

Legacy Search System Restoration

# Goal

Restore the Legacy Search System from the original VB6 ACGM client.

The Search tab currently exists as a placeholder.

This package recreates the complete VB6 Search workspace while preserving the existing project philosophy:

**Recreation first. Modernization underneath.**

No modern search enhancements are included in this package.

# Background

The original VB6 client contained a dedicated Search workspace allowing advanced filtering of the allegiance tree.

The current Modern Client contains only a placeholder page.

This package restores the complete legacy functionality.

# Scope

Implement the Search tab to match the legacy VB6 client.

Restore:

* Search Filter group
* Rank filter
* Race filter
* Skill filter grid
* Search button
* Reset button
* Search Results grid

# Do Not Modify

See:

* AI_PROJECT_SPEC.md
* AI_CERTIFIED_SUBSYSTEMS.md

Certified subsystems remain frozen.

Do not modify:

* HTTPS
* Login
* Tree Parser
* Character Parser
* Character Editing
* Character Save Serialization
* Character Save Certification Suite
* Tree Icons
* Current Player Functions
* Character Dialog
* Legacy Protocols

Unless a verified Search System defect requires a targeted correction.

# Search Filter Certification

Restore the following VB6 controls.

General

* Rank dropdown
* Race dropdown

Skill Filters

Restore all legacy skill filters.

Each skill supports a minimum value.

Search Controls

* Search
* Reset

Results

Display:

* Name
* Level
* Rank
* Race

# Certification Requirements

Verify:

No filters:

* Returns all characters.

Rank filter:

* Correct results returned.

Race filter:

* Correct results returned.

Single Skill filter:

* Correct results returned.

Multiple Skill filters:

* Correct intersection returned.

Mixed filters:

* Rank
* Race
* Skills

Reset:

* Clears all filters.

Empty Result:

* Displays correctly.

Large Result Set:

* Displays correctly.

# Diagnostics

Reuse existing certification diagnostics where appropriate.

Do not introduce additional diagnostics unless required.

# Documentation

Update:

* README
* CHANGELOG
* RELEASE_NOTES
* LEGACY_DISCOVERIES.md
* Engineering Notes

Document:

The Legacy Search System has been restored and certified against the original VB6 implementation.

Record any remaining behavioral differences for future modernization packages.

# Future Modernization (Not Part of This Package)

Deferred to Phase 0.12:

* Double-click result opens Character
* Column sorting
* Live filtering
* Export results
* Saved searches
* Additional search criteria
* Search history
* Cached searches

# Deliverables

* Complete Visual Studio Solution
* Updated Documentation
* Release Notes
* ZIP Package

# Version

ACGM Modern Client 0.11.16 – Legacy Search System Restoration
