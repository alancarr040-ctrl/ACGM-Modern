# Engineering Notes - ACGM Modern Client 0.11.5

## Package

ACGM Modern Client 0.11.5 - Legacy Menu System & Find Character Restoration

## Scope

0.11.5 is a UI restoration package based on the verified 0.11.4 release. It does not change protocol/parsing behavior.

## Restored Menu System

The prior menu bar contained placeholder string entries only. This package replaces them with real ToolStripMenuItem dropdowns:

- File
- Current Player Functions
- Tree Operations
- Options
- Administrator
- Help

Menu items are wired to existing safe behavior where available. Unrestored administrator behavior displays a clear placeholder message.

## Find Character

Find Character is restored for both the left-panel button and Tree Operations menu. It searches the loaded CharacterRecord collection using case-insensitive partial matching, prompts for selection when multiple matches are found, selects the corresponding TreeView node, expands parent nodes, ensures visibility, and loads character details.

## Compatibility

No msgid values, POST fields, request payloads, response formats, tree parsing, character detail parsing, awards parsing, or save serialization were changed.
