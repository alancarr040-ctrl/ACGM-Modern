# Engineering Notes – 0.11.13a

## Diagnostics Configuration Correction

0.11.13 introduced the Character Save Certification Suite, but its runtime toggle was not exposed through the intended `[Diagnostics]` section in `acgm.ini`.

0.11.13a adds section-aware INI parsing for:

```ini
[Diagnostics]
CharacterSave=0
```

Truthy values: `1`, `true`, `yes`, `on`, `enabled`.

Missing or false values keep diagnostics disabled.

The legacy 0.11.13 top-level/environment switches remain supported for compatibility, but `[Diagnostics] CharacterSave` is the canonical configuration going forward.

No certified behavior was changed.

## 0.11.13a Config Persistence Revision

The character-save diagnostics switch is now part of the normal `acgm.ini` configuration model. The client loads `[Diagnostics] CharacterSave` with the rest of the application settings and writes it back during the normal save-on-exit process, so the setting survives client shutdown.

Use:

```ini
[Diagnostics]
CharacterSave = 0
```

Set `CharacterSave = 1` to enable `character-save-diagnostics.log`. Missing or disabled settings keep production behavior unchanged. The `[Diagnostics]` section is not treated as a server entry.
