## 0.11.19 - Legacy Protocol Inventory & Documentation

See `engineering-notes-0.11.19.md` for the documentation-focused protocol inventory, certification report, developer guide, and source audit.

## 0.11.13 - Legacy Character Save Certification Suite

See `engineering-notes-0.11.13.md` for the opt-in developer certification suite for `MSG_UPDATE_CHAR_INFO`.

## 0.11.12 - Legacy Character Save Serialization Certification

See `engineering-notes-0.11.12.md` for the certified `MSG_UPDATE_CHAR_INFO` serializer behavior.

## 0.11.11 - Legacy Tree Icon Polish

See `engineering-notes-0.11.11.md` for the certified tree icon asset and priority mapping, including PK icon selection.

# Engineering Notes - 0.11.4

## Intent

This release fixes parser correctness while preserving the project's core rule:

> Recreation first. Modernization underneath.

The UI remains presentation-only. Legacy field interpretation is isolated in `ACGM.Protocol`, and parsed state is exposed through `ACGM.Models`.

## Corrected legacy field map

The relevant zero-based legacy fields are:

| Field | Meaning |
|---:|---|
| 11 | ISMULE |
| 26 | ISPK |
| 27 | ISRESCUE |
| 28 | ISPRIVATE |
| 29 | CAN_SUMMON |
| 30 | MAIN_CHAR |
| 31 | LASTMODID |
| final | Awards |

The important rule is that Awards should be read from `fields[^1]`, not assumed to be a normal fixed field 32 value.

## Tree icon priority

The priority remains:

1. Mule
2. Rescue Squad
3. PK
4. Normal

This is implemented in the model layer through `TreeIcon` properties so the UI does not duplicate flag priority logic.

## Awards parsing

Awards are separated by the legacy delimiter:

```text
!;
```

The parser trims empty entries and returns one award per item. Serialization joins non-empty awards with the same delimiter.

## Compatibility

This release does not change:

- `msgid` values
- POST field names
- request format
- response format
- legacy pipe-delimited records

`LegacyServerClient` continues to post `msgid` values compatible with the legacy CGI endpoint.

## 0.11.4

See `engineering-notes-0.11.4.md` for the tree parser and awards mapping fix.

- 0.11.5: Legacy Menu System & Find Character Restoration (`engineering-notes-0.11.5.md`)

## 0.11.13a Config Persistence Revision

The character-save diagnostics switch is now part of the normal `acgm.ini` configuration model. The client loads `[Diagnostics] CharacterSave` with the rest of the application settings and writes it back during the normal save-on-exit process, so the setting survives client shutdown.

Use:

```ini
[Diagnostics]
CharacterSave = 0
```

Set `CharacterSave = 1` to enable `character-save-diagnostics.log`. Missing or disabled settings keep production behavior unchanged. The `[Diagnostics]` section is not treated as a server entry.
