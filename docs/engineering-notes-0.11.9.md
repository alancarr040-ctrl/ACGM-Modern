# ACGM Modern Client 0.11.9 - File Login Restoration

## Scope

0.11.9 restores the legacy File -> Login menu item while preserving all verified 0.11.8 protocol and parser behavior.

## Implementation Notes

- `MainForm` login/session fields are mutable so the active main window can switch credentials.
- `File -> Login` opens `PlayerLoginForm` using existing settings.
- On successful login, the endpoint, character name, password, and `AcgmProtocolService` are replaced.
- Current character/editor state is cleared.
- The title bar updates to the new character.
- The allegiance tree refreshes automatically.

## Unchanged Behavior

- Tree parser unchanged.
- Character detail parser unchanged.
- Awards parser/save behavior unchanged.
- Administrator protocol behavior unchanged.
- Server setup behavior unchanged.
