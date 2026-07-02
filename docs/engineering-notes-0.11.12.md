# Engineering Notes - 0.11.12

## Legacy Character Save Serialization Certification

0.11.12 is scoped exclusively to outgoing `MSG_UPDATE_CHAR_INFO` character-save serialization. Certified/frozen subsystems from 0.11.11, especially Tree Icons, were not modified.

## Root Cause

The legacy CGI update routine expects the Awards/Admin-only tail to remain available through the `ADMIN_ONLY` POST field. Previous modern saves only posted changed fields. Saving PK or another flag while Awards were unchanged could omit `ADMIN_ONLY`, allowing the server-side update path to collapse, replace, or lose the final Awards field.

## Fix

`AcgmHttpClient.UpdateCharacterInfoAsync` now always appends `ADMIN_ONLY` as the final character-save update field. Multiline Awards UI text is serialized back to the legacy `!;` delimiter format before `LegacyPostEncoder` performs the existing legacy escaping.

## Diagnostics

`logs/character-save-diagnostics.log` records:

- Current character values before save.
- Field index, field name, and field value.
- Expected VB6 `MSG_UPDATE_CHAR_INFO` POST order.
- Current serializer field order.
- Final serialized legacy payload.
- Outgoing POST payload.
- Server response.

## Certification Notes

Expected validation focuses on William Ohmsford:

1. Load character.
2. Confirm Awards display.
3. Toggle PK on and save.
4. Refresh Tree and verify PK icon.
5. Reload character and confirm PK remains checked.
6. Confirm Awards still include `Monster Smasher`.
7. Toggle PK off and save.
8. Refresh Tree and verify icon clears.
9. Reload/restart and confirm Awards and status fields remain correct.

