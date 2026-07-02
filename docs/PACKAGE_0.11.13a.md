# Package

0.11.13a

# Title

Diagnostics Configuration Correction

# Goal

Complete the 0.11.13 diagnostics toggle by supporting the documented `[Diagnostics]` section in `acgm.ini`.

# Scope

Configuration correction only. No production character-save behavior changes.

# Change

Character-save certification diagnostics are now controlled by:

```ini
[Diagnostics]
CharacterSave=0
```

Set `CharacterSave=1`, `true`, `yes`, `on`, or `enabled` to enable diagnostics.

If the section or setting is missing, diagnostics remain disabled.

# Compatibility

The older 0.11.13 emergency toggles remain supported:

- `CHARACTER_SAVE_CERTIFICATION=1`
- `ENABLE_CHARACTER_SAVE_DIAGNOSTICS=1`
- Environment variables prefixed with `ACGM_`

# Testing

1. Confirm `[Diagnostics] CharacterSave=0` creates no `character-save-diagnostics.log`.
2. Confirm `[Diagnostics] CharacterSave=1` creates `character-save-diagnostics.log` after saving a character.
3. Confirm normal character save behavior is unchanged.

# Version

ACGM Modern Client 0.11.13a – Diagnostics Configuration Correction
