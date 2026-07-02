# ACGM Legacy Protocol Reference

Version: 0.11.19 - Legacy Protocol Inventory & Documentation

This document is the definitive protocol reference for the certified 0.11 legacy recreation baseline. It documents the legacy `server.cgi` message model implemented by the Modern Client.

## 1. Protocol Philosophy

The Modern Client preserves the observable VB6 client/server protocol. Modernization is internal only.

Rules:

- Preserve legacy `msgid` values.
- Preserve legacy POST field names.
- Preserve legacy response framing.
- Preserve legacy custom value escaping.
- Do not replace legacy behavior with modern behavior unless a package explicitly declares modernization work.

## 2. Transport

The VB6 client manually created an HTTP POST request and used `User-Agent: ACGM 0.3`.

The Modern Client uses `HttpClient` while preserving compatible request behavior:

- `POST` to the configured `server.cgi` endpoint.
- `Content-Type: application/x-www-form-urlencoded`.
- `User-Agent: ACGM 0.3`.
- `Connection: close`.
- HTTPS support is modernized underneath without changing the CGI protocol.

## 3. Shared Request Fields

Most requests begin with the prepared legacy fields:

| Field | Meaning |
|---|---|
| `charname` | Login/current character name |
| `password` | Login/current character password |
| `msgid` | Legacy operation identifier |

Additional fields are added per operation.

## 4. Legacy POST Encoding

The legacy client did not use ordinary URL encoding for all data. The Modern Client preserves the custom substitutions used by the VB6 code:

| Original | Encoded |
|---|---|
| `|` | `(pipe)` |
| `!;` | `(end)` |
| `&` | `(amp)` |

This is implemented by `LegacyPostEncoder` and is required for CGI compatibility.

## 5. Response Framing

The legacy CGI response is framed as:

```text
<START_HERE><result-code>;<payload><STOP_HERE>
```

The Modern Client parses the result code and payload from this envelope.

Known result codes:

| Code | Meaning | Status |
|---:|---|---|
| 800 | OK | Certified |
| 801 | New server | Preserved |

Codes from `800` through `899` are treated as success by the modern response model.

## 6. Message Inventory

| Message | ID | Direction | Purpose | Certification Status |
|---|---:|---|---|---|
| `MSG_GET_TREE` | 100 | Client -> Server | Download allegiance tree payload | Gold |
| `MSG_ADD_VASSAL` | 101 | Client -> Server | Add a vassal under a patron | Gold |
| `MSG_REMOVE_VASSAL` | 102 | Client -> Server | Remove vassal relationship | Documented, not exposed as Delete Character |
| `MSG_CHANGE_PATRON` | 103 | Client -> Server | Move a character to a different patron | Gold |
| `MSG_LOGIN` | 104 | Client -> Server | Authenticate login character | Gold |
| `MSG_INIT_NEW_SERVER` | 105 | Client -> Server | Legacy new-server initialization | Documented |
| `MSG_UPDATE_CHAR_INFO` | 106 | Client -> Server | Save character-detail edits | Gold |
| `MSG_GET_CHAR_INFO` | 107 | Client -> Server | Download character-detail record | Gold |
| `MSG_CHANGE_PASSWORD` | 108 | Client -> Server | Change current character password | Gold |
| `MSG_CHANGE_SERVER_SETUP` | 109 | Client -> Server | Save server setup options | Gold |
| `MSG_RESET_PASSWORD` | 110 | Client -> Server | Reset another character password | Gold |
| `MSG_BACKUP_DB` | 111 | Client -> Server | Request legacy database backup payload | Gold |
| `MSG_CHANGE_SEC_LEVEL` | 112 | Client -> Server | Change a character security level | Gold |
| `MSG_GET_RESCUE_SQUAD` | 113 | Client -> Server | Retrieve Rescue Squad list | Gold |
| `MSG_GET_PORTAL_LIST` | 114 | Client -> Server | Retrieve Summonable Portal List | Gold |
| `MSG_GET_TRADE_SKILL_LIST` | 115 | Client -> Server | Retrieve Trade Skills List | Gold |
| `MSG_SEARCH` | 116 | Client -> Server | Legacy search message | Documented; modern 0.11.16 Search uses local certified data |

## 7. Authentication

### `MSG_LOGIN = 104`

Direction: Client -> Server

Purpose: Authenticate the login character and retrieve login/session state.

POST fields:

- `charname`
- `password`
- `msgid=104`

Certification: Gold.

## 8. Tree Download

### `MSG_GET_TREE = 100`

Direction: Client -> Server

Purpose: Retrieve the legacy compact tree payload.

POST fields currently used by the Modern Client:

- `charname`
- `password`
- `msgid=100`

Legacy VB6 also supported display/filter options such as tree type, root id, mule display, security display, PK display, and rescue display. The modern certified tree path preserves the server-compatible compact tree response and does not alter the CGI format.

### Tree Record Format

Records are separated by `!;`.

Each record uses nine pipe-delimited fields:

```text
id|name|level|rank|patron_id|vassal_ids|is_mule|is_pk|is_rescue_squad_member
```

Field mapping:

| Index | Field | Meaning |
|---:|---|---|
| 0 | `id` | Character ID |
| 1 | `name` | Character name |
| 2 | `level` | Character level |
| 3 | `rank` | Allegiance rank |
| 4 | `patron_id` | Patron character ID |
| 5 | `vassal_ids` | Comma-separated vassal IDs |
| 6 | `is_mule` | Mule flag |
| 7 | `is_pk` | PK flag |
| 8 | `is_rescue_squad_member` | Rescue Squad flag |

Awards are not parsed from tree records.

Certification: Gold.

## 9. Character Download

### `MSG_GET_CHAR_INFO = 107`

Direction: Client -> Server

Purpose: Retrieve the full character-detail record for display/editing.

POST fields:

- `charname`
- `password`
- `msgid=107`
- `getcharid`

Character detail response records are pipe-delimited. Certified important mappings:

| Index | Meaning |
|---:|---|
| 0 | Character ID |
| 1 | Name |
| 2 | Level |
| 3 | Rank |
| 6 | Follower Count |
| 8 | Class |
| 9 | Race |
| 10 | Sex |
| 11 | Is Mule |
| 12 | Mule For |
| 13 | Lifestoned At |
| 14 | Tied To |
| 15 | Biography |
| 16 | Skills |
| 17 | Real Name |
| 18 | City/State |
| 19 | Misc Real Life |
| 20 | Email |
| 21 | ICQ |
| 23 | Last Modified |
| 24 | Patron |
| 25 | Path To Monarch |
| 26 | Is PK |
| 27 | Is Rescue Squad Member |
| 28 | Hide Info On Web |
| 29 | Can Summon |
| 30 | Main Character |
| 31 | Last Modified By / ID |
| final | Awards/Admin-only field |

Awards are read from the final field, not from a fixed field index.

Certification: Gold.

## 10. Character Save

### `MSG_UPDATE_CHAR_INFO = 106`

Direction: Client -> Server

Purpose: Save character-detail edits.

Required POST fields:

- `charname`
- `password`
- `charid` - login/current character ID
- `updateid` - edited character ID
- `msgid=106`

Legacy character fields supported by the certified serializer:

| Field | Meaning |
|---|---|
| `NAME` | Character name |
| `LEVEL` | Level |
| `CLASS` | Class |
| `RACE` | Race |
| `RANK` | Rank |
| `SEX` | Sex |
| `ISMULE` | Mule flag |
| `MULEFOR` | Mule For |
| `LSAT` | Lifestoned At |
| `TIEDTO` | Tied To |
| `BIO` | Biography |
| `ISPK` | PK flag |
| `ISRESCUE` | Rescue Squad flag |
| `ISPRIVATE` | Hide Info On Web |
| `CAN_SUMMON` | Can Summon |
| `MAIN_CHAR` | Main Character |
| `SKILLS` | Skills payload |
| `REALNAME` | Real Name |
| `CITYSTATE` | City/State |
| `MISCINFO` | Misc Real Life |
| `EMAIL` | Email |
| `ICQ` | ICQ |
| `ADMIN_ONLY` | Awards/Admin-only final field |

Certified behavior:

- Changed fields are posted using legacy field names.
- `ADMIN_ONLY` is always included to preserve the legacy awards tail field.
- Awards are serialized with `!;` delimiters before POST encoding.
- PK, Mule, Rescue Squad, Main Character, Security, Monarch-related state, and Awards persist after save, refresh, reload, and restart.
- Tree parser and icon behavior are not modified by character-save certification.

Certification: Gold.

## 11. Current Player Functions

### Add Vassal - `MSG_ADD_VASSAL = 101`

POST fields:

- `charname`
- `password`
- `msgid=101`
- `charid`
- `addtoid`
- `vassalname`
- `vassallist`
- `patronid`

Certification: Gold.

### Change Patron - `MSG_CHANGE_PATRON = 103`

POST fields:

- `charname`
- `password`
- `msgid=103`
- `moveid`
- `charid`
- `oldpatronid`
- `oldpatronname`
- `oldpatronvassallist`
- `newpatronid`
- `newpatronname`
- `newpatronvassallist`

Certification: Gold.

### Change Password - `MSG_CHANGE_PASSWORD = 108`

POST fields:

- `charname`
- `password`
- `msgid=108`
- `charid`
- `newpw`
- `confirmpw`

Certification: Gold.

### Remove Vassal - `MSG_REMOVE_VASSAL = 102`

POST fields documented in the modern protocol wrapper:

- `charname`
- `password`
- `msgid=102`
- `charid`
- `patronid`
- `vassalid`
- `vassalname`
- `vassallist`

Important: VB6 did not expose Delete Character in the desktop client. Character Delete is not certified as a 0.11 desktop-client function and is deferred to modernization as an administrator-only enhancement.

Certification: Documented, not part of current user-facing certification.

## 12. Administration

### Server Setup - `MSG_CHANGE_SERVER_SETUP = 109`

POST fields:

- `charname`
- `password`
- `msgid=109`
- `guildname`
- `use_rescue_squad`
- `rescue_squad_name`
- `resetpw`
- `defaultpw` only when a new default password is entered

Certification: Gold.

### Reset Password - `MSG_RESET_PASSWORD = 110`

POST fields:

- `charname`
- `password`
- `msgid=110`
- `charid`
- `resetid`

Certification: Gold.

### Backup Database - `MSG_BACKUP_DB = 111`

POST fields:

- `charname`
- `password`
- `msgid=111`
- `charid`

Certification: Gold.

### Change Security Level - `MSG_CHANGE_SEC_LEVEL = 112`

POST fields:

- `charname`
- `password`
- `msgid=112`
- `charid`
- `changeid`
- `newlevel`

Legacy security levels:

| Level | Meaning |
|---:|---|
| 1 | Normal User |
| 3 | Administrator |

Certification: Gold.

## 13. Utility Lists

Utility-list responses use rows separated by `!;` and fields separated by `|`.

### Rescue Squad - `MSG_GET_RESCUE_SQUAD = 113`

POST fields:

- `charname`
- `password`
- `msgid=113`

Displayed columns:

- Name
- Level
- Lifestone
- Tied To
- Can Summon
- Main Character Name
- Email Address
- ICQ Number

Certification: Gold.

### Summonable Portal List - `MSG_GET_PORTAL_LIST = 114`

POST fields:

- `charname`
- `password`
- `msgid=114`

Displayed columns:

- Name
- Level
- Tied To
- Lifestone

Certification: Gold.

### Trade Skills List - `MSG_GET_TRADE_SKILL_LIST = 115`

POST fields:

- `charname`
- `password`
- `msgid=115`

Displayed columns:

- Name
- Level
- Alchemy
- Cooking
- Fletching

Certification: Gold.

## 14. Search

### `MSG_SEARCH = 116`

Legacy purpose: server-side search support.

Documented legacy fields:

- `race`
- `rank`
- `skillsearch`

The 0.11.16 Modern Client restores the legacy Search workspace and certified search behavior using locally available character/tree/skill data. The legacy `MSG_SEARCH` ID remains reserved and documented.

Certification: Search workspace and behavior are Gold. `MSG_SEARCH` protocol remains documented for legacy completeness.

## 15. Diagnostics

Character-save diagnostics are developer-only and controlled by `acgm.ini`:

```ini
[Diagnostics]
CharacterSave = 0
```

Set `CharacterSave = 1` to enable:

```text
logs/character-save-diagnostics.log
```

Diagnostics are disabled by default and must not affect production behavior.

Certification: Gold.

## 16. Known Legacy Behaviors

- The Search system is a workspace tab, not a modal dialog.
- Current Player Functions are menu actions, not Character Dialog tabs.
- Tree refresh preserves selected deep-node visibility but may collapse lower branches when a top-level character is selected.
- The client restores the previous tree location/selection after restart.
- Delete Character is not a VB6 desktop-client function.

## 17. Certification Status

All documented user-facing 0.11 legacy protocol behaviors are certified or explicitly documented as not exposed by the VB6 desktop client.

The final remaining 0.11 task is 0.11.20 Final Legacy Certification.
