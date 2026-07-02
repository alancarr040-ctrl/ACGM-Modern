# Engineering Notes - 0.11.13

## Package

ACGM Modern Client 0.11.13 - Legacy Character Save Certification Suite

## Purpose

This package adds a permanent developer-only certification suite around the already-certified `MSG_UPDATE_CHAR_INFO` character-save path.

The package does not alter character-save production behavior, tree parsing, icon behavior, character parsing, awards display, menus, login, HTTPS, or administrator functions.

## Certification Mode

Character-save diagnostics are now opt-in.

Diagnostics may be enabled without recompiling by using one of the following methods:

```text
ACGM_CHARACTER_SAVE_CERTIFICATION=1
```

or in `acgm.ini`:

```text
CharacterSaveCertification = 1
```

or:

```text
EnableCharacterSaveDiagnostics = 1
```

When enabled, the client writes:

```text
logs/character-save-diagnostics.log
```

When disabled, no character-save certification log is generated.

## Logged Data

The certification log records:

- Timestamp
- Character name and save context
- Original and updated character values
- Expected VB6 `MSG_UPDATE_CHAR_INFO` POST field order
- Current serializer field order and values
- Field-by-field comparison
- Final serialized POST payload
- CGI server response
- Reloaded character value guidance for manual certification

## Scope Control

Only diagnostics around the outgoing character-save serializer were changed.

No certified subsystem was modified.

## Testing Checklist

1. Run client with certification disabled.
2. Save a character.
3. Confirm normal operation remains unchanged.
4. Confirm no `character-save-diagnostics.log` is generated.
5. Enable certification mode.
6. Save William Ohmsford or another known test character.
7. Confirm `character-save-diagnostics.log` is generated.
8. Confirm field order, payload, and server response are recorded.
9. Refresh tree and reload the character.
10. Confirm no behavior regressions.
