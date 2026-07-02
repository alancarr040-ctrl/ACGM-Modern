# ACGM Modern Client 0.11.11 - Legacy Tree Icon Certification

## Scope

0.11.11 certifies the Allegiance Tree icon behavior. This package is intentionally visual-only. It does not change HTTPS, login, tree parsing, character parsing, character editing, character save serialization, `MSG_UPDATE_CHAR_INFO`, awards display/save UI, menus, administrator functions, Server Setup, Find Character, Security UI, msgids, POST fields, or legacy protocol behavior.

## Icon Assets

The following 16x16 assets are included under `ACGM.ModernClient/Assets/`:

- `M.bmp` - monarch/root icon.
- `guy.bmp` - normal member icon.
- `muleico.jpg` - mule icon.
- `pkico.jpg` - PK icon.
- `rescueico.jpg` - Rescue Squad icon.

## TreeView Icon Priority

The certified visual priority is:

1. Monarch/root: `PatronId == -1` -> `M.bmp`
2. Mule: `IsMule` -> `muleico.jpg`
3. Rescue Squad: `IsRescueSquad` -> `rescueico.jpg`
4. PK: `IsPk` -> `pkico.jpg`
5. Normal member: `guy.bmp`

## Certification Change

The verified 0.11.11 icon-polish source loaded `pkico.jpg` and parsed `IsPk`, but `GetTreeImageKey()` never returned the `pk` image key. This package adds the missing `IsPk` branch after Rescue Squad. Existing Monarch, Mule, Rescue Squad, and Normal icon paths are otherwise preserved.

## Build Notes

The project file continues to copy both `Assets/*.jpg` and `Assets/*.bmp` into the output directory.
