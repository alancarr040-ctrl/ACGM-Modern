# Engineering Notes - 0.7.0

## Focus

Package 0.7.0 adds the first functional implementation of the legacy Allegiance Info tab without changing the server protocol.

## Design decision

The legacy server does not expose all allegiance values as a clean modern object. The modern client therefore builds an `AllegianceInfo` model from two compatible legacy sources:

1. The selected character detail response.
2. The downloaded allegiance tree.

This keeps the UI free of parsing/derivation logic while preserving compatibility with current `server.cgi` behavior.

## Added files

- `Models/AllegianceInfo.cs`
- `Protocol/AllegianceInfoBuilder.cs`

## UI behavior

The Allegiance Info tab is read-only in this package. It displays derived information and allows navigation by double-clicking a direct vassal.

## Future work

- Compare output against the original VB6 Allegiance Info tab.
- Add any missing legacy-only fields discovered in the old form code or live server payloads.
- Consider an allegiance relationship editing package only after display behavior is verified.
