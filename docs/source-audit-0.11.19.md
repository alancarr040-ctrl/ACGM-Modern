# Source Audit - 0.11.19

This audit records the source areas reviewed for the Legacy Protocol Inventory & Documentation package.

## Protocol Layer

- `ACGM.ModernClient/Protocol/AcgmConstants.cs`
  - Defines legacy message IDs 100 through 116.
  - Defines response markers `<START_HERE>` and `<STOP_HERE>`.
- `ACGM.ModernClient/Protocol/AcgmHttpClient.cs`
  - Implements the protocol-backed operations and legacy POST fields.
- `ACGM.ModernClient/Protocol/LegacyPostEncoder.cs`
  - Preserves legacy POST escaping.
- `ACGM.ModernClient/Protocol/AcgmResponse.cs`
  - Parses legacy response framing.

## Parser Layer

- `LegacyTreeParser.cs`
  - Parses compact tree records and certified icon flags.
- `LegacyCharacterParser.cs`
  - Parses full character-detail payloads and final awards field.
- `LegacySkillParser.cs`
  - Parses character skill payloads.
- `LegacyUtilityListParser.cs`
  - Parses Rescue Squad, Portal List, and Trade Skills list payloads.

## Model Layer

- `CharacterRecord.cs`
  - Tree/list model.
- `CharacterDetails.cs`
  - Character Dialog model.
- `UtilityRecords.cs`
  - Utility-list row models.

## UI Layer

- `Forms/MainForm.cs`
  - Main workspace tabs.
  - Tree behavior.
  - Current Player Functions.
  - Search workspace.
  - Character Dialog integration.

No source code changes were required for 0.11.19.
