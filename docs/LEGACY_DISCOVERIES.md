# Legacy Discoveries

This document records behavior learned during the 0.11 Legacy Certification phase. It is a knowledge base, not a feature request list.

## Tree Icons

Legacy icon assets and meanings:

- Normal -> `guy.bmp`
- Monarch -> `M.bmp`
- Mule -> `muleico.jpg`
- PK -> `pkico.jpg`
- Rescue Squad -> `rescueico.jpg`

The certified display order is Monarch/root first, then Mule, Rescue Squad, PK, and Normal.

## Tree Parser

`tree.dat` uses the legacy compact tree record format returned by `MSG_GET_TREE`.

Tree records are separated by `!;` and each record uses pipe-delimited fields:

```text
id|name|level|rank|patron_id|vassal_ids|is_mule|is_pk|is_rescue_squad_member
```

The tree payload does not include race. Awards are not read from tree records.

## Tree Behavior

Legacy Tree behavior has been verified and certified.

- Refresh Tree preserves visibility for a selected deep character by reopening parent branches as needed.
- Refreshing while a top-level/main character is selected may collapse lower expanded branches.
- Closing and reopening the client restores the previous tree location/selection.
- This behavior matches VB6 and requires no modernization.

## Character Detail Parser

Character details are loaded from `MSG_GET_CHAR_INFO` and mapped from the pipe-delimited legacy character-detail response.

Important certified fields include:

- ID
- Name
- Level
- Rank
- Follower count
- Class
- Race
- Sex
- Mule flag
- Mule For
- Lifestoned At
- Tied To
- Biography
- Skills
- Real Name
- City/State
- Misc Information
- Email
- ICQ
- Patron
- Path to Monarch
- PK flag
- Rescue Squad flag
- Hide on Web
- Can Summon
- Main Character
- Last modified fields
- Awards final field

## Character Save

VB6 preserves trailing empty fields.

Awards are serialized in the final character-detail field using the legacy `!;` delimiter.

`MSG_UPDATE_CHAR_INFO` preserves the legacy character-save layout and behavior. Do not collapse empty trailing values.

The modern client uses legacy field names and always preserves the `ADMIN_ONLY` awards tail field during character saves.

## Current Player Functions

The legacy VB6 Current Player Functions menu is separate from the Character Dialog tabs.

Certified actions:

- Add Vassal
- Change Patron
- Change Password

Basic Info, Allegiance Info, Skills, Real Life & Contact Info, and Awards remain Character Dialog tabs, not Current Player Functions menu commands.

## Search System

The VB6 client implemented Search as a dedicated workspace tab, not as a modal dialog.

The Search workspace included:

- Rank filter
- Race filter
- Skill filter grid
- Search
- Reset
- Results list

The Legacy Search System has been restored and certified against the original VB6 implementation.

Modern enhancements such as live filtering, sorting, export, cached search, and saved searches are intentionally deferred until Phase 0.12.

## Administration

Administrative behavior has been verified through accumulated certification.

Certified workflows include:

- Administrator menu visibility
- Backup Database
- Change Character Security Level
- Reset Character Password
- Server Setup
- Add Vassal
- Change Patron
- Change Password

VB6 did not expose Delete Character in the desktop client. Delete Character is therefore not part of 0.11 certification and has been recorded as a future administrator-only modernization item.
