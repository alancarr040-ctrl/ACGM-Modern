# ACGM Modern Client Certification Report

Version: 0.11.19 - Legacy Protocol Inventory & Documentation

## Executive Summary

ACGM Modern Client 0.11 is the Legacy Certification phase. Its purpose is to recreate the behavior of the original VB6 ACGM client before introducing modern enhancements.

The project rule remains:

> Recreation first. Modernization underneath.

As of 0.11.19, the major user-facing legacy subsystems have been restored, tested, and certified. The remaining 0.11 milestone is 0.11.20 Final Legacy Certification.

## Package Certification Matrix

| Package | Area | Status |
|---|---|---|
| 0.11.11 | Legacy Tree Icon Certification | Gold |
| 0.11.12 | Legacy Character Save Serialization Certification | Gold |
| 0.11.13 | Legacy Character Save Certification Suite | Gold |
| 0.11.13a | Diagnostics Configuration Correction | Gold |
| 0.11.14 | Legacy Current Player Functions Menu Restoration | Gold |
| 0.11.15 | Legacy Character Dialog Certification | Gold |
| 0.11.16 | Legacy Search System Restoration | Gold |
| 0.11.17 | Legacy Tree Behavior Certification | Gold |
| 0.11.18 | Administration Certification | Gold by accumulated certification |
| 0.11.19 | Legacy Protocol Inventory & Documentation | Complete |

## Certified Subsystems

| Subsystem | Status |
|---|---|
| HTTPS | Gold |
| Login | Gold |
| Legacy CGI Compatibility | Gold |
| Tree Parser | Gold |
| Character Parser | Gold |
| Tree Icons | Gold |
| Character Editing | Gold |
| Character Save Serialization | Gold |
| Character Save Certification Suite | Gold |
| Awards | Gold |
| Menu System | Gold |
| Current Player Functions | Gold |
| Character Dialog | Gold |
| Find Character | Gold |
| Search Functionality | Gold |
| Tree Functionality | Gold |
| Administrator Functions | Gold |
| Server Setup | Gold |
| Security Level | Gold |
| Administrative Functionality | Gold |

## Certified Data Paths

### Tree Path

```text
MSG_GET_TREE -> LegacyTreeParser -> CharacterRecord -> TreeView -> Certified Icons
```

Certified behaviors:

- Compact tree payload parsing.
- Mule, PK, and Rescue Squad flag mapping.
- Monarch, Normal, Mule, PK, and Rescue icon selection.
- Refresh behavior.
- Tree location/selection persistence.

### Character Detail Path

```text
MSG_GET_CHAR_INFO -> LegacyCharacterParser -> CharacterDetails -> Character Dialog
```

Certified behaviors:

- General information.
- Allegiance information.
- Skills.
- Real Life and Contact information.
- Awards display.
- Administrative fields.

### Character Save Path

```text
Character Dialog -> MSG_UPDATE_CHAR_INFO -> CGI -> tree.dat -> Reload
```

Certified behaviors:

- Awards preserved.
- PK preserved.
- Mule preserved.
- Rescue Squad preserved.
- Main Character preserved.
- Security level preserved.
- No field shifting.
- Legacy awards tail field preserved through `ADMIN_ONLY`.

### Search Path

```text
Search Workspace -> Rank/Race/Skill filters -> Results Grid
```

Certified behaviors:

- Legacy Search workspace restored.
- Rank filter.
- Race filter.
- Skill minimum filters.
- Mixed filters.
- Reset.
- Results grid.

## Known Legacy Deviations

None known for certified 0.11 user-facing legacy behavior.

## Explicit Non-Legacy Items Deferred

The following are not part of 0.11 because they were not VB6 desktop-client behavior or are modern improvements:

- Delete Character in the desktop client.
- Live search filtering.
- Search export.
- Saved searches.
- Column sorting.
- Double-click any character list to open Character Dialog.
- Smart tab auto-loading.
- Cached search index.

These are tracked in `AI_MODERNIZATION_BACKLOG.md`.

## Final 0.11 Work Remaining

0.11.20 Final Legacy Certification should perform one full end-to-end regression pass across all Gold-certified subsystems and freeze the certified legacy baseline.
