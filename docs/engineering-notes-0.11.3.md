# Engineering Notes 0.11.3 - Character Flags and Awards Diagnostics

This package intentionally avoids changing legacy server data. It adds diagnostic logging for character detail payloads and tree icon decisions so PK/Rescue icon precedence and Awards persistence can be corrected from observed server data rather than guesses.

## Diagnostic Log

At runtime, the client writes diagnostics to:

```text
<application folder>/logs/character-flags-awards-diagnostics.log
```

The log includes:

- Raw character detail payload returned by `server.cgi`.
- Indexed field dump after splitting on `|`.
- Parsed PK, Rescue Squad, Mule, and Awards/Admin values.
- Unknown trailing fields after the current expected Awards/Admin field.
- Tree record flags and selected icon reason for each loaded character.

## Safety

Diagnostics are append-only local files. They do not alter request formats, response formats, or server data.

## Current areas under investigation

- Whether the tree payload includes Rescue Squad status in the same field currently parsed as `IsRescueSquad`.
- Whether Awards are stored at field 32, another character detail field, or a separate legacy update field.
- Whether `ADMIN_ONLY` is the correct update key for Awards in all server versions.
